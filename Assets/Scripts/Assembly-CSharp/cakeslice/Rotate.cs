using UnityEngine;

namespace cakeslice
{
	public class Rotate : MonoBehaviour
	{
		private float timer;

		private const float time = 1f;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.Rotate(Vector3.up, Time.deltaTime * 20f);
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				timer = 1f;
			}
		}
	}
}
