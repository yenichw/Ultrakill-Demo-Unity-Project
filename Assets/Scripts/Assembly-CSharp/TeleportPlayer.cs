using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
	public Vector3 relativePosition;

	private NewMovement nm;

	public bool notRelative;

	public bool relativeToCollider;

	public Vector3 objectivePosition;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (notRelative)
			{
				other.transform.position = objectivePosition;
			}
			else if (relativeToCollider)
			{
				other.transform.position = base.transform.position + relativePosition;
			}
			else
			{
				other.transform.position = other.transform.position + relativePosition;
			}
		}
	}
}
