using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D.Examples
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dColorCounter")]
	[AddComponentMenu("Paint in 3D/Examples/Color Counter")]
	public class P3dColorCounter : P3dPaintableTextureMonitorMask
	{
		public class Contribution
		{
			public P3dColor Color;

			public int Count;

			public float Ratio;

			public byte R;

			public byte G;

			public byte B;

			public byte A;

			public static Stack<Contribution> Pool = new Stack<Contribution>();
		}

		public static LinkedList<P3dColorCounter> Instances = new LinkedList<P3dColorCounter>();

		private LinkedListNode<P3dColorCounter> node;

		[SerializeField]
		private int downsampleSteps = 3;

		[Range(0f, 1f)]
		[SerializeField]
		private float threshold = 0.5f;

		[NonSerialized]
		private List<Contribution> contributions = new List<Contribution>();

		public int DownsampleSteps
		{
			get
			{
				return downsampleSteps;
			}
			set
			{
				downsampleSteps = value;
			}
		}

		public float Threshold
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public List<Contribution> Contributions => contributions;

		protected override void OnEnable()
		{
			base.OnEnable();
			node = Instances.AddLast(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Instances.Remove(node);
			node = null;
			Contribute(0);
		}

		protected override void UpdateMonitor(P3dPaintableTexture paintableTexture, bool preview)
		{
			if (!preview && paintableTexture.Activated)
			{
				RenderTexture current = paintableTexture.Current;
				RenderTexture temporary = null;
				if (P3dHelper.Downsample(current, downsampleSteps, ref temporary))
				{
					Calculate(temporary, temporary, 1 << downsampleSteps);
					P3dHelper.ReleaseRenderTexture(temporary);
				}
				else
				{
					Calculate(current, temporary, 1);
				}
			}
		}

		private void Calculate(RenderTexture renderTexture, RenderTexture temporary, int scale)
		{
			int num = (int)(threshold * 255f);
			int width = renderTexture.width;
			int height = renderTexture.height;
			Texture2D readableCopy = P3dHelper.GetReadableCopy(renderTexture);
			Color32[] pixels = readableCopy.GetPixels32();
			P3dHelper.Destroy(readableCopy);
			UpdateTotal(renderTexture, width, height, renderTexture.format, scale);
			PrepareContributions();
			for (int i = 0; i < height; i++)
			{
				int num2 = i * width;
				for (int j = 0; j < height; j++)
				{
					int num3 = num2 + j;
					if (baked && !bakedPixels[num3])
					{
						continue;
					}
					Color32 color = pixels[num3];
					int num4 = -1;
					int num5 = num;
					for (int k = 0; k < P3dLinkedBehaviour<P3dColor>.InstanceCount; k++)
					{
						Contribution contribution = contributions[k];
						int num6 = 0;
						num6 += Math.Abs(contribution.R - color.r);
						num6 += Math.Abs(contribution.G - color.g);
						num6 += Math.Abs(contribution.B - color.b);
						num6 += Math.Abs(contribution.A - color.a);
						if (num6 <= num5)
						{
							num4 = k;
							num5 = num6;
						}
					}
					if (num4 >= 0)
					{
						contributions[num4].Count++;
					}
				}
			}
			Contribute(scale);
		}

		private void ClearContributions()
		{
			for (int num = contributions.Count - 1; num >= 0; num--)
			{
				Contribution.Pool.Push(contributions[num]);
			}
			contributions.Clear();
		}

		private void PrepareContributions()
		{
			ClearContributions();
			P3dColor p3dColor = P3dLinkedBehaviour<P3dColor>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dColor>.InstanceCount; i++)
			{
				Contribution contribution = ((Contribution.Pool.Count > 0) ? Contribution.Pool.Pop() : new Contribution());
				Color32 color = p3dColor.Color;
				contribution.Color = p3dColor;
				contribution.Count = 0;
				contribution.R = color.r;
				contribution.G = color.g;
				contribution.B = color.b;
				contribution.A = color.a;
				contributions.Add(contribution);
				p3dColor = p3dColor.NextInstance;
			}
		}

		private void Contribute(int scale)
		{
			float num = ((total > 0) ? (1f / (float)total) : 1f);
			for (int num2 = contributions.Count - 1; num2 >= 0; num2--)
			{
				Contribution contribution = contributions[num2];
				contribution.Count *= scale;
				contribution.Ratio = (float)contribution.Count * num;
				if (contribution.Color != null)
				{
					contribution.Color.Contribute(this, contribution.Count);
				}
				if (contribution.Count <= 0)
				{
					Contribution.Pool.Push(contribution);
					contributions.RemoveAt(num2);
				}
			}
		}
	}
}
