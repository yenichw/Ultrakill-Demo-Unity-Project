using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
	private AudioSource aud;

	private Door blockedDoor;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Door")
		{
			Door door = (blockedDoor = collision.gameObject.GetComponentInParent<Door>());
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			if (aud != null)
			{
				aud.Play();
			}
			if (door != null)
			{
				door.Close();
			}
		}
	}

	private void OnDestroy()
	{
		if (blockedDoor != null)
		{
			blockedDoor.Open();
		}
	}
}
