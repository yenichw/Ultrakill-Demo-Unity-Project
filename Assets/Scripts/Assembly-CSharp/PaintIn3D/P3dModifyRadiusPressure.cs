using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyRadiusPressure")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Radius Pressure")]
	public class P3dModifyRadiusPressure : MonoBehaviour, IModify, IModifyRadius
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private float radius = 1f;

		[SerializeField]
		private BlendType blend;

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

		public void ModifyRadius(float pressure, ref float currentRadius)
		{
			float num = 0f;
			switch (blend)
			{
			case BlendType.Replace:
				num = radius;
				break;
			case BlendType.Multiply:
				num = currentRadius * radius;
				break;
			case BlendType.Increment:
				num = currentRadius + radius;
				break;
			}
			currentRadius += (num - currentRadius) * pressure;
		}
	}
}
