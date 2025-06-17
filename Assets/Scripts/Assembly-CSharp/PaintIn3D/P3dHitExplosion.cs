using System;
using System.Collections;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitExplosion")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Explosion")]
	public class P3dHitExplosion : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp = 0,
			CameraUp = 1
		}

		public enum NormalType
		{
			HitNormal = 0,
			RayDirection = 1
		}

		[SerializeField]
		private float radius = 1f;

		[SerializeField]
		private int count = 20;

		[SerializeField]
		private float delayMax = 0.25f;

		[SerializeField]
		private LayerMask layers = -5;

		[SerializeField]
		private OrientationType orientation;

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private NormalType normal;

		[SerializeField]
		private float offset;

		[SerializeField]
		private bool preview;

		[SerializeField]
		private int priority;

		[NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

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

		public float DelayMax
		{
			get
			{
				return delayMax;
			}
			set
			{
				delayMax = value;
			}
		}

		public LayerMask Layers
		{
			get
			{
				return layers;
			}
			set
			{
				layers = value;
			}
		}

		public OrientationType Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
			}
		}

		public Camera Camera
		{
			get
			{
				return _camera;
			}
			set
			{
				_camera = value;
			}
		}

		public NormalType Normal
		{
			get
			{
				return normal;
			}
			set
			{
				normal = value;
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

		public bool Preview
		{
			get
			{
				return preview;
			}
			set
			{
				preview = value;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
			}
		}

		public P3dHitCache HitCache => hitCache;

		[ContextMenu("Clear Hit Cache")]
		public void ClearHitCache()
		{
			hitCache.Clear();
		}

		protected virtual void Start()
		{
			ExplodeNow();
		}

		[ContextMenu("Explode Now")]
		public void ExplodeNow()
		{
			for (int i = 0; i < Count; i++)
			{
				Vector3 position = base.transform.position;
				Vector3 pointB = position + UnityEngine.Random.onUnitSphere * radius;
				StartCoroutine(DelayedOnHit(position, pointB));
			}
		}

		private IEnumerator DelayedOnHit(Vector3 pointA, Vector3 pointB)
		{
			Vector3 direction = pointB - pointA;
			float magnitude = direction.magnitude;
			Ray ray = new Ray(pointA, direction);
			RaycastHit hit = default(RaycastHit);
			if (Physics.Raycast(ray, out hit, magnitude, layers))
			{
				float distance = Mathf.InverseLerp(0f, radius, hit.distance);
				yield return new WaitForSeconds(distance * delayMax);
				Vector3 upwards = ((orientation == OrientationType.CameraUp) ? P3dHelper.GetCameraUp(_camera) : Vector3.up);
				Vector3 worldPosition = hit.point + hit.normal * offset;
				Quaternion worldRotation = Quaternion.LookRotation((normal == NormalType.HitNormal) ? (-hit.normal) : ray.direction, upwards);
				hitCache.InvokePoints(base.gameObject, preview, priority, hit.collider, worldPosition, worldRotation, 1f - distance);
			}
		}
	}
}
