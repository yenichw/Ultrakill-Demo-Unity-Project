using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dGroupData : ScriptableObject
	{
		[SerializeField]
		private int index;

		[SerializeField]
		private Color defaultColor;

		[SerializeField]
		private Texture defaultTexture;

		private static List<P3dGroupData> cachedInstances = new List<P3dGroupData>();

		private static bool cachedInstancesSet;

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		public Color DefaultColor
		{
			get
			{
				return defaultColor;
			}
			set
			{
				defaultColor = value;
			}
		}

		public Texture DefaultTexture
		{
			get
			{
				return defaultTexture;
			}
			set
			{
				defaultTexture = value;
			}
		}

		public static List<P3dGroupData> CachedInstances
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

		public string GetName(bool prefixNumber)
		{
			if (prefixNumber)
			{
				return index + ": " + base.name;
			}
			return base.name;
		}

		public static void UpdateCachedInstances()
		{
			cachedInstancesSet = true;
		}

		public static string GetGroupName(int index, bool prefixNumber)
		{
			P3dGroupData groupData = GetGroupData(index);
			if (!(groupData != null))
			{
				return null;
			}
			return groupData.GetName(prefixNumber);
		}

		public static P3dGroupData GetGroupData(int index)
		{
			foreach (P3dGroupData cachedInstance in CachedInstances)
			{
				if (cachedInstance != null && cachedInstance.index == index)
				{
					return cachedInstance;
				}
			}
			return null;
		}
	}
}
