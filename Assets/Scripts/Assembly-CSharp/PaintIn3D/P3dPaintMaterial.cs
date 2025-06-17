using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dPaintMaterial : ScriptableObject, P3dShaderTemplate.IHasTemplate
	{
		[Serializable]
		public class Slot
		{
			public P3dGroup Group;

			public Texture Texture;

			public Color Color = Color.white;
		}

		public enum StyleType
		{
			Seamless = 0,
			Decal = 1
		}

		[SerializeField]
		private string category;

		[SerializeField]
		private Texture2D thumbnail;

		[SerializeField]
		private StyleType style;

		[SerializeField]
		private Texture shape;

		[SerializeField]
		private Material material;

		[SerializeField]
		private P3dShaderTemplate template;

		[SerializeField]
		private List<Slot> slots;

		private static List<P3dPaintMaterial> cachedInstances = new List<P3dPaintMaterial>();

		private static bool cachedInstancesSet;

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public Texture2D Thumbnail
		{
			get
			{
				return thumbnail;
			}
			set
			{
				thumbnail = value;
			}
		}

		public StyleType Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
			}
		}

		public Texture Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
			}
		}

		public Material Material
		{
			get
			{
				return material;
			}
			set
			{
				material = value;
			}
		}

		public P3dShaderTemplate Template
		{
			get
			{
				return template;
			}
			set
			{
				template = value;
			}
		}

		public List<Slot> Slots
		{
			get
			{
				if (slots == null)
				{
					slots = new List<Slot>();
				}
				return slots;
			}
		}

		public static List<P3dPaintMaterial> CachedInstances
		{
			get
			{
				if (!cachedInstancesSet)
				{
					UpdateCachedInstances();
				}
				return cachedInstances;
			}
		}

		public void SetTemplate(P3dShaderTemplate template)
		{
			this.template = template;
		}

		public P3dShaderTemplate GetTemplate()
		{
			return template;
		}

		[ContextMenu("Clear Thumbnail")]
		public void ClearThumbnail()
		{
			UnityEngine.Object.DestroyImmediate(thumbnail);
			thumbnail = null;
		}

		public static void UpdateCachedInstances()
		{
			cachedInstancesSet = true;
			cachedInstances.Clear();
		}

		public Slot GetSlot(P3dGroup group)
		{
			foreach (Slot slot in Slots)
			{
				if ((int)slot.Group == (int)group)
				{
					return slot;
				}
			}
			return null;
		}
	}
}
