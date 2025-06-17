using UnityEngine;

namespace cakeslice
{
	public class Toggle : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.K))
			{
				GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
			}
		}
	}
}
