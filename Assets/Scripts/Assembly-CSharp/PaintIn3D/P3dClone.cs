using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dClone : P3dLinkedBehaviour<P3dClone>, IClone
	{
		public abstract Matrix4x4 Transform(Matrix4x4 matrix);
	}
}
