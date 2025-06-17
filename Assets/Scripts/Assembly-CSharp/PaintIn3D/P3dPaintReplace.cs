using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintReplace")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Replace")]
	public class P3dPaintReplace : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public Texture Texture;

			public Color Color;

			private static Stack<Command> pool;

			private static Material cachedMaterial;

			public override bool RequireMesh => false;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Replace");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.Texture = Texture;
				command.Color = Color;
				return command;
			}

			public override void Apply()
			{
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetMaterial(Texture texture, Color color)
			{
				Blend = P3dBlendMode.Replace;
				Material = cachedMaterial;
				Texture = texture;
				Color = color;
				Radius = float.PositiveInfinity;
			}
		}

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private Texture texture;

		[SerializeField]
		private Color color = Color.white;

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

		public static void Blit(RenderTexture renderTexture, Texture texture, Color tint)
		{
			Command.Instance.SetMaterial(texture, tint);
			Command.Instance.Apply();
			P3dHelper.Blit(renderTexture, Command.Instance.Material);
		}

		public static void BlitFast(RenderTexture renderTexture, Texture texture, Color tint)
		{
			Command.Instance.SetMaterial(texture, tint);
			Command.Instance.Apply();
			Graphics.Blit(null, renderTexture, Command.Instance.Material);
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
				Texture texture = this.texture;
				P3dPaintableManager.BuildModifiers(base.gameObject);
				P3dPaintableManager.ModifyColor(pressure, ref color);
				P3dPaintableManager.ModifyTexture(pressure, ref texture);
				Command.Instance.SetMaterial(texture, color);
				for (int num = list.Count - 1; num >= 0; num--)
				{
					P3dPaintableTexture paintableTexture = list[num];
					P3dPaintableManager.Submit(Command.Instance, component, paintableTexture, preview, priority);
				}
			}
		}
	}
}
