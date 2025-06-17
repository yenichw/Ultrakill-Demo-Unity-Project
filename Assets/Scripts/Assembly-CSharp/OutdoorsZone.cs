using UnityEngine;

public class OutdoorsZone : MonoBehaviour
{
	private OutdoorLightMaster olm;

	private bool hasRequested;

	private void Start()
	{
		olm = GetComponentInParent<OutdoorLightMaster>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !hasRequested)
		{
			olm.AddRequest();
			hasRequested = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && hasRequested)
		{
			olm.RemoveRequest();
			hasRequested = false;
		}
	}
}
