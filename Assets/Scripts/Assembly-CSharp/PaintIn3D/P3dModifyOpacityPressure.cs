using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyOpacityPressure")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Opacity Pressure")]
	public class P3dModifyOpacityPressure : MonoBehaviour, IModify, IModifyOpacity
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private float opacity = 1f;

		[SerializeField]
		private BlendType blend;

		public float Opacity
		{
			get
			{
				return opacity;
			}
			set
			{
				opacity = value;
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

		public void ModifyOpacity(float pressure, ref float currentOpacity)
		{
			float num = 0f;
			switch (blend)
			{
			case BlendType.Replace:
				num = opacity;
				break;
			case BlendType.Multiply:
				num = currentOpacity * opacity;
				break;
			case BlendType.Increment:
				num = currentOpacity + opacity;
				break;
			}
			currentOpacity += (num - currentOpacity) * pressure;
		}
	}
}
