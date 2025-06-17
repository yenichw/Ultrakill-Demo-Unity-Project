using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dRandomlyMove")]
	[AddComponentMenu("Paint in 3D/Examples/Randomly Move")]
	public class P3dRandomlyMove : MonoBehaviour
	{
		[SerializeField]
		private Vector3 minimum = -Vector3.one;

		[SerializeField]
		private Vector3 maximum = Vector3.one;

		[SerializeField]
		private float interval = 5f;

		[SerializeField]
		private float dampening = 0.1f;

		[SerializeField]
		private float age;

		[SerializeField]
		private Vector3 target;

		public Vector3 Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = value;
			}
		}

		public Vector3 Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = value;
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

		[ContextMenu("Move Now")]
		public void MoveNow()
		{
			age = 0f;
			target.x = Mathf.Lerp(minimum.x, maximum.x, Random.value);
			target.y = Mathf.Lerp(minimum.y, maximum.y, Random.value);
			target.z = Mathf.Lerp(minimum.z, maximum.z, Random.value);
		}

		protected virtual void Start()
		{
			MoveNow();
		}

		protected virtual void Update()
		{
			Vector3 localPosition = base.transform.localPosition;
			float t = P3dHelper.DampenFactor(dampening, Time.deltaTime);
			age += Time.deltaTime;
			if (age >= interval)
			{
				age %= interval;
				MoveNow();
			}
			localPosition = Vector3.Lerp(localPosition, target, t);
			base.transform.localPosition = localPosition;
		}
	}
}
