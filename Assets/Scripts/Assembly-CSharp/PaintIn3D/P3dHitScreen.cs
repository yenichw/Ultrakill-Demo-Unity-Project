using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitScreen")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Screen")]
	public class P3dHitScreen : P3dHitConnector
	{
		private class Link
		{
			public P3dInputManager.Finger Finger;

			public float Distance;
		}

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
		private Camera _camera;

		[SerializeField]
		private float spacing = 5f;

		[SerializeField]
		private LayerMask layers = -5;

		[SerializeField]
		private KeyCode key = KeyCode.Mouse0;

		[SerializeField]
		private OrientationType orientation = OrientationType.CameraUp;

		[SerializeField]
		private NormalType normal;

		[SerializeField]
		private float offset;

		[SerializeField]
		private float touchOffset;

		[SerializeField]
		private bool showPreview = true;

		[SerializeField]
		private int priority;

		[SerializeField]
		private bool storeStates = true;

		[NonSerialized]
		private List<Link> links = new List<Link>();

		[NonSerialized]
		private P3dInputManager inputManager = new P3dInputManager();

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

		public float Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
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

		public KeyCode Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
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

		public float TouchOffset
		{
			get
			{
				return touchOffset;
			}
			set
			{
				touchOffset = value;
			}
		}

		public bool ShowPreview
		{
			get
			{
				return showPreview;
			}
			set
			{
				showPreview = value;
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

		public bool StoreStates
		{
			get
			{
				return storeStates;
			}
			set
			{
				storeStates = value;
			}
		}

		protected void LateUpdate()
		{
			inputManager.Update(key);
			if (showPreview)
			{
				if (Input.touchCount == 0 && !Input.GetKey(key) && !P3dInputManager.PointOverGui(Input.mousePosition))
				{
					PaintAt(Input.mousePosition, preview: true, 1f, this);
				}
				else
				{
					BreakHits(this);
				}
			}
			for (int num = inputManager.Fingers.Count - 1; num >= 0; num--)
			{
				P3dInputManager.Finger finger = inputManager.Fingers[num];
				bool down = finger.Down;
				bool up = finger.Up;
				Paint(finger, down, up);
			}
		}

		private void Paint(P3dInputManager.Finger finger, bool down, bool up)
		{
			Link link = GetLink(finger);
			if (spacing > 0f)
			{
				Vector2 vector = finger.SmoothPositions[0];
				if (down)
				{
					link.Distance = 0f;
					if (storeStates)
					{
						P3dStateManager.StoreAllStates();
					}
					PaintAt(vector, preview: false, finger.Pressure, link);
				}
				for (int i = 1; i < finger.SmoothPositions.Count; i++)
				{
					Vector2 vector2 = finger.SmoothPositions[i];
					float num = Vector2.Distance(vector, vector2);
					int num2 = Mathf.FloorToInt((link.Distance + num) / spacing);
					for (int j = 0; j < num2; j++)
					{
						float num3 = spacing - link.Distance;
						vector = Vector2.MoveTowards(vector, vector2, num3);
						PaintAt(vector, preview: false, finger.Pressure, link);
						num -= num3;
						link.Distance = 0f;
					}
					link.Distance += num;
					vector = vector2;
				}
			}
			else if (showPreview)
			{
				if (up)
				{
					if (storeStates)
					{
						P3dStateManager.StoreAllStates();
					}
					PaintAt(finger.PositionA, preview: false, finger.Pressure, link);
				}
				else
				{
					PaintAt(finger.PositionA, preview: true, finger.Pressure, link);
				}
			}
			else if (down)
			{
				if (storeStates)
				{
					P3dStateManager.StoreAllStates();
				}
				PaintAt(finger.PositionA, preview: false, finger.Pressure, link);
			}
			if (up)
			{
				BreakHits(link);
			}
		}

		private void PaintAt(Vector2 screenPosition, bool preview, float pressure, object owner)
		{
			Camera camera = P3dHelper.GetCamera(_camera);
			if (camera != null)
			{
				if (touchOffset != 0f && Input.touchCount > 0)
				{
					screenPosition.y += touchOffset * P3dInputManager.ScaleFactor;
				}
				Ray ray = camera.ScreenPointToRay(screenPosition);
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layers))
				{
					Vector3 upwards = ((orientation == OrientationType.CameraUp) ? camera.transform.up : Vector3.up);
					Vector3 worldPosition = hitInfo.point + hitInfo.normal * offset;
					Quaternion worldRotation = Quaternion.LookRotation((normal == NormalType.HitNormal) ? (-hitInfo.normal) : ray.direction, upwards);
					hitCache.InvokeRaycast(base.gameObject, preview, priority, hitInfo, pressure);
					DispatchHits(preview, priority, hitInfo.collider, worldPosition, worldRotation, pressure, owner);
					return;
				}
			}
			BreakHits(owner);
		}

		private Link GetLink(P3dInputManager.Finger finger)
		{
			for (int num = links.Count - 1; num >= 0; num--)
			{
				Link link = links[num];
				if (link.Finger == finger)
				{
					return link;
				}
			}
			Link link2 = new Link();
			link2.Finger = finger;
			links.Add(link2);
			return link2;
		}
	}
}
