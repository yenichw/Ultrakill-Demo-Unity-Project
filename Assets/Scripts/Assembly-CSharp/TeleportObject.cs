using UnityEngine;

public class TeleportObject : MonoBehaviour
{
	public Vector3 position;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			other.transform.position = position;
		}
	}
}
