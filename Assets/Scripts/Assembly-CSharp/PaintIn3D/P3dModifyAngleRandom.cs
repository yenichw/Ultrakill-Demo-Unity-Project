using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyAngleRandom")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Angle Random")]
	public class P3dModifyAngleRandom : MonoBehaviour, IModify, IModifyAngle
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private float min = -180f;

		[SerializeField]
		private float max = 180f;

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

		public void ModifyAngle(float pressure, ref float angle)
		{
			float num = Random.Range(min, max);
			switch (blend)
			{
			case BlendType.Replace:
				angle = num;
				break;
			case BlendType.Multiply:
				angle *= num;
				break;
			case BlendType.Increment:
				angle += num;
				break;
			}
		}
	}
}
