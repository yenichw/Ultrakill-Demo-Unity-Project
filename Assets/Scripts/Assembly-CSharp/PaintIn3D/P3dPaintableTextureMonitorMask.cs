using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dPaintableTextureMonitorMask : P3dPaintableTextureMonitor
	{
		[SerializeField]
		private Mesh mesh;

		[SerializeField]
		protected int total;

		[SerializeField]
		protected bool baked;

		[SerializeField]
		private Mesh bakedMesh;

		[SerializeField]
		private Vector2Int bakedSize;

		[SerializeField]
		protected List<bool> bakedPixels;

		private static Material cachedMaterial;

		public Mesh Mesh
		{
			get
			{
				return mesh;
			}
			set
			{
				mesh = value;
			}
		}

		public int Total => total;

		public void ClearBake()
		{
			if (baked)
			{
				baked = false;
				bakedMesh = null;
				bakedSize = Vector2Int.zero;
				bakedPixels.Clear();
			}
		}

		protected void UpdateTotal(RenderTexture temporary, int width, int height, RenderTextureFormat format, int scale)
		{
			if (mesh != null)
			{
				if (!baked || bakedMesh != mesh || bakedSize.x != width || bakedSize.y != height)
				{
					Bake(temporary, width, height, format, scale);
				}
			}
			else
			{
				ClearBake();
				total = width * height * scale;
			}
		}

		private void Bake(RenderTexture temporary, int width, int height, RenderTextureFormat format, int scale)
		{
			if (bakedPixels == null)
			{
				bakedPixels = new List<bool>();
			}
			else
			{
				bakedPixels.Clear();
			}
			baked = true;
			bakedMesh = mesh;
			bakedSize = new Vector2Int(width, height);
			if (cachedMaterial == null)
			{
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/White");
			}
			cachedMaterial.SetVector(P3dShader._Channel, P3dHelper.IndexToVector((int)base.PaintableTexture.Channel));
			RenderTexture active = RenderTexture.active;
			RenderTexture renderTexture = temporary;
			if (temporary == null)
			{
				renderTexture = P3dHelper.GetRenderTexture(new RenderTextureDescriptor(width, height, format, 0));
			}
			RenderTexture.active = renderTexture;
			GL.Clear(clearDepth: true, clearColor: true, Color.black);
			cachedMaterial.SetPass(0);
			Graphics.DrawMeshNow(mesh, Matrix4x4.identity, base.PaintableTexture.Slot.Index);
			RenderTexture.active = active;
			Texture2D readableCopy = P3dHelper.GetReadableCopy(renderTexture);
			if (temporary == null)
			{
				P3dHelper.ReleaseRenderTexture(renderTexture);
			}
			bakedPixels.Capacity = width * height;
			total = 0;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (readableCopy.GetPixel(j, i).r > 0.5f)
					{
						bakedPixels.Add(item: true);
						total += scale;
					}
					else
					{
						bakedPixels.Add(item: false);
					}
				}
			}
			P3dHelper.Destroy(readableCopy);
		}
	}
}
