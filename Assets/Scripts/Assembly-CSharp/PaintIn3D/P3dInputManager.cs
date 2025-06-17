using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D
{
	public class P3dInputManager
	{
		public class Finger
		{
			public int Index;

			public float Pressure;

			public bool LastSet;

			public bool Set;

			public Vector2 PositionA;

			public Vector2 PositionB;

			public Vector2 PositionC;

			public Vector2 PositionD;

			public List<Vector2> SmoothPositions = new List<Vector2>();

			public bool Down
			{
				get
				{
					if (Set)
					{
						return !LastSet;
					}
					return false;
				}
			}

			public bool Up
			{
				get
				{
					if (!Set)
					{
						return LastSet;
					}
					return false;
				}
			}
		}

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		private List<Finger> fingers = new List<Finger>();

		private static Stack<Finger> pool = new Stack<Finger>();

		public static float ScaleFactor
		{
			get
			{
				float num = Screen.dpi;
				if (num <= 0f)
				{
					num = 200f;
				}
				return 200f / num;
			}
		}

		public List<Finger> Fingers => fingers;

		public Vector2 GetAverageDeltaScaled()
		{
			Vector2 zero = Vector2.zero;
			int num = 0;
			for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
			{
				Finger finger = fingers[num2];
				zero += finger.PositionA - finger.PositionB;
				num++;
			}
			if (num > 0)
			{
				zero *= ScaleFactor;
				zero /= (float)num;
			}
			return zero;
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, 32);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();
			EventSystem current = EventSystem.current;
			if (current != null)
			{
				if (current != tempEventSystem)
				{
					tempEventSystem = current;
					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}
				tempPointerEventData.position = screenPosition;
				current.RaycastAll(tempPointerEventData, tempRaycastResults);
				if (tempRaycastResults.Count > 0)
				{
					for (int num = tempRaycastResults.Count - 1; num >= 0; num--)
					{
						if (((1 << tempRaycastResults[num].gameObject.layer) & (int)layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(num);
						}
					}
				}
			}
			return tempRaycastResults;
		}

		public void Update(KeyCode key)
		{
			for (int num = fingers.Count - 1; num >= 0; num--)
			{
				Finger finger = fingers[num];
				if (finger.Up)
				{
					fingers.RemoveAt(num);
					pool.Push(finger);
				}
				else
				{
					finger.LastSet = finger.Set;
					finger.Set = false;
				}
			}
			if (Input.touchCount > 0)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began)
					{
						CreateFinger(touch.fingerId, touch.position, touch.pressure);
					}
					else
					{
						UpdateFinger(touch.fingerId, touch.position, touch.pressure, touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
					}
				}
			}
			else
			{
				Vector2 screenPosition = Input.mousePosition;
				if (Input.GetKeyDown(key))
				{
					CreateFinger(-1, screenPosition, 1f);
				}
				else
				{
					UpdateFinger(-1, screenPosition, 1f, Input.GetKey(key));
				}
			}
		}

		private void CreateFinger(int index, Vector2 screenPosition, float pressure)
		{
			if (!PointOverGui(screenPosition))
			{
				Finger finger = ((pool.Count > 0) ? pool.Pop() : new Finger());
				finger.Index = index;
				finger.Pressure = pressure;
				finger.LastSet = false;
				finger.Set = true;
				finger.PositionA = screenPosition;
				finger.PositionB = screenPosition;
				finger.PositionC = screenPosition;
				finger.PositionD = screenPosition;
				finger.SmoothPositions.Clear();
				finger.SmoothPositions.Add(finger.PositionB);
				fingers.Add(finger);
			}
		}

		private void UpdateFinger(int index, Vector2 screenPosition, float pressure, bool set)
		{
			for (int num = fingers.Count - 1; num >= 0; num--)
			{
				Finger finger = fingers[num];
				if (finger.Index == index)
				{
					finger.Pressure = pressure;
					finger.Set = set;
					finger.PositionD = finger.PositionC;
					finger.PositionC = finger.PositionB;
					finger.PositionB = finger.PositionA;
					finger.PositionA = screenPosition;
					finger.SmoothPositions.Clear();
					finger.SmoothPositions.Add(finger.PositionC);
					if (set)
					{
						int num2 = Mathf.FloorToInt(Vector2.Distance(finger.PositionB, finger.PositionC));
						float num3 = P3dHelper.Reciprocal(num2);
						for (int i = 1; i <= num2; i++)
						{
							Vector2 item = Hermite(finger.PositionD, finger.PositionC, finger.PositionB, finger.PositionA, (float)i * num3);
							finger.SmoothPositions.Add(item);
						}
					}
					else
					{
						finger.SmoothPositions.Add(finger.PositionB);
						finger.SmoothPositions.Add(finger.PositionA);
					}
					break;
				}
			}
		}

		private static Vector2 Hermite(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
		{
			float num = t * t;
			float mu = num * t;
			float x = HermiteInterpolate(a.x, b.x, c.x, d.x, t, num, mu);
			float y = HermiteInterpolate(a.y, b.y, c.y, d.y, t, num, mu);
			return new Vector2(x, y);
		}

		private static float HermiteInterpolate(float y0, float y1, float y2, float y3, float mu, float mu2, float mu3)
		{
			float num = (y1 - y0) * 0.5f + (y2 - y1) * 0.5f;
			float num2 = (y2 - y1) * 0.5f + (y3 - y2) * 0.5f;
			float num3 = 2f * mu3 - 3f * mu2 + 1f;
			float num4 = mu3 - 2f * mu2 + mu;
			float num5 = mu3 - mu2;
			float num6 = -2f * mu3 + 3f * mu2;
			return num3 * y1 + num4 * num + num5 * num2 + num6 * y2;
		}
	}
}
