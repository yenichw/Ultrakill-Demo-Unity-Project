using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D.Examples
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dChannelCounter")]
	[AddComponentMenu("Paint in 3D/Examples/Channel Counter")]
	public class P3dChannelCounter : P3dPaintableTextureMonitorMask
	{
		public static LinkedList<P3dChannelCounter> Instances = new LinkedList<P3dChannelCounter>();

		private LinkedListNode<P3dChannelCounter> node;

		[SerializeField]
		private int downsampleSteps = 3;

		[Range(0f, 1f)]
		[SerializeField]
		private float threshold = 0.5f;

		[SerializeField]
		private int solidR;

		[SerializeField]
		private int solidG;

		[SerializeField]
		private int solidB;

		[SerializeField]
		private int solidA;

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

		public int SolidR => solidR;

		public int SolidG => solidG;

		public int SolidB => solidB;

		public int SolidA => solidA;

		public float RatioR
		{
			get
			{
				if (total <= 0)
				{
					return 0f;
				}
				return (float)solidR / (float)total;
			}
		}

		public float RatioG
		{
			get
			{
				if (total <= 0)
				{
					return 0f;
				}
				return (float)solidG / (float)total;
			}
		}

		public float RatioB
		{
			get
			{
				if (total <= 0)
				{
					return 0f;
				}
				return (float)solidB / (float)total;
			}
		}

		public float RatioA
		{
			get
			{
				if (total <= 0)
				{
					return 0f;
				}
				return (float)solidA / (float)total;
			}
		}

		public Vector4 RatioRGBA
		{
			get
			{
				if (total > 0)
				{
					Vector4 result = default(Vector4);
					float num = 1f / (float)total;
					result.x = Mathf.Clamp01((float)solidR * num);
					result.y = Mathf.Clamp01((float)solidG * num);
					result.z = Mathf.Clamp01((float)solidB * num);
					result.w = Mathf.Clamp01((float)solidA * num);
					return result;
				}
				return Vector4.zero;
			}
		}

		public static long GetTotal(ICollection<P3dChannelCounter> counters = null)
		{
			long num = 0L;
			foreach (P3dChannelCounter item in counters ?? Instances)
			{
				num += item.total;
			}
			return num;
		}

		public static long GetSolidR(ICollection<P3dChannelCounter> counters = null)
		{
			long num = 0L;
			foreach (P3dChannelCounter item in counters ?? Instances)
			{
				num += item.solidR;
			}
			return num;
		}

		public static long GetSolidG(ICollection<P3dChannelCounter> counters = null)
		{
			long num = 0L;
			foreach (P3dChannelCounter item in counters ?? Instances)
			{
				num += item.solidG;
			}
			return num;
		}

		public static long GetSolidB(ICollection<P3dChannelCounter> counters = null)
		{
			long num = 0L;
			foreach (P3dChannelCounter item in counters ?? Instances)
			{
				num += item.solidB;
			}
			return num;
		}

		public static long GetSolidA(ICollection<P3dChannelCounter> counters = null)
		{
			long num = 0L;
			foreach (P3dChannelCounter item in counters ?? Instances)
			{
				num += item.solidA;
			}
			return num;
		}

		public static Vector4 GetRatioRGBA(ICollection<P3dChannelCounter> counters = null)
		{
			if (counters == null)
			{
				counters = Instances;
			}
			if (counters.Count > 0)
			{
				Vector4 zero = Vector4.zero;
				foreach (P3dChannelCounter counter in counters)
				{
					zero += counter.RatioRGBA;
				}
				return zero / Instances.Count;
			}
			return Vector4.zero;
		}

		protected override void OnEnable()
		{
			node = Instances.AddLast(this);
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Instances.Remove(node);
			node = null;
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
			byte b = (byte)(threshold * 255f);
			int width = renderTexture.width;
			int height = renderTexture.height;
			Texture2D readableCopy = P3dHelper.GetReadableCopy(renderTexture);
			Color32[] pixels = readableCopy.GetPixels32();
			P3dHelper.Destroy(readableCopy);
			UpdateTotal(temporary, width, height, renderTexture.format, scale);
			solidR = 0;
			solidG = 0;
			solidB = 0;
			solidA = 0;
			for (int i = 0; i < height; i++)
			{
				int num = i * width;
				for (int j = 0; j < height; j++)
				{
					int num2 = num + j;
					if (!baked || bakedPixels[num2])
					{
						Color32 color = pixels[num2];
						if (color.r >= b)
						{
							solidR++;
						}
						if (color.g >= b)
						{
							solidG++;
						}
						if (color.b >= b)
						{
							solidB++;
						}
						if (color.a >= b)
						{
							solidA++;
						}
					}
				}
			}
			solidR *= scale;
			solidG *= scale;
			solidB *= scale;
			solidA *= scale;
		}
	}
}
