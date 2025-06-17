using UnityEngine;

public class EnemyRevolver : MonoBehaviour
{
	public int variation;

	public GameObject bullet;

	public GameObject altBullet;

	public GameObject primaryPrepare;

	private GameObject currentpp;

	private GameObject altCharge;

	private AudioSource altChargeAud;

	private float chargeAmount;

	private bool charging;

	public Transform shootPoint;

	public GameObject muzzleFlash;

	public GameObject muzzleFlashAlt;

	private void Start()
	{
		altCharge = shootPoint.GetChild(0).gameObject;
		altChargeAud = altCharge.GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (charging)
		{
			chargeAmount = Mathf.MoveTowards(chargeAmount, 1f, Time.deltaTime * 2f);
			altChargeAud.pitch = chargeAmount / 2f;
			altCharge.transform.localScale = Vector3.one * chargeAmount * 5f;
		}
	}

	public void Fire()
	{
		if (currentpp != null)
		{
			Object.Destroy(currentpp);
		}
		Object.Instantiate(bullet, shootPoint.transform.position, shootPoint.transform.rotation);
		Object.Instantiate(muzzleFlash, shootPoint.transform.position, shootPoint.transform.rotation);
	}

	public void AltFire()
	{
		charging = false;
		chargeAmount = 0f;
		altChargeAud.pitch = 0f;
		altCharge.SetActive(value: false);
		Object.Instantiate(altBullet, shootPoint.transform.position, shootPoint.transform.rotation);
		Object.Instantiate(muzzleFlashAlt, shootPoint.transform.position, shootPoint.transform.rotation);
	}

	public void PrepareFire()
	{
		if (currentpp != null)
		{
			Object.Destroy(currentpp);
		}
		currentpp = Object.Instantiate(primaryPrepare, shootPoint);
		currentpp.transform.Rotate(Vector3.up * 90f);
	}

	public void PrepareAltFire()
	{
		altCharge.SetActive(value: true);
		charging = true;
	}

	private void OnDisable()
	{
		if (currentpp != null)
		{
			Object.Destroy(currentpp);
		}
	}
}
