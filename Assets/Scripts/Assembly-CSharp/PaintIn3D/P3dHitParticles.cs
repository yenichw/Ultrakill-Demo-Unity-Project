using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[RequireComponent(typeof(ParticleSystem))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitParticles")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Particles")]
	public class P3dHitParticles : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp = 0,
			CameraUp = 1
		}

		public enum NormalType
		{
			ParticleVelocity = 0,
			CollisionNormal = 1
		}

		[SerializeField]
		private OrientationType orientation;

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private NormalType normal;

		[SerializeField]
		private float offset;

		[SerializeField]
		private int skip;

		[SerializeField]
		private bool preview;

		[SerializeField]
		private int priority;

		[SerializeField]
		[Range(0f, 1f)]
		private float pressure = 1f;

		[SerializeField]
		private float pressureMinDistance;

		[SerializeField]
		private float pressureMaxDistance;

		[NonSerialized]
		private ParticleSystem cachedParticleSystem;

		[NonSerialized]
		private static List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();

		[NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

		[NonSerialized]
		private int skipCounter;

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

		public int Skip
		{
			get
			{
				return skip;
			}
			set
			{
				skip = value;
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

		public float Pressure
		{
			get
			{
				return pressure;
			}
			set
			{
				pressure = value;
			}
		}

		public float PressureMinDistance
		{
			get
			{
				return pressureMinDistance;
			}
			set
			{
				pressureMinDistance = value;
			}
		}

		public float PressureMaxDistance
		{
			get
			{
				return pressureMaxDistance;
			}
			set
			{
				pressureMaxDistance = value;
			}
		}

		public P3dHitCache HitCache => hitCache;

		[ContextMenu("Clear Hit Cache")]
		public void ClearHitCache()
		{
			hitCache.Clear();
		}

		protected virtual void OnEnable()
		{
			cachedParticleSystem = GetComponent<ParticleSystem>();
		}

		protected virtual void OnParticleCollision(GameObject hitGameObject)
		{
			int safeCollisionEventSize = cachedParticleSystem.GetSafeCollisionEventSize();
			for (int i = particleCollisionEvents.Count; i < safeCollisionEventSize; i++)
			{
				particleCollisionEvents.Add(default(ParticleCollisionEvent));
			}
			safeCollisionEventSize = cachedParticleSystem.GetCollisionEvents(hitGameObject, particleCollisionEvents);
			Vector3 upwards = ((orientation == OrientationType.CameraUp) ? P3dHelper.GetCameraUp(_camera) : Vector3.up);
			for (int j = 0; j < safeCollisionEventSize; j++)
			{
				if (skip > 0)
				{
					if (skipCounter++ <= skip)
					{
						continue;
					}
					skipCounter = 0;
				}
				ParticleCollisionEvent particleCollisionEvent = particleCollisionEvents[j];
				Vector3 worldPosition = particleCollisionEvent.intersection + particleCollisionEvent.normal * offset;
				Vector3 vector = ((normal == NormalType.CollisionNormal) ? (-particleCollisionEvent.normal) : particleCollisionEvent.velocity);
				Quaternion worldRotation = ((vector != Vector3.zero) ? Quaternion.LookRotation(vector, upwards) : Quaternion.identity);
				float num = pressure;
				if (pressureMinDistance != pressureMaxDistance)
				{
					float value = Vector3.Distance(base.transform.position, particleCollisionEvent.intersection);
					num *= Mathf.InverseLerp(pressureMinDistance, pressureMaxDistance, value);
				}
				hitCache.InvokePoints(base.gameObject, preview, priority, particleCollisionEvent.colliderComponent as Collider, worldPosition, worldRotation, num);
			}
		}
	}
}
