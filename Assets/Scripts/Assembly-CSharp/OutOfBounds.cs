using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
	private StatsManager sman;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	public Door[] toUnlock;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (other.gameObject.GetComponent<NewMovement>().hp <= 0)
			{
				return;
			}
			other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			if (sman == null)
			{
				sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			}
			if (sman.currentCheckPoint != null)
			{
				other.transform.position = sman.currentCheckPoint.transform.position + Vector3.up * 1.25f;
				sman.currentCheckPoint.toActivate.SetActive(value: true);
				Door[] doorsToUnlock = sman.currentCheckPoint.doorsToUnlock;
				for (int i = 0; i < doorsToUnlock.Length; i++)
				{
					doorsToUnlock[i].Unlock();
				}
			}
			else
			{
				other.transform.position = sman.spawnPos;
				GameObject[] array = toActivate;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
				array = toDisactivate;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				Door[] doorsToUnlock = toUnlock;
				for (int i = 0; i < doorsToUnlock.Length; i++)
				{
					doorsToUnlock[i].Unlock();
				}
			}
			GetComponent<HudMessage>().PlayMessage();
		}
		else if (other.gameObject.layer == 10 || other.gameObject.layer == 9)
		{
			Object.Destroy(other.gameObject);
		}
		else if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("Instakill Phase 1");
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			if (!component.dead)
			{
				component.InstaKill();
			}
		}
	}
}
