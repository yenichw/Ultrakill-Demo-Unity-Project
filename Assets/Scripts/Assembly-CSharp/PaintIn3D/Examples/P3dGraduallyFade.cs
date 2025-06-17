using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dGraduallyFade")]
	[AddComponentMenu("Paint in 3D/Examples/Gradually Fade")]
	public class P3dGraduallyFade : MonoBehaviour
	{
		[SerializeField]
		private P3dPaintableTexture paintableTexture;

		[SerializeField]
		private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		[SerializeField]
		private Texture texture;

		[SerializeField]
		private Color color = Color.white;

		[Range(0f, 1f)]
		[SerializeField]
		private float threshold = 0.1f;

		[SerializeField]
		private float speed = 1f;

		[SerializeField]
		private float counter;

		public P3dPaintableTexture PaintableTexture
		{
			get
			{
				return paintableTexture;
			}
			set
			{
				paintableTexture = value;
			}
		}

		public P3dBlendMode BlendMode
		{
			get
			{
				return blendMode;
			}
			set
			{
				blendMode = value;
			}
		}

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

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public float Threshold
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
			}
		}

		protected virtual void Update()
		{
			if (!(paintableTexture != null) || !paintableTexture.Activated)
			{
				return;
			}
			if (speed > 0f)
			{
				counter += speed * Time.deltaTime;
			}
			if (counter >= threshold)
			{
				int num = Mathf.FloorToInt(counter * 255f);
				if (num > 0)
				{
					float num2 = (float)num / 255f;
					counter -= num2;
					paintableTexture.Current = P3dPaintFill.Blit(paintableTexture.Current, blendMode, texture, color, Mathf.Min(num2, 1f), Mathf.Min(num2, 1f));
					paintableTexture.NotifyOnModified(preview: false);
				}
			}
		}
	}
}
