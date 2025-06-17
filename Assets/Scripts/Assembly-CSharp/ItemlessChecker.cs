using UnityEngine;

public class ItemlessChecker : MonoBehaviour
{
	private ItemIdentifier itid;

	private bool failed;

	private void Update()
	{
		if (failed)
		{
			return;
		}
		if (itid == null)
		{
			itid = GetComponent<ItemIdentifier>();
		}
		if (itid != null && itid.pickedUp)
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
