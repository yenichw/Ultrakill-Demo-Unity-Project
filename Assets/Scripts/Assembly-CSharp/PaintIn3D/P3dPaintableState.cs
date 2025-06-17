using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dPaintableState
	{
		public RenderTexture Texture;

		public List<P3dCommand> Commands = new List<P3dCommand>();

		private static Stack<P3dPaintableState> pool = new Stack<P3dPaintableState>();

		public static P3dPaintableState Pop()
		{
			if (pool.Count <= 0)
			{
				return new P3dPaintableState();
			}
			return pool.Pop();
		}

		public void Write(RenderTexture current)
		{
			Clear();
			Texture = P3dHelper.GetRenderTexture(current.descriptor);
			P3dHelper.Blit(Texture, current);
		}

		public void Write(List<P3dCommand> commands)
		{
			Clear();
			Commands.AddRange(commands);
		}

		private void Clear()
		{
			if (Texture != null)
			{
				P3dHelper.ReleaseRenderTexture(Texture);
				Texture = null;
			}
			for (int num = Commands.Count - 1; num >= 0; num--)
			{
				Commands[num].Pool();
			}
			Commands.Clear();
		}

		public void Pool()
		{
			Clear();
			pool.Push(this);
		}
	}
}
