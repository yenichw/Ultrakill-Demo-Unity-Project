using UnityEngine;

public class PlayerActivator : MonoBehaviour
{
	public GameObject[] toActivate;

	public float delay;

	private NewMovement nm;

	private bool activated;

	private int i;

	public bool startTimer;

	private GunControl gc;

	public GameObject impactDust;

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.tag == "Player")
		{
			nm = other.gameObject.GetComponentInParent<NewMovement>();
			gc = other.gameObject.GetComponentInChildren<GunControl>();
			if (!nm.activated)
			{
				nm.activated = true;
				nm.cc.activated = true;
				nm.cc.CameraShake(1f);
				GetComponent<AudioSource>().Play();
			}
			activated = true;
			if (toActivate.Length != 0)
			{
				gc.YesWeapon();
				ActivateObjects();
			}
			if (startTimer)
			{
				GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().StartTimer();
			}
			Object.FindObjectOfType<FistControl>().YesFist();
		}
	}

	private void ActivateObjects()
	{
		if (i < 3 || i > 4 || !gc.noWeapons)
		{
			toActivate[i].SetActive(value: true);
		}
		i++;
		if (i < toActivate.Length)
		{
			Invoke("ActivateObjects", delay);
		}
	}
}
