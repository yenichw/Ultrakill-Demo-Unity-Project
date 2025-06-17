using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintDirectDecal")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Direct Decal")]
	public class P3dPaintDirectDecal : MonoBehaviour, IHit, IHitRaycast
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public Color Color;

			public float Opacity;

			public Texture Texture;

			public Texture Shape;

			private static Stack<Command> pool;

			private static Material[] cachedMaterials;

			public override bool RequireMesh => false;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Direct Decal");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.Color = Color;
				command.Opacity = Opacity;
				command.Texture = Texture;
				command.Shape = Shape;
				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetTexture(P3dShader._Shape, Shape);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetLocation(Texture canvas, Vector2 coord, float angle, Vector2 scale, float radius, Texture decal)
			{
				Vector3 vector = new Vector3(scale.x * radius, scale.y * radius, 1f);
				if (decal != null)
				{
					if (decal.width > decal.height)
					{
						vector.y *= (float)decal.height / (float)decal.width;
					}
					else
					{
						vector.x *= (float)decal.width / (float)decal.height;
					}
				}
				Matrix = Matrix4x4.TRS(coord, Quaternion.Euler(0f, 0f, angle), new Vector3(vector.x, vector.y, 1f));
				if (canvas != null)
				{
					Matrix4x4 matrix4x = Matrix4x4.Scale(new Vector3(1f / (float)canvas.width, 1f / (float)canvas.height, 1f));
					coord.x *= canvas.width;
					coord.y *= canvas.height;
					Matrix = Matrix4x4.TRS(coord, Quaternion.Euler(0f, 0f, angle), new Vector3(vector.x, vector.y, 1f));
					Matrix = matrix4x * Matrix;
				}
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture decal, Color color, float opacity, Texture shape)
			{
				Blend = blendMode;
				Material = cachedMaterials[(int)blendMode];
				Color = color;
				Opacity = opacity;
				Texture = decal;
				Shape = shape;
			}
		}

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		[SerializeField]
		private Texture texture;

		[SerializeField]
		private Texture shape;

		[SerializeField]
		private Color color = Color.white;

		[Range(0f, 1f)]
		[SerializeField]
		private float opacity = 1f;

		[SerializeField]
		private float opacityPressure;

		[Range(-180f, 180f)]
		[SerializeField]
		private float angle;

		[SerializeField]
		private Vector2 scale = Vector2.one;

		[SerializeField]
		private float radius = 32f;

		[SerializeField]
		private float radiusPressure;

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

		public Texture Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
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

		public float Angle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value;
			}
		}

		public Vector2 Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

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

		public float RadiusPressure
		{
			get
			{
				return radiusPressure;
			}
			set
			{
				radiusPressure = value;
			}
		}

		[ContextMenu("Flip Horizontal")]
		public void FlipHorizontal()
		{
			scale.x = 0f - scale.x;
		}

		[ContextMenu("Flip Vertical")]
		public void FlipVertical()
		{
			scale.y = 0f - scale.y;
		}

		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		public void IncrementAngle(float degrees)
		{
			angle = Mathf.Repeat(angle + 180f + degrees, 360f) - 180f;
		}

		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		public void HandleHitRaycast(bool preview, int priority, RaycastHit hit, float pressure)
		{
			P3dModel component = hit.collider.GetComponent<P3dModel>();
			if (!(component != null))
			{
				return;
			}
			List<P3dPaintableTexture> list = P3dPaintableTexture.Filter(component, group);
			float num = opacity + (1f - opacity) * opacityPressure * pressure;
			float num2 = radius + radiusPressure * pressure;
			Color color = this.color;
			float num3 = angle;
			Texture decal = texture;
			P3dPaintableManager.BuildModifiers(base.gameObject);
			P3dPaintableManager.ModifyColor(pressure, ref color);
			P3dPaintableManager.ModifyAngle(pressure, ref num3);
			P3dPaintableManager.ModifyOpacity(pressure, ref num);
			P3dPaintableManager.ModifyRadius(pressure, ref num2);
			P3dPaintableManager.ModifyTexture(pressure, ref decal);
			Command.Instance.SetMaterial(blendMode, decal, color, num, shape);
			for (int num4 = list.Count - 1; num4 >= 0; num4--)
			{
				P3dPaintableTexture p3dPaintableTexture = list[num4];
				Vector2 coord = default(Vector2);
				switch (p3dPaintableTexture.Channel)
				{
				case P3dCoord.First:
					coord = hit.textureCoord;
					break;
				case P3dCoord.Second:
					coord = hit.textureCoord2;
					break;
				}
				Command.Instance.SetLocation(p3dPaintableTexture.Current, coord, num3, scale, num2, texture);
				P3dPaintableManager.Submit(Command.Instance, component, p3dPaintableTexture, preview, priority);
			}
		}
	}
}
