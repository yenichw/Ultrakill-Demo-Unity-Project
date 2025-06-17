using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModel")]
	[AddComponentMenu("Paint in 3D/Model")]
	public class P3dModel : P3dLinkedBehaviour<P3dModel>
	{
		[SerializeField]
		protected P3dPaintable paintable;

		[SerializeField]
		protected bool includeScale = true;

		[NonSerialized]
		private Renderer cachedRenderer;

		[NonSerialized]
		private bool cachedRendererSet;

		[NonSerialized]
		private SkinnedMeshRenderer cachedSkinned;

		[NonSerialized]
		private MeshFilter cachedFilter;

		[NonSerialized]
		private bool cachedSkinnedSet;

		[NonSerialized]
		private Transform cachedTransform;

		[NonSerialized]
		private GameObject cachedGameObject;

		[NonSerialized]
		private Material[] materials;

		[NonSerialized]
		private bool materialsSet;

		[NonSerialized]
		private Mesh bakedMesh;

		[NonSerialized]
		private bool bakedMeshSet;

		[NonSerialized]
		protected bool prepared;

		[NonSerialized]
		private Mesh preparedMesh;

		[NonSerialized]
		private Matrix4x4 preparedMatrix;

		[NonSerialized]
		private static List<P3dModel> tempModels = new List<P3dModel>();

		public virtual P3dPaintable Paintable
		{
			get
			{
				return paintable;
			}
			set
			{
				paintable = value;
			}
		}

		public virtual bool IncludeScale
		{
			get
			{
				return includeScale;
			}
			set
			{
				includeScale = value;
			}
		}

		public bool Prepared
		{
			get
			{
				return prepared;
			}
			set
			{
				prepared = value;
			}
		}

		public GameObject CachedGameObject => cachedGameObject;

		public Renderer CachedRenderer
		{
			get
			{
				if (!cachedRendererSet)
				{
					CacheRenderer();
				}
				return cachedRenderer;
			}
		}

		public Material[] Materials
		{
			get
			{
				if (!materialsSet)
				{
					materials = CachedRenderer.sharedMaterials;
					materialsSet = true;
				}
				return materials;
			}
		}

		private void CacheRenderer()
		{
			cachedRenderer = GetComponent<Renderer>();
			cachedRendererSet = true;
			if (cachedRenderer is SkinnedMeshRenderer)
			{
				cachedSkinned = (SkinnedMeshRenderer)cachedRenderer;
				cachedSkinnedSet = true;
			}
			else
			{
				cachedFilter = GetComponent<MeshFilter>();
			}
		}

		[ContextMenu("Dirty Materials")]
		public void DirtyMaterials()
		{
			materialsSet = false;
		}

		public static List<P3dModel> FindOverlap(Vector3 position, float radius, int layerMask)
		{
			tempModels.Clear();
			P3dModel p3dModel = P3dLinkedBehaviour<P3dModel>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dModel>.InstanceCount; i++)
			{
				if (P3dHelper.IndexInMask(p3dModel.CachedGameObject.layer, layerMask) && p3dModel.Paintable != null)
				{
					Bounds bounds = p3dModel.CachedRenderer.bounds;
					float num = radius + bounds.extents.magnitude;
					num *= num;
					if (Vector3.SqrMagnitude(position - bounds.center) < num)
					{
						tempModels.Add(p3dModel);
						if (!p3dModel.paintable.Activated)
						{
							p3dModel.paintable.Activate();
						}
					}
				}
				p3dModel = p3dModel.NextInstance;
			}
			return tempModels;
		}

		public void GetPrepared(ref Mesh mesh, ref Matrix4x4 matrix)
		{
			if (!prepared)
			{
				prepared = true;
				if (!cachedRendererSet)
				{
					CacheRenderer();
				}
				if (cachedSkinnedSet)
				{
					if (!bakedMeshSet)
					{
						bakedMesh = new Mesh();
						bakedMeshSet = true;
					}
					Vector3 vector = P3dHelper.Reciprocal3(cachedTransform.lossyScale);
					Vector3 localScale = cachedTransform.localScale;
					cachedTransform.localScale = Vector3.one;
					cachedSkinned.BakeMesh(bakedMesh);
					cachedTransform.localScale = localScale;
					preparedMesh = bakedMesh;
					preparedMatrix = cachedTransform.localToWorldMatrix;
					if (includeScale)
					{
						preparedMatrix *= Matrix4x4.Scale(vector);
					}
				}
				else
				{
					preparedMesh = cachedFilter.sharedMesh;
					preparedMatrix = cachedTransform.localToWorldMatrix;
				}
			}
			mesh = preparedMesh;
			matrix = preparedMatrix;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			cachedGameObject = base.gameObject;
			cachedTransform = base.transform;
		}

		protected virtual void OnDestroy()
		{
			P3dHelper.Destroy(bakedMesh);
		}
	}
}
