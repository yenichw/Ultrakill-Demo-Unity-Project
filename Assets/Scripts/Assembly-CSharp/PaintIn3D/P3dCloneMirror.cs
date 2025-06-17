using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dCloneMirror")]
	[AddComponentMenu("Paint in 3D/Transform/Clone Mirror")]
	public class P3dCloneMirror : P3dClone
	{
		[SerializeField]
		private bool flip;

		public bool Flip
		{
			get
			{
				return flip;
			}
			set
			{
				flip = value;
			}
		}

		public override Matrix4x4 Transform(Matrix4x4 matrix)
		{
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Matrix4x4 matrix4x = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			Matrix4x4 matrix4x2 = Matrix4x4.Translate(position);
			Matrix4x4 matrix4x3 = Matrix4x4.Rotate(rotation);
			Matrix4x4 matrix4x4 = Matrix4x4.Translate(-position);
			Matrix4x4 matrix4x5 = Matrix4x4.Rotate(Quaternion.Inverse(rotation));
			if (flip)
			{
				matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 0f), new Vector3(1f, 1f, -1f));
			}
			return matrix4x2 * matrix4x3 * matrix4x * matrix4x5 * matrix4x4 * matrix;
		}
	}
}
