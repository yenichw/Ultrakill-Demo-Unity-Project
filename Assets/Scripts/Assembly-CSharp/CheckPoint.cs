using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public StatsManager sm;

	private bool activated;

	public GameObject toActivate;

	public GameObject[] rooms;

	public GameObject[] roomsToInherit;

	private List<string> inheritNames = new List<string>();

	private List<Transform> inheritParents = new List<Transform>();

	private GameObject[] roomsPriv;

	[HideInInspector]
	public List<GameObject> defaultRooms = new List<GameObject>();

	public Door[] doorsToUnlock;

	[HideInInspector]
	public List<GameObject> newRooms = new List<GameObject>();

	private int i;

	private GameObject player;

	private NewMovement nm;

	private float tempRot;

	public GameObject graphic;

	public int restartDeaths;

	public int stylePoints;

	private StyleHUD shud;

	public List<int> succesfulHitters = new List<int>();

	private void Start()
	{
		GameObject[] array = rooms;
		foreach (GameObject item in array)
		{
			defaultRooms.Add(item);
		}
		for (int j = 0; j < defaultRooms.Count; j++)
		{
			defaultRooms[j].GetComponent<GoreZone>().checkpoint = this;
			newRooms.Add(Object.Instantiate(defaultRooms[j], defaultRooms[j].transform.position, defaultRooms[j].transform.rotation, defaultRooms[j].transform.parent));
			defaultRooms[j].gameObject.SetActive(value: false);
			newRooms[j].gameObject.SetActive(value: true);
			defaultRooms[j].transform.position = new Vector3(defaultRooms[j].transform.position.x + 10000f, defaultRooms[j].transform.position.y, defaultRooms[j].transform.position.z);
		}
		player = GameObject.FindWithTag("Player");
		sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
		for (int k = 0; k < roomsToInherit.Length; k++)
		{
			inheritNames.Add(roomsToInherit[k].name);
			inheritParents.Add(roomsToInherit[k].transform.parent);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (activated || !(other.gameObject.tag == "Player"))
		{
			return;
		}
		sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		sm.currentCheckPoint = this;
		activated = true;
		GetComponent<AudioSource>().Play();
		Object.Destroy(graphic);
		stylePoints = sm.stylePoints;
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
		if (roomsToInherit.Length == 0)
		{
			return;
		}
		for (int i = 0; i < roomsToInherit.Length; i++)
		{
			string text = inheritNames[i];
			text = text.Replace("(Clone)", "");
			GameObject gameObject = null;
			for (int num = inheritParents[i].childCount - 1; num >= 0; num--)
			{
				GameObject gameObject2 = inheritParents[i].GetChild(num).gameObject;
				if (gameObject2.name.Replace("(Clone)", "") == text)
				{
					if (gameObject == null)
					{
						gameObject = gameObject2;
					}
					else
					{
						Object.Destroy(gameObject2);
					}
				}
			}
			InheritRoom(gameObject);
		}
	}

	public void OnRespawn()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		player.transform.position = Vector3.one * -1000f;
		this.i = 0;
		sm.kills -= restartDeaths;
		restartDeaths = 0;
		sm.stylePoints = stylePoints;
		if (succesfulHitters.Count > 0)
		{
			KillHitterCache killHitterCache = Object.FindObjectOfType<KillHitterCache>();
			if ((bool)killHitterCache)
			{
				foreach (int succesfulHitter in succesfulHitters)
				{
					killHitterCache.RemoveId(succesfulHitter);
				}
			}
		}
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
		shud.ComboOver();
		if (doorsToUnlock.Length != 0)
		{
			Door[] array = doorsToUnlock;
			foreach (Door door in array)
			{
				if (door.locked)
				{
					door.Unlock();
				}
				if (door.startOpen)
				{
					door.Open();
				}
			}
		}
		Projectile[] array2 = Object.FindObjectsOfType<Projectile>();
		if (array2 != null && array2.Length != 0)
		{
			Projectile[] array3 = array2;
			foreach (Projectile projectile in array3)
			{
				if (projectile.gameObject.activeInHierarchy)
				{
					Object.Destroy(projectile.gameObject);
				}
			}
		}
		Explosion[] array4 = Object.FindObjectsOfType<Explosion>();
		if (array4 != null && array4.Length != 0)
		{
			Explosion[] array5 = array4;
			foreach (Explosion explosion in array5)
			{
				if (explosion.gameObject.activeInHierarchy)
				{
					Object.Destroy(explosion.gameObject);
				}
			}
		}
		Harpoon[] array6 = Object.FindObjectsOfType<Harpoon>();
		if (array6 != null && array6.Length != 0)
		{
			Harpoon[] array7 = array6;
			foreach (Harpoon harpoon in array7)
			{
				if (harpoon.gameObject.activeInHierarchy)
				{
					TimeBomb componentInChildren = harpoon.GetComponentInChildren<TimeBomb>();
					if ((bool)componentInChildren)
					{
						componentInChildren.dontExplode = true;
					}
					Object.Destroy(harpoon.gameObject);
				}
			}
		}
		if (newRooms.Count > 0)
		{
			ResetRoom();
		}
	}

	private void ResetRoom()
	{
		Vector3 position = newRooms[i].transform.position;
		Object.Destroy(newRooms[i]);
		newRooms[i] = Object.Instantiate(defaultRooms[i], position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
		newRooms[i].SetActive(value: true);
		if (i + 1 < defaultRooms.Count)
		{
			i++;
			ResetRoom();
			return;
		}
		toActivate.SetActive(value: true);
		player.transform.position = base.transform.position + base.transform.right * 0.1f + Vector3.up * 1.25f;
		player.GetComponent<Rigidbody>().velocity = Vector3.zero;
		if (nm == null)
		{
			nm = player.GetComponent<NewMovement>();
		}
		nm.cc.ResetCamera(base.transform.rotation.eulerAngles.y + 0.1f, instant: true);
		if (!nm.enabled)
		{
			nm.enabled = true;
		}
		nm.Respawn();
		nm.GetHealth(0, silent: true);
		nm.cc.StopShake();
	}

	public void UpdateRooms()
	{
		Vector3 position = newRooms[i].transform.position;
		Object.Destroy(newRooms[i]);
		newRooms[i] = Object.Instantiate(defaultRooms[i], position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
		newRooms[i].SetActive(value: true);
		if (i + 1 < defaultRooms.Count)
		{
			i++;
			UpdateRooms();
		}
		else
		{
			i = 0;
		}
	}

	public void InheritRoom(GameObject targetRoom)
	{
		new List<GameObject>();
		new List<GameObject>();
		defaultRooms.Add(targetRoom);
		int index = defaultRooms.IndexOf(targetRoom);
		defaultRooms[index].GetComponent<GoreZone>().checkpoint = this;
		newRooms.Add(Object.Instantiate(defaultRooms[index], defaultRooms[index].transform.position, defaultRooms[index].transform.rotation, defaultRooms[index].transform.parent));
		defaultRooms[index].gameObject.SetActive(value: false);
		newRooms[index].gameObject.SetActive(value: true);
		defaultRooms[index].transform.position = new Vector3(defaultRooms[index].transform.position.x + 10000f, defaultRooms[index].transform.position.y, defaultRooms[index].transform.position.z);
	}
}
