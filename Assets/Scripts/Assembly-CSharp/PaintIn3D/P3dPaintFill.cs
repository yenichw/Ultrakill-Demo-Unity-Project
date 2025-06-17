using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintFill")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Fill")]
	public class P3dPaintFill : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public Texture Texture;

			public Color Color;

			public float Opacity;

			public float Minimum;

			private static Stack<Command> pool;

			private static Material[] cachedMaterials;

			public override bool RequireMesh => false;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Fill");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.Texture = Texture;
				command.Color = Color;
				command.Opacity = Opacity;
				command.Minimum = Minimum;
				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetVector(P3dShader._Minimum, new Vector4(Minimum, Minimum, Minimum, Minimum));
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
			{
				Material = cachedMaterials[(int)blendMode];
				Texture = texture;
				Color = color;
				Opacity = opacity;
				Minimum = minimum;
			}
		}

		public enum RotationType
		{
			World = 0,
			Normal = 1
		}

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		[SerializeField]
		private Texture texture;

		[SerializeField]
		private Color color = Color.white;

		[Range(0f, 1f)]
		[SerializeField]
		private float opacity = 1f;

		[SerializeField]
		private float opacityPressure;

		[Range(0f, 1f)]
		[SerializeField]
		private float minimum;

		public P3dGroup Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
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

		public float OpacityPressure
		{
			get
			{
				return opacityPressure;
			}
			set
			{
				opacityPressure = value;
			}
		}

		public float Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = value;
			}
		}

		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		public static RenderTexture Blit(RenderTexture main, P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
		{
			RenderTexture swap = P3dHelper.GetRenderTexture(main.descriptor);
			Blit(ref main, ref swap, blendMode, texture, color, opacity, minimum);
			P3dHelper.ReleaseRenderTexture(swap);
			return main;
		}

		public static void Blit(ref RenderTexture main, ref RenderTexture swap, P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
		{
			Command.Instance.SetMaterial(blendMode, texture, color, opacity, minimum);
			Command.Instance.Apply(main);
			P3dHelper.Blit(swap, Command.Instance.Material);
			P3dHelper.Swap(ref main, ref swap);
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (!(collider != null))
			{
				return;
			}
			P3dModel component = collider.GetComponent<P3dModel>();
			if (!(component != null))
			{
				return;
			}
			List<P3dPaintableTexture> list = P3dPaintableTexture.Filter(component, group);
			if (list.Count > 0)
			{
				Color color = this.color;
				float num = opacity;
				Texture texture = this.texture;
				P3dPaintableManager.BuildModifiers(base.gameObject);
				P3dPaintableManager.ModifyColor(pressure, ref color);
				P3dPaintableManager.ModifyOpacity(pressure, ref num);
				P3dPaintableManager.ModifyTexture(pressure, ref texture);
				Command.Instance.SetMaterial(blendMode, texture, color, opacity, minimum);
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					P3dPaintableTexture paintableTexture = list[num2];
					P3dPaintableManager.Submit(Command.Instance, component, paintableTexture, preview, priority);
				}
			}
		}
	}
}
