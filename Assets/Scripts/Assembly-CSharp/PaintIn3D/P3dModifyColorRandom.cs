using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyColorRandom")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Color Random")]
	public class P3dModifyColorRandom : MonoBehaviour, IModify, IModifyColor
	{
		public enum BlendType
		{
			Replace = 0,
			Multiply = 1,
			Increment = 2
		}

		[SerializeField]
		private Gradient gradient;

		[SerializeField]
		private BlendType blend;

		public Gradient Gradient
		{
			get
			{
				if (gradient == null)
				{
					gradient = new Gradient();
				}
				return gradient;
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

		public void ModifyColor(float pressure, ref Color color)
		{
			if (gradient != null)
			{
				Color color2 = gradient.Evaluate(Random.value);
				switch (blend)
				{
				case BlendType.Replace:
					color = color2;
					break;
				case BlendType.Multiply:
					color *= color2;
					break;
				case BlendType.Increment:
					color += color2;
					break;
				}
			}
		}
	}
}
