using UnityEngine;

namespace PaintIn3D
{
	public interface IClone
	{
		Matrix4x4 Transform(Matrix4x4 matrix);
	}
}
