using UnityEngine;

namespace PaintIn3D
{
	public interface IHitRaycast
	{
		void HandleHitRaycast(bool preview, int priority, RaycastHit hit, float pressure);
	}
}
