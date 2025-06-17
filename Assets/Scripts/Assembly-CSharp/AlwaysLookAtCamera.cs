using UnityEngine;

public class AlwaysLookAtCamera : MonoBehaviour
{
	private GameObject cam;

	private void Start()
	{
		cam = GameObject.FindWithTag("MainCamera");
	}

	private void Update()
	{
		base.transform.LookAt(cam.transform);
	}
}
