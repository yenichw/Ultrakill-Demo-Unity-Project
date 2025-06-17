using UnityEngine;

namespace PaintIn3D
{
	public interface IModifyTexture
	{
		void ModifyTexture(float pressure, ref Texture texture);
	}
}
