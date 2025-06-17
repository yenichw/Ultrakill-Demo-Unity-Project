using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintReplaceChannels")]
	[AddComponentMenu("Paint in 3D/Paint/Paint Replace Channels")]
	public class P3dPaintReplaceChannels : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance;

			public Texture TextureR;

			public Texture TextureG;

			public Texture TextureB;

			public Texture TextureA;

			public Vector4 ChannelR;

			public Vector4 ChannelG;

			public Vector4 ChannelB;

			public Vector4 ChannelA;

			private static Stack<Command> pool;

			private static Material cachedMaterial;

			public override bool RequireMesh => false;

			static Command()
			{
				Instance = new Command();
				pool = new Stack<Command>();
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Replace Channels");
			}

			public override P3dCommand SpawnCopy()
			{
				Command command = SpawnCopy(pool);
				command.TextureR = TextureR;
				command.TextureG = TextureG;
				command.TextureB = TextureB;
				command.TextureA = TextureA;
				command.ChannelR = ChannelR;
				command.ChannelG = ChannelG;
				command.ChannelB = ChannelB;
				command.ChannelA = ChannelA;
				return command;
			}

			public override void Apply()
			{
				Material.SetTexture(P3dShader._TextureR, TextureR);
				Material.SetTexture(P3dShader._TextureG, TextureG);
				Material.SetTexture(P3dShader._TextureB, TextureB);
				Material.SetTexture(P3dShader._TextureA, TextureA);
				Material.SetColor(P3dShader._ChannelR, ChannelR);
				Material.SetColor(P3dShader._ChannelG, ChannelG);
				Material.SetColor(P3dShader._ChannelB, ChannelB);
				Material.SetColor(P3dShader._ChannelA, ChannelA);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetMaterial(Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
			{
				Blend = P3dBlendMode.Replace;
				Material = cachedMaterial;
				TextureR = textureR;
				TextureG = textureG;
				TextureB = textureB;
				TextureA = textureA;
				ChannelR = channelR;
				ChannelG = channelG;
				ChannelB = channelB;
				ChannelA = channelA;
				Radius = float.PositiveInfinity;
			}
		}

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private Texture textureR;

		[SerializeField]
		private Texture textureG;

		[SerializeField]
		private Texture textureB;

		[SerializeField]
		private Texture textureA;

		[SerializeField]
		private Vector4 channelR = new Vector4(1f, 0f, 0f, 0f);

		[SerializeField]
		private Vector4 channelG = new Vector4(1f, 0f, 0f, 0f);

		[SerializeField]
		private Vector4 channelB = new Vector4(1f, 0f, 0f, 0f);

		[SerializeField]
		private Vector4 channelA = new Vector4(1f, 0f, 0f, 0f);

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

		public Texture TextureR
		{
			get
			{
				return textureR;
			}
			set
			{
				textureR = value;
			}
		}

		public Texture TextureG
		{
			get
			{
				return textureG;
			}
			set
			{
				textureG = value;
			}
		}

		public Texture TextureB
		{
			get
			{
				return textureB;
			}
			set
			{
				textureB = value;
			}
		}

		public Texture TextureA
		{
			get
			{
				return textureA;
			}
			set
			{
				textureA = value;
			}
		}

		public Vector4 ChannelR
		{
			get
			{
				return channelR;
			}
			set
			{
				channelR = value;
			}
		}

		public Vector4 ChannelG
		{
			get
			{
				return channelG;
			}
			set
			{
				channelR = value;
			}
		}

		public Vector4 ChannelB
		{
			get
			{
				return channelB;
			}
			set
			{
				channelR = value;
			}
		}

		public Vector4 ChannelA
		{
			get
			{
				return channelA;
			}
			set
			{
				channelR = value;
			}
		}

		public static void Blit(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
		{
			Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);
			Command.Instance.Apply();
			P3dHelper.Blit(renderTexture, Command.Instance.Material);
		}

		public static void BlitFast(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
		{
			Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);
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
				Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);
				for (int num = list.Count - 1; num >= 0; num--)
				{
					P3dPaintableTexture paintableTexture = list[num];
					P3dPaintableManager.Submit(Command.Instance, component, paintableTexture, preview, priority);
				}
			}
		}
	}
}
