using UnityEngine;

public class ScreenZone : MonoBehaviour
{
	private GunControl gc;

	private FistControl pun;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.NoWeapon();
			if (pun == null)
			{
				pun = other.GetComponentInChildren<FistControl>();
			}
			pun.ShopMode();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.YesWeapon();
			pun.StopShop();
		}
	}
}
