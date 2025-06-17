using UnityEngine;

namespace PaintIn3D
{
	public interface IModifyColor
	{
		void ModifyColor(float pressure, ref Color color);
	}
}
