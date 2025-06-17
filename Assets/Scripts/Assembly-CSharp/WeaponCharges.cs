using UnityEngine;

public class WeaponCharges : MonoBehaviour
{
	private GunControl gc;

	public float rev0charge = 100f;

	public float rev1charge = 400f;

	public float nai0charge = 2f;

	public float nai0heatUp;

	public float nai1heatUp;

	public float raicharge = 5f;

	public GameObject railCannonFullChargeSound;

	public bool railChargePlayed;

	private void Update()
	{
		if (rev0charge < 100f)
		{
			rev0charge = Mathf.MoveTowards(rev0charge, 100f, 40f * Time.deltaTime);
		}
		if (rev1charge < 400f)
		{
			rev1charge = Mathf.MoveTowards(rev1charge, 400f, 25f * Time.deltaTime);
		}
		if (nai0charge < 3f)
		{
			nai0charge = Mathf.MoveTowards(nai0charge, 2f, Time.deltaTime * 0.075f);
		}
		if (nai0heatUp > 0f)
		{
			nai0heatUp = Mathf.MoveTowards(nai0heatUp, 0f, Time.deltaTime * 0.3f);
		}
		if (nai1heatUp > 0f)
		{
			nai0heatUp = Mathf.MoveTowards(nai0heatUp, 0f, Time.deltaTime * 0.3f);
		}
		if (raicharge < 5f)
		{
			raicharge = Mathf.MoveTowards(raicharge, 5f, Time.deltaTime * 0.25f);
			if (raicharge >= 4f && (bool)railCannonFullChargeSound)
			{
				raicharge = 5f;
				PlayRailCharge();
			}
		}
	}

	public void MaxCharges()
	{
		rev0charge = 100f;
		rev1charge = 400f;
		nai0charge = 2f;
		nai0heatUp = 0f;
		nai1heatUp = 0f;
		if (raicharge < 5f)
		{
			raicharge = 5f;
			PlayRailCharge();
		}
		if (!gc)
		{
			gc = GetComponent<GunControl>();
		}
		if ((bool)gc && (bool)gc.currentWeapon)
		{
			gc.currentWeapon.SendMessage("MaxCharge", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void PlayRailCharge()
	{
		railChargePlayed = true;
		Object.Instantiate(railCannonFullChargeSound);
	}
}
