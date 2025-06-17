using UnityEngine;

namespace PaintIn3D
{
	public interface IHitPoint
	{
		void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure);
	}
}
