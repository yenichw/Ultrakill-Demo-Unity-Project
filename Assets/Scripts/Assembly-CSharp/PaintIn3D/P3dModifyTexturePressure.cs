using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyTexturePressure")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Texture Pressure")]
	public class P3dModifyTexturePressure : MonoBehaviour, IModify, IModifyTexture
	{
		[SerializeField]
		private Texture texture;

		[SerializeField]
		private float pressureMin = 0.5f;

		[SerializeField]
		private float pressureMax = 1f;

		public Texture Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
			}
		}

		public float PressureMin
		{
			get
			{
				return pressureMin;
			}
			set
			{
				pressureMin = value;
			}
		}

		public float PressureMax
		{
			get
			{
				return pressureMax;
			}
			set
			{
				pressureMax = value;
			}
		}

		public void ModifyTexture(float pressure, ref Texture currentTexture)
		{
			if (pressure >= pressureMin && pressure <= pressureMax)
			{
				currentTexture = texture;
			}
		}
	}
}
