using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyHardnessRandom")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Hardness Random")]
	public class P3dModifyHardnessRandom : MonoBehaviour, IModify, IModifyHardness
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private float min = 0.6666f;

		[SerializeField]
		private float max = 1.5f;

		[SerializeField]
		private BlendType blend;

		public float Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
			}
		}

		public float Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
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

		public void ModifyHardness(float pressure, ref float hardness)
		{
			float num = Random.Range(min, max);
			switch (blend)
			{
			case BlendType.Replace:
				hardness = num;
				break;
			case BlendType.Multiply:
				hardness *= num;
				break;
			case BlendType.Increment:
				hardness += num;
				break;
			}
		}
	}
}
