using System.Collections.Generic;
using UnityEngine;

public class GunSetter : MonoBehaviour
{
	public GunControl gunc;

	[Header("Revolver")]
	public GameObject revolverPierce;

	public GameObject revolverRicochet;

	[Header("Shotgun")]
	public GameObject shotgunGrenade;

	public GameObject shotgunPump;

	[Header("Nailgun")]
	public GameObject nailMagnet;

	private void Start()
	{
		gunc = GetComponent<GunControl>();
		if (base.enabled)
		{
			ResetWeapons();
		}
	}

	public void ResetWeapons()
	{
		if (gunc == null)
		{
			gunc = GetComponent<GunControl>();
		}
		foreach (List<GameObject> slot in gunc.slots)
		{
			foreach (GameObject item in slot)
			{
				Object.Destroy(item);
			}
			slot.Clear();
		}
		CheckWeapon("rev0", gunc.slot1, revolverPierce);
		CheckWeapon("rev2", gunc.slot1, revolverRicochet);
		CheckWeapon("sho0", gunc.slot2, shotgunGrenade);
		CheckWeapon("sho1", gunc.slot2, shotgunPump);
		CheckWeapon("nai0", gunc.slot3, nailMagnet);
		gunc.UpdateWeaponList();
	}

	private void CheckWeapon(string name, List<GameObject> slot, GameObject prefab)
	{
		if (PlayerPrefs.GetInt(name, 0) == 1 && GameProgressSaver.CheckGear(name) > 0 && prefab != null)
		{
			slot.Add(Object.Instantiate(prefab, base.transform));
		}
	}

	public void ForceWeapon(string weaponName)
	{
		if (gunc == null)
		{
			gunc = GetComponent<GunControl>();
		}
		switch (weaponName)
		{
		case "rev0":
			gunc.ForceWeapon(revolverPierce);
			break;
		case "rev2":
			gunc.ForceWeapon(revolverRicochet);
			break;
		case "sho0":
			gunc.ForceWeapon(shotgunGrenade);
			break;
		case "sho1":
			gunc.ForceWeapon(shotgunPump);
			break;
		case "nai0":
			gunc.ForceWeapon(nailMagnet);
			break;
		}
	}
}
