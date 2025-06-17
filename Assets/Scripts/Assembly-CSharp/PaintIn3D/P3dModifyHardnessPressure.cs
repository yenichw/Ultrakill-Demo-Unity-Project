using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyHardnessPressure")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Hardness Pressure")]
	public class P3dModifyHardnessPressure : MonoBehaviour, IModify, IModifyHardness
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private float hardness = 1f;

		[SerializeField]
		private BlendType blend;

		public float Hardness
		{
			get
			{
				return hardness;
			}
			set
			{
				hardness = value;
			}
		}

		public BlendType Blend
		{
			get
			{
				return blend;
			}
			set
			{
				blend = value;
			}
		}

		public void ModifyHardness(float pressure, ref float currentHardness)
		{
			float num = 0f;
			switch (blend)
			{
			case BlendType.Replace:
				num = hardness;
				break;
			case BlendType.Multiply:
				num = currentHardness * hardness;
				break;
			case BlendType.Increment:
				num = currentHardness + hardness;
				break;
			}
			currentHardness += (num - currentHardness) * pressure;
		}
	}
}
