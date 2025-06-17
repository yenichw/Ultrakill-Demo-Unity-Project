using UnityEngine;

public class ActivateArena : MonoBehaviour
{
	public bool onlyWave;

	private bool activated;

	public Door[] doors;

	public GameObject[] enemies;

	private int currentEnemy;

	public bool forEnemy;

	public bool waitForStatus;

	private ArenaStatus astat;

	private void OnTriggerEnter(Collider other)
	{
		if (waitForStatus)
		{
			if (astat == null)
			{
				astat = GetComponentInParent<ArenaStatus>();
			}
			if (astat == null || !astat.activated)
			{
				return;
			}
		}
		if ((forEnemy || !(other.gameObject.tag == "Player") || activated) && (!forEnemy || !(other.gameObject.tag == "Enemy") || activated))
		{
			return;
		}
		activated = true;
		if (!onlyWave && !forEnemy)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().ArenaMusicStart();
		}
		if (doors.Length != 0)
		{
			Door[] array = doors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Lock();
			}
			if (enemies.Length != 0)
			{
				Invoke("SpawnEnemy", 1f);
			}
		}
		else if (enemies.Length != 0)
		{
			SpawnEnemy();
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void SpawnEnemy()
	{
		if (enemies[currentEnemy] != null)
		{
			enemies[currentEnemy].SetActive(value: true);
		}
		currentEnemy++;
		if (currentEnemy < enemies.Length)
		{
			Invoke("SpawnEnemy", 0.1f);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
