using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintDecal")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Decal")]
	public class P3dPaintDecal : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public Vector3 Direction;

			public Color Color;

			public float Opacity;

			public float Hardness;

			public float Wrapping;

			public Texture Texture;

			public Texture Shape;

			public Vector4 ShapeChannel;

			public Vector2 NormalFront;

			public Vector2 NormalBack;

			public Texture TileTexture;

			public Matrix4x4 TileMatrix;

			public float TileBlend;

			private static Stack<Command> pool;

			private static Material[] cachedMaterials;

			public override bool RequireMesh => true;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.Direction = Direction;
				command.Color = Color;
				command.Opacity = Opacity;
				command.Hardness = Hardness;
				command.Wrapping = Wrapping;
				command.Texture = Texture;
				command.Shape = Shape;
				command.ShapeChannel = ShapeChannel;
				command.NormalFront = NormalFront;
				command.NormalBack = NormalBack;
				command.TileTexture = TileTexture;
				command.TileMatrix = TileMatrix;
				command.TileBlend = TileBlend;
				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetVector(P3dShader._Direction, Direction);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetFloat(P3dShader._Wrapping, Wrapping);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetTexture(P3dShader._Shape, Shape);
				Material.SetVector(P3dShader._ShapeChannel, ShapeChannel);
				Material.SetVector(P3dShader._NormalFront, NormalFront);
				Material.SetVector(P3dShader._NormalBack, NormalBack);
				Material.SetTexture(P3dShader._TileTexture, TileTexture);
				Material.SetMatrix(P3dShader._TileMatrix, TileMatrix);
				Material.SetFloat(P3dShader._TileBlend, TileBlend);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetLocation(Vector3 worldPosition, Quaternion worldRotation, Vector3 scale, float radius, float aspect)
			{
				Vector3 vector = scale * radius;
				if (aspect > 1f)
				{
					vector.y /= aspect;
				}
				else
				{
					vector.x *= aspect;
				}
				Matrix = Matrix4x4.TRS(worldPosition, worldRotation, vector);
				Position = worldPosition;
				Radius = (vector * 0.5f).magnitude;
				Direction = worldRotation * Vector3.forward;
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture texture, Texture shape, P3dChannel shapeChannel, float hardness, float wrapping, float normalBack, float normalFront, float normalFade, Color color, float opacity, Texture tileTexture, Matrix4x4 tileMatrix, float tileBlend)
			{
				Blend = blendMode;
				Material = cachedMaterials[(int)blendMode];
				Color = color;
				Opacity = opacity;
				Hardness = hardness;
				Wrapping = wrapping;
				Texture = texture;
				Shape = shape;
				ShapeChannel = P3dHelper.IndexToVector((int)shapeChannel);
				TileTexture = tileTexture;
				TileMatrix = tileMatrix;
				TileBlend = tileBlend;
				float num = normalFront - 1f - normalFade;
				float num2 = normalFront - 1f;
				float num3 = 1f - normalBack + normalFade;
				float num4 = 1f - normalBack;
				NormalFront = new Vector2(num, P3dHelper.Reciprocal(num2 - num));
				NormalBack = new Vector2(num3, P3dHelper.Reciprocal(num4 - num3));
			}

			public override void SetLocation(Matrix4x4 matrix)
			{
				base.SetLocation(matrix);
				Direction = matrix.MultiplyVector(Vector3.forward).normalized;
			}
		}

		[SerializeField]
		private LayerMask layers = -1;

		[SerializeField]
		private P3dModel targetModel;

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private P3dPaintableTexture targetTexture;

		[SerializeField]
		private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		[SerializeField]
		private Texture texture;

		[SerializeField]
		private Texture shape;

		[SerializeField]
		private P3dChannel shapeChannel = P3dChannel.Alpha;

		[SerializeField]
		private Color color = Color.white;

		[Range(0f, 1f)]
		[SerializeField]
		private float opacity = 1f;

		[Range(-180f, 180f)]
		[SerializeField]
		private float angle;

		[SerializeField]
		private Vector3 scale = Vector3.one;

		[SerializeField]
		private float radius = 0.1f;

		[SerializeField]
		private float hardness = 3f;

		[SerializeField]
		[Range(0f, 1f)]
		private float wrapping = 1f;

		[Range(0f, 2f)]
		[SerializeField]
		private float normalFront = 0.2f;

		[Range(0f, 2f)]
		[SerializeField]
		private float normalBack;

		[Range(0.001f, 0.5f)]
		[SerializeField]
		private float normalFade = 0.01f;

		[SerializeField]
		private Texture tileTexture;

		[SerializeField]
		private Transform tileTransform;

		[Range(1f, 10f)]
		[SerializeField]
		private float tileBlend = 3f;

		public LayerMask Layers
		{
			get
			{
				return layers;
			}
			set
			{
				layers = value;
			}
		}

		public P3dModel TargetModel
		{
			get
			{
				return targetModel;
			}
			set
			{
				targetModel = value;
			}
		}

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

		public P3dPaintableTexture TargetTexture
		{
			get
			{
				return targetTexture;
			}
			set
			{
				targetTexture = value;
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

		public P3dChannel ShapeChannel
		{
			get
			{
				return shapeChannel;
			}
			set
			{
				shapeChannel = value;
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

		public Vector3 Scale
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

		public float Hardness
		{
			get
			{
				return hardness;
			}
			set
			{
				hardness = value;
			}
		}

		public float Wrapping
		{
			get
			{
				return wrapping;
			}
			set
			{
				wrapping = value;
			}
		}

		public float NormalFront
		{
			get
			{
				return normalFront;
			}
			set
			{
				normalFront = value;
			}
		}

		public float NormalBack
		{
			get
			{
				return normalBack;
			}
			set
			{
				normalBack = value;
			}
		}

		public float NormalFade
		{
			get
			{
				return normalFade;
			}
			set
			{
				normalFade = value;
			}
		}

		public Texture TileTexture
		{
			get
			{
				return tileTexture;
			}
			set
			{
				tileTexture = value;
			}
		}

		public Transform TileTransform
		{
			get
			{
				return tileTransform;
			}
			set
			{
				tileTransform = value;
			}
		}

		public float TileBlend
		{
			get
			{
				return tileBlend;
			}
			set
			{
				tileBlend = value;
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

		public void MultiplyHardness(float multiplier)
		{
			hardness *= multiplier;
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			float num = opacity;
			float num2 = radius;
			float num3 = hardness;
			Color color = this.color;
			float z = angle;
			Texture texture = this.texture;
			Matrix4x4 tileMatrix = ((tileTransform != null) ? tileTransform.localToWorldMatrix : Matrix4x4.identity);
			float aspect = P3dHelper.GetAspect(shape, this.texture);
			P3dPaintableManager.BuildModifiers(base.gameObject);
			P3dPaintableManager.ModifyColor(pressure, ref color);
			P3dPaintableManager.ModifyAngle(pressure, ref z);
			P3dPaintableManager.ModifyOpacity(pressure, ref num);
			P3dPaintableManager.ModifyHardness(pressure, ref num3);
			P3dPaintableManager.ModifyRadius(pressure, ref num2);
			P3dPaintableManager.ModifyTexture(pressure, ref texture);
			worldRotation *= Quaternion.Euler(0f, 0f, z);
			Command.Instance.SetLocation(worldPosition, worldRotation, scale, num2, aspect);
			Command.Instance.SetMaterial(blendMode, this.texture, shape, shapeChannel, num3, wrapping, normalBack, normalFront, normalFade, color, num, tileTexture, tileMatrix, tileBlend);
			P3dPaintableManager.SubmitAll(Command.Instance, preview, priority, layers, group, targetModel, targetTexture);
		}
	}
}
