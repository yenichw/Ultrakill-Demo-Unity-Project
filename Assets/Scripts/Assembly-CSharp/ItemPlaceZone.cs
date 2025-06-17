using UnityEngine;

public class ItemPlaceZone : MonoBehaviour
{
	public ItemType acceptedItemType;

	public GameObject[] activateOnSuccess;

	public GameObject[] deactivateOnSuccess;

	public GameObject[] activateOnFailure;

	public Door[] doors;

	public Door[] reverseDoors;

	public ArenaStatus[] arenaStatuses;

	public ArenaStatus[] reverseArenaStatuses;

	private Collider col;

	private void Start()
	{
		col = GetComponent<Collider>();
		CheckItem();
	}

	public void CheckItem()
	{
		ItemIdentifier componentInChildren = GetComponentInChildren<ItemIdentifier>();
		GameObject[] array;
		Door[] array2;
		ArenaStatus[] array3;
		if (componentInChildren != null)
		{
			if (componentInChildren.itemType == acceptedItemType)
			{
				array = activateOnSuccess;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
				array = deactivateOnSuccess;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				array2 = doors;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Open(enemy: false, skull: true);
				}
				array2 = reverseDoors;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Close();
				}
				array3 = arenaStatuses;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].activated = true;
				}
				array3 = reverseArenaStatuses;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].activated = false;
				}
			}
			else
			{
				array = activateOnFailure;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
			}
			col.enabled = false;
			return;
		}
		array = activateOnSuccess;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = activateOnFailure;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = deactivateOnSuccess;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		array2 = doors;
		foreach (Door door in array2)
		{
			if (door.transform.localPosition != door.closedPos)
			{
				door.Close();
			}
		}
		array2 = reverseDoors;
		foreach (Door door2 in array2)
		{
			if (door2.transform.localPosition != door2.closedPos + door2.openPos)
			{
				door2.Open(enemy: false, skull: true);
			}
		}
		array3 = arenaStatuses;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].activated = false;
		}
		array3 = reverseArenaStatuses;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].activated = true;
		}
		col.enabled = true;
	}
}
