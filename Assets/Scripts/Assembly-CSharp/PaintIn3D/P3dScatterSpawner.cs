using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dScatterSpawner")]
	[AddComponentMenu("Paint in 3D/Scatter Spawner")]
	public class P3dScatterSpawner : MonoBehaviour, IHit, IHitPoint
	{
		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private float offset;

		[SerializeField]
		private float radius = 0.5f;

		[SerializeField]
		private int count = 10;

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

		public float Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = value;
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (prefab != null)
			{
				Vector3 vector = worldPosition + worldRotation * Vector3.forward * offset;
				for (int i = 0; i < count; i++)
				{
					Vector3 vector2 = worldRotation * Random.insideUnitCircle * radius;
					Object.Instantiate(prefab, vector + vector2, worldRotation, null);
				}
			}
		}
	}
}
