using System;
using UnityEngine;

namespace PaintIn3D
{
	[Serializable]
	public struct P3dSlot
	{
		public int Index;

		public string Name;

		public P3dSlot(int newIndex, string newName)
		{
			Index = newIndex;
			Name = newName;
		}

		public Material FindMaterial(GameObject gameObject)
		{
			return P3dHelper.GetMaterial(gameObject, Index);
		}

		public Texture FindTexture(GameObject gameObject)
		{
			Material material = P3dHelper.GetMaterial(gameObject, Index);
			if (material != null)
			{
				return material.GetTexture(Name);
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Index.GetHashCode() ^ Name.GetHashCode();
		}

		public static bool operator ==(P3dSlot a, P3dSlot b)
		{
			if (a.Index == b.Index)
			{
				return a.Name == b.Name;
			}
			return false;
		}

		public static bool operator !=(P3dSlot a, P3dSlot b)
		{
			if (a.Index == b.Index)
			{
				return a.Name != b.Name;
			}
			return true;
		}
	}
}
