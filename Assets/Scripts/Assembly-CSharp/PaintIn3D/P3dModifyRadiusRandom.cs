using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyRadiusRandom")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Radius Random")]
	public class P3dModifyRadiusRandom : MonoBehaviour, IModify, IModifyRadius
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

		public void ModifyRadius(float pressure, ref float radius)
		{
			float num = Random.Range(min, max);
			switch (blend)
			{
			case BlendType.Replace:
				radius = num;
				break;
			case BlendType.Multiply:
				radius *= num;
				break;
			case BlendType.Increment:
				radius += num;
				break;
			}
		}
	}
}
