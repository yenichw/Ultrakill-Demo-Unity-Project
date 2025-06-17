using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintSphereBlur")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Sphere Blur")]
	public class P3dPaintSphereBlur : MonoBehaviour, IHit, IHitPoint, IHitLine
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public float Opacity;

			public float Hardness;

			public float Squash;

			public float KernelSize;

			private static Stack<Command> pool;

			private static Material cachedMaterial;

			private static P3dBlendMode cachedBlend;

			public override bool RequireMesh => true;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedBlend = new P3dBlendMode
				{
					Index = 0
				};
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Sphere Blur");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.Opacity = Opacity;
				command.Hardness = Hardness;
				command.Squash = Squash;
				command.KernelSize = KernelSize;
				return command;
			}

			public override void Apply()
			{
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetFloat(P3dShader._Squash, Squash);
				Material.SetFloat(P3dShader._KernelSize, KernelSize);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetLocation(Vector3 positionA, Vector3 positionB, float radius)
			{
				Vector3 forward = positionB - positionA;
				if (forward.sqrMagnitude > 0f && radius > 0f)
				{
					Vector3 position = (positionA + positionB) * 0.5f;
					float num = forward.magnitude * 0.5f;
					float num2 = num + radius;
					Quaternion rotation = ((num > radius * 0.01f) ? Quaternion.LookRotation(forward) : Quaternion.identity);
					SetLocation(position, new Vector3(radius, radius, num2), rotation);
					Squash = num2 / radius;
				}
				else
				{
					SetLocation(positionA, radius);
				}
			}

			public void SetLocation(Vector3 position, float radius)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m00 = (identity.m11 = (identity.m22 = radius));
				identity.m03 = position.x;
				identity.m13 = position.y;
				identity.m23 = position.z;
				Matrix = identity;
				Squash = 1f;
				Position = position;
				Radius = radius;
			}

			public void SetLocation(Vector3 position, Vector3 radius, Quaternion rotation)
			{
				Matrix = Matrix4x4.TRS(position, rotation, radius);
				Squash = 1f;
				Position = position;
				Radius = Mathf.Max(radius.x, Mathf.Max(radius.y, radius.z));
			}

			public void SetMaterial(float hardness, float opacity, float kernelSize)
			{
				Blend = cachedBlend;
				Material = cachedMaterial;
				Hardness = hardness;
				Opacity = opacity;
				KernelSize = kernelSize;
			}
		}

		public enum RotationType
		{
			World = 0,
			Normal = 1
		}

		[SerializeField]
		private LayerMask layers = -1;

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private P3dModel targetModel;

		[SerializeField]
		private P3dPaintableTexture targetTexture;

		[SerializeField]
		private Vector3 scale = Vector3.one;

		[SerializeField]
		private RotationType rotateTo;

		[SerializeField]
		private float radius = 0.1f;

		[SerializeField]
		private float radiusPressure;

		[SerializeField]
		private float hardness = 1f;

		[SerializeField]
		private float hardnessPressure;

		[Range(0f, 1f)]
		[SerializeField]
		private float opacity = 1f;

		[SerializeField]
		private float opacityPressure;

		[SerializeField]
		private float kernelSize = 0.001f;

		[SerializeField]
		private float kernelSizePressure;

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

		public RotationType RotateTo
		{
			get
			{
				return rotateTo;
			}
			set
			{
				rotateTo = value;
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

		public float HardnessPressure
		{
			get
			{
				return hardnessPressure;
			}
			set
			{
				hardnessPressure = value;
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

		public float KernelSize
		{
			get
			{
				return kernelSize;
			}
			set
			{
				kernelSize = value;
			}
		}

		public float KernelSizePressure
		{
			get
			{
				return kernelSizePressure;
			}
			set
			{
				kernelSizePressure = value;
			}
		}

		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		public void MultiplyRadius(float multiplier)
		{
			radius *= multiplier;
		}

		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			float num = opacity + (1f - opacity) * opacityPressure * pressure;
			float num2 = radius + radiusPressure * pressure;
			float num3 = hardness + hardnessPressure * pressure;
			P3dPaintableManager.BuildModifiers(base.gameObject);
			P3dPaintableManager.ModifyOpacity(pressure, ref num);
			P3dPaintableManager.ModifyRadius(pressure, ref num2);
			P3dPaintableManager.ModifyHardness(pressure, ref num3);
			if (scale == Vector3.one)
			{
				Command.Instance.SetLocation(worldPosition, num2);
			}
			else
			{
				Command.Instance.SetLocation(worldPosition, scale * num2, (rotateTo == RotationType.World) ? Quaternion.identity : worldRotation);
			}
			Command.Instance.SetMaterial(num3, num, kernelSize);
			P3dPaintableManager.SubmitAll(Command.Instance, preview, priority, layers, group, targetModel, targetTexture);
		}

		public void HandleHitLine(bool preview, int priority, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			float num = opacity + (1f - opacity) * opacityPressure * pressureA;
			float num2 = radius + radiusPressure * pressureA;
			float num3 = hardness + hardnessPressure * pressureA;
			Command.Instance.SetLocation(worldPositionA, worldPositionB, num2);
			Command.Instance.SetMaterial(num3, num, kernelSize);
			P3dPaintableManager.SubmitAll(Command.Instance, preview, priority, layers, group, targetModel, targetTexture);
		}
	}
}
