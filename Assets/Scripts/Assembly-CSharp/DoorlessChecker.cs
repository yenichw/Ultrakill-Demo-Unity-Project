using UnityEngine;

public class DoorlessChecker : MonoBehaviour
{
	private Door dr;

	private bool failed;

	private void Update()
	{
		if (failed)
		{
			return;
		}
		if (dr == null)
		{
			dr = GetComponent<Door>();
		}
		if (dr != null && dr.open)
		{
			DoorlessChallenge[] array = Resources.FindObjectsOfTypeAll<DoorlessChallenge>();
			if (array.Length != 0)
			{
				array[0].failed = true;
			}
			failed = true;
		}
	}
}
