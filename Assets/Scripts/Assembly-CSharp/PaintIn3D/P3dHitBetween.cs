using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PaintIn3D
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitBetween")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Between")]
	public class P3dHitBetween : P3dHitConnector
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
		private Transform pointA;

		[SerializeField]
		private Transform pointB;

		[SerializeField]
		private float fraction = 1f;

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

		[FormerlySerializedAs("delay")]
		[SerializeField]
		private float interval = 0.05f;

		[Range(0f, 1f)]
		[SerializeField]
		private float pressure = 1f;

		[SerializeField]
		private bool preview;

		[SerializeField]
		private int priority;

		[SerializeField]
		private Transform point;

		[SerializeField]
		private LineRenderer line;

		[NonSerialized]
		private float current;

		public Transform PointA
		{
			get
			{
				return pointA;
			}
			set
			{
				pointA = value;
			}
		}

		public Transform PointB
		{
			get
			{
				return pointB;
			}
			set
			{
				pointB = value;
			}
		}

		public float Fraction => fraction;

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

		public float Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
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

		public Transform Point
		{
			get
			{
				return point;
			}
			set
			{
				point = value;
			}
		}

		public LineRenderer Line
		{
			get
			{
				return line;
			}
			set
			{
				line = value;
			}
		}

		[ContextMenu("Raycast Now")]
		public void RaycastNow()
		{
			if (pointA != null && pointB != null)
			{
				Vector3 direction = pointB.position - pointA.position;
				float magnitude = direction.magnitude;
				Ray ray = new Ray(pointA.position, direction);
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, magnitude, layers))
				{
					Vector3 upwards = ((orientation == OrientationType.CameraUp) ? P3dHelper.GetCameraUp(_camera) : Vector3.up);
					Vector3 worldPosition = hitInfo.point + hitInfo.normal * offset;
					Quaternion worldRotation = Quaternion.LookRotation((normal == NormalType.HitNormal) ? hitInfo.normal : (-ray.direction), upwards);
					hitCache.InvokeRaycast(base.gameObject, preview, priority, hitInfo, pressure);
					DispatchHits(preview, priority, hitInfo.collider, worldPosition, worldRotation, pressure, this);
					fraction = (hitInfo.distance + offset) / magnitude;
					UpdatePointAndLine();
				}
				else
				{
					BreakHits(this);
					fraction = 1f;
					UpdatePointAndLine();
				}
			}
		}

		private void UpdatePointAndLine()
		{
			if (pointA != null && pointB != null)
			{
				Vector3 position = pointA.position;
				Vector3 position2 = pointB.position;
				Vector3 position3 = Vector3.Lerp(position, position2, fraction);
				if (point != null)
				{
					point.position = position3;
				}
				if (line != null)
				{
					line.positionCount = 2;
					line.SetPosition(0, position);
					line.SetPosition(1, position3);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (point != null && pointB != null)
			{
				point.position = pointB.position;
			}
		}

		protected virtual void LateUpdate()
		{
			if (interval >= 0f)
			{
				current += Time.fixedDeltaTime;
				if (current >= interval)
				{
					current = 0f;
					RaycastNow();
				}
			}
			UpdatePointAndLine();
		}
	}
}
