using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dDestroyAfterTime")]
	[AddComponentMenu("Paint in 3D/Examples/Destroy After Time")]
	public class P3dDestroyAfterTime : MonoBehaviour
	{
		[SerializeField]
		private float seconds = 5f;

		[SerializeField]
		private float age;

		public float Seconds
		{
			get
			{
				return seconds;
			}
			set
			{
				seconds = value;
			}
		}

		[ContextMenu("Destroy Now")]
		public void DestroyNow()
		{
			Object.Destroy(base.gameObject);
		}

		protected virtual void Update()
		{
			if (seconds >= 0f)
			{
				age += Time.deltaTime;
				if (age >= seconds)
				{
					DestroyNow();
				}
			}
		}
	}
}
