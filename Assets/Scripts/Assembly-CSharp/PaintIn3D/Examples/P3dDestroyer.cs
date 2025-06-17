using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dDestroyer")]
	[AddComponentMenu("Paint in 3D/Examples/Destroyer")]
	public class P3dDestroyer : MonoBehaviour, IHit, IHitPoint
	{
		[SerializeField]
		private GameObject target;

		public GameObject Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		[ContextMenu("Destroy Now")]
		public void DestroyNow()
		{
			Object.Destroy(base.gameObject);
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			DestroyNow();
		}
	}
}
