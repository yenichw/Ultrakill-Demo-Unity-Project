using UnityEngine;

namespace PaintIn3D.Examples
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dDrip")]
	[AddComponentMenu("Paint in 3D/Examples/Drip")]
	public class P3dDrip : MonoBehaviour
	{
		[SerializeField]
		private float speed = 1f;

		[SerializeField]
		private float dampening = 1f;

		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
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
			base.transform.position += Vector3.down * speed * Time.deltaTime;
			float t = P3dHelper.DampenFactor(dampening, Time.deltaTime);
			speed = Mathf.Lerp(speed, 0f, t);
		}
	}
}
