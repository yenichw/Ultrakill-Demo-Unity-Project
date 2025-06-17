using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dSpawner")]
	[AddComponentMenu("Paint in 3D/Examples/Spawner")]
	public class P3dSpawner : MonoBehaviour, IHit, IHitPoint
	{
		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private float offset;

		public GameObject Prefab
		{
			get
			{
				return prefab;
			}
			set
			{
				prefab = value;
			}
		}

		public float Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		public void Spawn()
		{
			Spawn(base.transform.position, base.transform.rotation);
		}

		public void Spawn(Vector3 position, Vector3 normal)
		{
			Spawn(position, Quaternion.LookRotation(normal));
		}

		public void Spawn(Vector3 position, Quaternion rotation)
		{
			if (prefab != null)
			{
				Object.Instantiate(prefab, position, base.transform.rotation, null).SetActive(value: true);
			}
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			Spawn(worldPosition + worldRotation * Vector3.forward * offset, worldRotation);
		}
	}
}
