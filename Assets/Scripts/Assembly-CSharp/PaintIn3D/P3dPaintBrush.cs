using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dPaintBrush : ScriptableObject
	{
		[SerializeField]
		private string category;

		[SerializeField]
		private Texture shape;

		[SerializeField]
		private P3dChannel shapeChannel;

		[SerializeField]
		private Color color = Color.white;

		[SerializeField]
		private float angle;

		[SerializeField]
		private float radius = 1f;

		private static List<P3dPaintBrush> cachedInstances = new List<P3dPaintBrush>();

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

		public P3dChannel ShapeChannel
		{
			get
			{
				return shapeChannel;
			}
			set
			{
				shapeChannel = value;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public float Angle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value;
			}
		}

		public float Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = value;
			}
		}

		public static List<P3dPaintBrush> CachedInstances
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

		public static void UpdateCachedInstances()
		{
			cachedInstancesSet = true;
			cachedInstances.Clear();
		}
	}
}
