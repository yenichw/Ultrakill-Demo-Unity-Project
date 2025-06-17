using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitCollisions")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Collisions")]
	public class P3dHitCollisions : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp = 0,
			CameraUp = 1
		}

		[SerializeField]
		private OrientationType orientation;

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private float offset;

		[FormerlySerializedAs("threshold")]
		[SerializeField]
		private float speedMin = 1f;

		[SerializeField]
		private float speedPressure = 100f;

		[SerializeField]
		private bool onlyUseFirstContact = true;

		[SerializeField]
		private float delay;

		[SerializeField]
		private float raycastDistance = 0.0001f;

		[SerializeField]
		private int priority;

		[SerializeField]
		private float cooldown;

		[NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

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

		public float SpeedMin
		{
			get
			{
				return speedMin;
			}
			set
			{
				speedMin = value;
			}
		}

		public float SpeedPressure
		{
			get
			{
				return speedPressure;
			}
			set
			{
				speedPressure = value;
			}
		}

		public bool OnlyUseFirstContact
		{
			get
			{
				return onlyUseFirstContact;
			}
			set
			{
				onlyUseFirstContact = value;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			set
			{
				delay = value;
			}
		}

		public float RaycastDistance
		{
			get
			{
				return raycastDistance;
			}
			set
			{
				raycastDistance = value;
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

		protected virtual void OnCollisionEnter(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		private void CheckCollision(Collision collision)
		{
			if (cooldown > 0f)
			{
				return;
			}
			float magnitude = collision.relativeVelocity.magnitude;
			if (!(magnitude >= speedMin))
			{
				return;
			}
			cooldown = delay;
			Vector3 upwards = ((orientation == OrientationType.CameraUp) ? P3dHelper.GetCameraUp(_camera) : Vector3.up);
			ContactPoint[] contacts = collision.contacts;
			float pressure = Mathf.InverseLerp(speedMin, speedPressure, magnitude);
			int num = contacts.Length - 1;
			while (num >= 0)
			{
				ContactPoint contactPoint = contacts[num];
				Vector3 worldPosition = contactPoint.point + contactPoint.normal * offset;
				Quaternion worldRotation = Quaternion.LookRotation(-contactPoint.normal, upwards);
				hitCache.InvokePoints(base.gameObject, preview: false, priority, contactPoint.otherCollider, worldPosition, worldRotation, 1f);
				if (raycastDistance > 0f)
				{
					Ray ray = new Ray(contactPoint.point + contactPoint.normal * raycastDistance, -contactPoint.normal);
					RaycastHit hitInfo = default(RaycastHit);
					if (contactPoint.otherCollider.Raycast(ray, out hitInfo, raycastDistance * 2f))
					{
						hitCache.InvokeRaycast(base.gameObject, preview: false, priority, hitInfo, pressure);
					}
				}
				if (!onlyUseFirstContact)
				{
					num--;
					continue;
				}
				break;
			}
		}
	}
}
