using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
	public GameObject weapon;

	public int inventorySlot;

	private int tempSlot;

	public GunSetter gs;

	private GunControl gc;

	public string pPref;

	public bool arm;

	private FistControl fc;

	public GameObject activateOnPickup;

	private void Start()
	{
		tempSlot = inventorySlot;
		if (!arm)
		{
			tempSlot--;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (!arm)
			{
				gc = collision.gameObject.GetComponentInChildren<GunControl>();
			}
			else
			{
				fc = collision.gameObject.GetComponentInChildren<FistControl>();
			}
			GotActivated();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (!arm)
			{
				gc = other.gameObject.GetComponentInChildren<GunControl>();
			}
			else
			{
				fc = other.gameObject.GetComponentInChildren<FistControl>();
			}
			GotActivated();
		}
	}

	private void GotActivated()
	{
		if (!arm)
		{
			gc.noWeapons = false;
			if (gs != null)
			{
				gs.enabled = true;
				gs.ResetWeapons();
			}
			bool flag = false;
			for (int i = 0; i < gc.slots[tempSlot].Count; i++)
			{
				if (gc.slots[tempSlot][i].name == weapon.name + "(Clone)")
				{
					flag = true;
				}
			}
			if (!flag)
			{
				GameObject item = Object.Instantiate(weapon, gc.transform);
				gc.slots[tempSlot].Add(item);
				gc.ForceWeapon(weapon);
				gc.noWeapons = false;
				gc.UpdateWeaponList();
			}
		}
		PlayerPrefs.SetInt(pPref, 1);
		GameProgressSaver.AddGear(pPref);
		if (arm)
		{
			fc.ResetFists();
			fc.ForceArm(tempSlot, animation: true);
		}
		if (activateOnPickup != null)
		{
			activateOnPickup.SetActive(value: true);
		}
		Object.Destroy(base.gameObject);
	}
}
