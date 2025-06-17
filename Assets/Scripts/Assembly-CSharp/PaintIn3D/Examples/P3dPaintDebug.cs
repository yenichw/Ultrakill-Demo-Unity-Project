using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintDebug")]
	[AddComponentMenu("Paint in 3D/Examples/Paint Debug")]
	public class P3dPaintDebug : MonoBehaviour, IHit, IHitPoint, IHitLine
	{
		[SerializeField]
		private Color color = Color.white;

		[SerializeField]
		private float duration = 0.05f;

		[SerializeField]
		private float size = 0.05f;

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public float Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public float Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
			}
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			Vector3 vector = worldPosition + worldRotation * new Vector3(0f - size, 0f - size);
			Vector3 vector2 = worldPosition + worldRotation * new Vector3(0f - size, size);
			Vector3 vector3 = worldPosition + worldRotation * new Vector3(size, size);
			Vector3 vector4 = worldPosition + worldRotation * new Vector3(size, 0f - size);
			Color color = this.color;
			if (preview)
			{
				color.a *= 0.5f;
			}
			color.a *= pressure * 0.75f + 0.25f;
			Debug.DrawLine(vector, vector2, color, duration);
			Debug.DrawLine(vector2, vector3, color, duration);
			Debug.DrawLine(vector3, vector4, color, duration);
			Debug.DrawLine(vector4, vector, color, duration);
			Debug.DrawLine(worldPosition, worldPosition + worldRotation * Vector3.forward * size, color, duration);
		}

		public void HandleHitLine(bool preview, int priority, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			Color color = this.color;
			if (preview)
			{
				color.a *= 0.5f;
			}
			color.a *= pressureA * 0.75f + 0.25f;
			Debug.DrawLine(worldPositionA, worldPositionB, color, duration);
		}
	}
}
