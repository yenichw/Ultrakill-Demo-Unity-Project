using UnityEngine;

public class EnemyShotgun : MonoBehaviour
{
	private AudioSource gunAud;

	public AudioClip shootSound;

	public AudioClip clickSound;

	public AudioClip smackSound;

	private AudioSource heatSinkAud;

	public int variation;

	public GameObject bullet;

	public GameObject grenade;

	public float spread;

	private Animator anim;

	public bool gunReady = true;

	public Transform shootPoint;

	public GameObject muzzleFlash;

	private ParticleSystem[] parts;

	private bool charging;

	private AudioSource chargeSound;

	private float chargeAmount;

	public GameObject warningFlash;

	private void Start()
	{
		gunAud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		parts = GetComponentsInChildren<ParticleSystem>();
		heatSinkAud = shootPoint.GetComponent<AudioSource>();
		chargeSound = base.transform.GetChild(0).GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (charging)
		{
			chargeAmount = Mathf.MoveTowards(chargeAmount, 1f, Time.deltaTime * 2f);
			chargeSound.pitch = chargeAmount;
		}
	}

	public void Fire()
	{
		gunReady = false;
		int num = 12;
		anim.SetTrigger("Shoot");
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(bullet, shootPoint.transform.position, shootPoint.transform.rotation);
			if (i > 0)
			{
				gameObject.transform.Rotate(Random.Range(0f - spread, spread), Random.Range(0f - spread, spread), Random.Range(0f - spread, spread));
			}
		}
		gunAud.clip = shootSound;
		gunAud.volume = 0.35f;
		gunAud.panStereo = 0f;
		gunAud.pitch = Random.Range(0.95f, 1.05f);
		gunAud.Play();
		Object.Instantiate(muzzleFlash, shootPoint.transform.position, shootPoint.transform.rotation);
	}

	public void AltFire()
	{
		gunReady = false;
		GameObject obj = Object.Instantiate(grenade, shootPoint.transform.position, Random.rotation);
		obj.GetComponent<Rigidbody>().AddForce(shootPoint.transform.forward * 70f, ForceMode.VelocityChange);
		Grenade componentInChildren = obj.GetComponentInChildren<Grenade>();
		if (componentInChildren != null)
		{
			componentInChildren.enemy = true;
		}
		anim.SetTrigger("Secondary Fire");
		gunAud.clip = shootSound;
		gunAud.volume = 0.35f;
		gunAud.panStereo = 0f;
		gunAud.pitch = Random.Range(0.75f, 0.85f);
		gunAud.Play();
		Object.Instantiate(muzzleFlash, shootPoint.transform.position, shootPoint.transform.rotation);
		charging = false;
		chargeAmount = 0f;
		chargeSound.pitch = 0f;
	}

	public void PrepareFire()
	{
		if (heatSinkAud == null)
		{
			heatSinkAud = shootPoint.GetComponent<AudioSource>();
		}
		heatSinkAud.Play();
		Object.Instantiate(warningFlash, shootPoint.transform.position, shootPoint.transform.rotation);
	}

	public void PrepareAltFire()
	{
		charging = true;
		chargeAmount = 0f;
		chargeSound.pitch = 0f;
	}

	public void ReleaseHeat()
	{
		ParticleSystem[] array = parts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void ClickSound()
	{
		gunAud.clip = clickSound;
		gunAud.volume = 0.5f;
		gunAud.pitch = Random.Range(0.95f, 1.05f);
		gunAud.Play();
	}

	public void ReadyGun()
	{
		gunReady = true;
	}

	public void Smack()
	{
		gunAud.clip = smackSound;
		gunAud.volume = 0.75f;
		gunAud.pitch = Random.Range(2f, 2.2f);
		gunAud.Play();
	}
}
