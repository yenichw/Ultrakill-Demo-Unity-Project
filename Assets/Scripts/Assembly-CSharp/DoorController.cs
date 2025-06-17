using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public int type;

	private Door dc;

	private bool open;

	private bool playerIn;

	public bool enemyIn;

	public bool reverseDirection;

	private List<GameObject> doorUsers = new List<GameObject>();

	private List<GameObject> doorUsersToDelete = new List<GameObject>();

	private void Start()
	{
		dc = base.transform.parent.GetComponentInChildren<Door>();
	}

	private void Update()
	{
		if ((playerIn || enemyIn) && !open && !dc.locked)
		{
			open = true;
			if (reverseDirection)
			{
				dc.reverseDirection = true;
			}
			else
			{
				dc.reverseDirection = false;
			}
			if (playerIn)
			{
				dc.Optimize();
			}
			if (type == 0)
			{
				if (!playerIn)
				{
					dc.Open(enemy: true);
				}
				else
				{
					dc.Open();
				}
			}
			else if (type == 1)
			{
				if (!playerIn)
				{
					dc.Open(enemy: true);
				}
				else
				{
					dc.Open();
				}
				Object.Destroy(this);
			}
			else if (type == 2)
			{
				dc.Close();
				Object.Destroy(this);
			}
		}
		else if (open && !dc.locked && !playerIn && !enemyIn)
		{
			Close();
		}
		if (enemyIn && doorUsers.Count > 0)
		{
			foreach (GameObject doorUser in doorUsers)
			{
				if (doorUser == null || !doorUser.activeInHierarchy)
				{
					doorUsersToDelete.Add(doorUser);
				}
			}
			if (doorUsersToDelete.Count > 0)
			{
				foreach (GameObject item in doorUsersToDelete)
				{
					doorUsers.Remove(item);
				}
				doorUsersToDelete.Clear();
				if (doorUsers.Count <= 0)
				{
					enemyIn = false;
				}
			}
		}
		if (!playerIn && !enemyIn && dc.transform.localPosition == dc.closedPos)
		{
			open = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerIn = true;
		}
		else if (other.gameObject.tag == "Enemy" && !open)
		{
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component != null && component.type == EnemyType.Machine)
			{
				enemyIn = true;
				doorUsers.Add(other.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerIn = false;
		}
		else
		{
			if (!(other.gameObject.tag == "Enemy"))
			{
				return;
			}
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component != null && component.type == EnemyType.Machine)
			{
				enemyIn = false;
				if (doorUsers.Contains(other.gameObject))
				{
					doorUsers.Remove(other.gameObject);
				}
			}
		}
	}

	public void Close()
	{
		open = false;
		dc.Close();
	}
}
