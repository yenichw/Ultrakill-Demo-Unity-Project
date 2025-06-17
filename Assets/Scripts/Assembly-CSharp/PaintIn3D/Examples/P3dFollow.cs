using UnityEngine;

namespace PaintIn3D.Examples
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dFollow")]
	[AddComponentMenu("Paint in 3D/Examples/Follow")]
	public class P3dFollow : MonoBehaviour
	{
		[SerializeField]
		private Transform target;

		[SerializeField]
		private Vector3 offset;

		[SerializeField]
		private Vector3 tilt;

		[SerializeField]
		private float dampening = 10f;

		public Transform Target
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

		public Vector3 Offset
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

		public Vector3 Tilt
		{
			get
			{
				return tilt;
			}
			set
			{
				tilt = value;
			}
		}

		public float Dampening
		{
			get
			{
				return dampening;
			}
			set
			{
				dampening = value;
			}
		}

		protected virtual void LateUpdate()
		{
			if (target != null)
			{
				Vector3 b = target.TransformPoint(offset);
				Quaternion b2 = target.rotation * Quaternion.Euler(tilt);
				float t = P3dHelper.DampenFactor(dampening, Time.deltaTime);
				base.transform.position = Vector3.Lerp(base.transform.position, b, t);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b2, t);
			}
		}
	}
}
