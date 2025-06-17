using UnityEngine;

namespace PaintIn3D
{
	public interface IHitLine
	{
		void HandleHitLine(bool preview, int priority, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB);
	}
}
