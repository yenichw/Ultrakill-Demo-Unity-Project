using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
	private InputManager inman;

	private AudioSource gunAud;

	public AudioClip shootSound;

	public AudioClip shootSound2;

	public AudioClip clickSound;

	public AudioClip clickChargeSound;

	public AudioClip smackSound;

	public AudioClip pump1sound;

	public AudioClip pump2sound;

	public int variation;

	public GameObject bullet;

	public GameObject grenade;

	public float spread;

	private bool smallSpread;

	private Animator anim;

	private GameObject cam;

	private CameraController cc;

	private GunControl gc;

	private bool gunReady;

	public Transform[] shootPoints;

	public GameObject muzzleFlash;

	public SkinnedMeshRenderer heatSinkSMR;

	private Color tempColor;

	private bool releasingHeat;

	private ParticleSystem[] parts;

	private AudioSource heatSinkAud;

	public LayerMask shotgunZoneLayerMask;

	private RaycastHit[] rhits;

	private bool charging;

	private float grenadeForce;

	private Vector3 grenadeVector;

	private Slider chargeSlider;

	private Image sliderFill;

	private Color origSliderColor;

	public Color[] sliderColors;

	public GameObject grenadeSoundBubble;

	public GameObject chargeSoundBubble;

	private AudioSource tempChargeSound;

	private int primaryCharge;

	private bool cockedBack;

	public GameObject explosion;

	public GameObject pumpChargeSound;

	public GameObject warningBeep;

	private float timeToBeep;

	private WeaponPos wpos;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		gunAud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		cam = Camera.main.gameObject;
		cc = cam.GetComponent<CameraController>();
		gc = GetComponentInParent<GunControl>();
		tempColor = heatSinkSMR.sharedMaterials[2].GetColor("_TintColor");
		parts = GetComponentsInChildren<ParticleSystem>();
		heatSinkAud = heatSinkSMR.GetComponent<AudioSource>();
		chargeSlider = GetComponentInChildren<Slider>();
		sliderFill = chargeSlider.GetComponentInChildren<Image>();
		origSliderColor = sliderFill.color;
		if (variation == 0)
		{
			chargeSlider.value = chargeSlider.maxValue;
		}
		else if (variation == 1)
		{
			chargeSlider.value = 0f;
		}
		wpos = GetComponent<WeaponPos>();
	}

	private void OnDisable()
	{
		if (anim == null)
		{
			anim = GetComponentInChildren<Animator>();
		}
		anim.StopPlayback();
		gunReady = false;
		if (sliderFill != null)
		{
			sliderFill.color = origSliderColor;
		}
		if (chargeSlider == null)
		{
			chargeSlider = GetComponentInChildren<Slider>();
		}
		if (variation == 0)
		{
			chargeSlider.value = chargeSlider.maxValue;
		}
		else if (variation == 1)
		{
			chargeSlider.value = 0f;
		}
		if (sliderFill == null)
		{
			sliderFill = chargeSlider.GetComponentInChildren<Image>();
		}
		primaryCharge = 0;
		charging = false;
		grenadeForce = 0f;
		if (tempChargeSound != null)
		{
			Object.Destroy(tempChargeSound);
		}
	}

	private void Update()
	{
		if (Input.GetKey(inman.Inputs["Fire1"]) && gunReady && gc.activated && !charging)
		{
			Shoot();
		}
		if (Input.GetKey(inman.Inputs["Fire2"]) && variation == 1 && gunReady && gc.activated)
		{
			gunReady = false;
			anim.SetTrigger("Pump");
			if (primaryCharge < 3)
			{
				primaryCharge++;
				chargeSlider.value += 20f;
				sliderFill.color = Color.Lerp(sliderColors[0], sliderColors[1], chargeSlider.value / 60f + 0.33f);
			}
		}
		if (timeToBeep != 0f)
		{
			timeToBeep = Mathf.MoveTowards(timeToBeep, 0f, Time.deltaTime * 5f);
		}
		if (variation == 1 && primaryCharge == 3)
		{
			if (timeToBeep == 0f)
			{
				timeToBeep = 1f;
				Object.Instantiate(warningBeep);
				sliderFill.color = Color.red;
			}
			else if (timeToBeep < 0.5f)
			{
				sliderFill.color = Color.black;
			}
		}
		if (Input.GetKey(inman.Inputs["Fire2"]) && variation == 0 && gunReady && gc.activated)
		{
			charging = true;
			if (grenadeForce < 60f)
			{
				grenadeForce = Mathf.MoveTowards(grenadeForce, 60f, Time.deltaTime * 60f);
			}
			grenadeVector = new Vector3(cam.transform.forward.x, cam.transform.forward.y + grenadeForce * 0.002f, cam.transform.forward.z);
			chargeSlider.value = grenadeForce;
			sliderFill.color = Color.Lerp(sliderColors[0], sliderColors[1], grenadeForce / 60f);
			base.transform.localPosition = new Vector3(wpos.currentDefault.x + Random.Range(grenadeForce / 3000f * -1f, grenadeForce / 3000f), wpos.currentDefault.y + Random.Range(grenadeForce / 3000f * -1f, grenadeForce / 3000f), wpos.currentDefault.z + Random.Range(grenadeForce / 3000f * -1f, grenadeForce / 3000f));
			if (tempChargeSound == null)
			{
				GameObject gameObject = Object.Instantiate(chargeSoundBubble);
				tempChargeSound = gameObject.GetComponent<AudioSource>();
			}
			tempChargeSound.pitch = grenadeForce / 60f;
		}
		if (Input.GetKeyUp(inman.Inputs["Fire2"]) && variation == 0 && gunReady && gc.activated && charging)
		{
			ShootSinks();
			charging = false;
			grenadeForce = 0f;
			Object.Destroy(tempChargeSound.gameObject);
		}
		if (releasingHeat)
		{
			tempColor.a -= Time.deltaTime * 2.5f;
			heatSinkSMR.sharedMaterials[2].SetColor("_TintColor", tempColor);
		}
	}

	private void Shoot()
	{
		gunReady = false;
		int num = 12;
		if (variation == 1)
		{
			switch (primaryCharge)
			{
			case 0:
				num = 10;
				gunAud.pitch = Random.Range(1.15f, 1.25f);
				break;
			case 1:
				num = 16;
				gunAud.pitch = Random.Range(0.95f, 1.05f);
				break;
			case 2:
				num = 24;
				gunAud.pitch = Random.Range(0.75f, 0.85f);
				break;
			case 3:
				num = 0;
				gunAud.pitch = Random.Range(0.75f, 0.85f);
				break;
			}
		}
		rhits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 4f, shotgunZoneLayerMask);
		Debug.DrawRay(cam.transform.position, cam.transform.forward * 4f, Color.red, 5f);
		if (rhits.Length != 0)
		{
			RaycastHit[] array = rhits;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.collider.gameObject.tag == "Body")
				{
					EnemyIdentifier eid = raycastHit.collider.GetComponentInParent<EnemyIdentifierIdentifier>().eid;
					if (!eid.dead && anim.GetCurrentAnimatorStateInfo(0).IsName("Equip"))
					{
						cc.GetComponentInChildren<StyleHUD>().AddPoints(50, "<color=cyan>QUICKDRAW</color>");
					}
					eid.hitter = "shotgunzone";
					if (!eid.hitterWeapons.Contains("shotgun" + variation))
					{
						eid.hitterWeapons.Add("shotgun" + variation);
					}
					eid.DeliverDamage(raycastHit.collider.gameObject, (eid.transform.position - base.transform.position).normalized * 10000f, raycastHit.point, 4f, tryForExplode: false, 0f);
				}
			}
		}
		if (variation != 1 || primaryCharge != 3)
		{
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = Object.Instantiate(bullet, cam.transform.position, cam.transform.rotation);
				gameObject.GetComponent<Projectile>().weaponType = "shotgun" + variation;
				if (variation == 1)
				{
					switch (primaryCharge)
					{
					case 0:
						gameObject.transform.Rotate(Random.Range((0f - spread) / 1.5f, spread / 1.5f), Random.Range((0f - spread) / 1.5f, spread / 1.5f), Random.Range((0f - spread) / 1.5f, spread / 1.5f));
						break;
					case 1:
						gameObject.transform.Rotate(Random.Range(0f - spread, spread), Random.Range(0f - spread, spread), Random.Range(0f - spread, spread));
						break;
					case 2:
						gameObject.transform.Rotate(Random.Range((0f - spread) * 2f, spread * 2f), Random.Range((0f - spread) * 2f, spread * 2f), Random.Range((0f - spread) * 2f, spread * 2f));
						break;
					}
				}
				else
				{
					gameObject.transform.Rotate(Random.Range(0f - spread, spread), Random.Range(0f - spread, spread), Random.Range(0f - spread, spread));
				}
			}
		}
		else
		{
			Explosion[] componentsInChildren = Object.Instantiate(explosion, cam.transform.position + cam.transform.forward, cam.transform.rotation).GetComponentsInChildren<Explosion>();
			foreach (Explosion obj in componentsInChildren)
			{
				obj.maxSize *= 1.5f;
				obj.damage = 50;
			}
		}
		if (variation != 1)
		{
			gunAud.pitch = Random.Range(0.95f, 1.05f);
		}
		gunAud.clip = shootSound;
		gunAud.volume = 0.45f;
		gunAud.panStereo = 0f;
		gunAud.Play();
		cc.CameraShake(1f);
		if (variation == 1)
		{
			anim.SetTrigger("PumpFire");
		}
		else
		{
			anim.SetTrigger("Fire");
		}
		Transform[] array2 = shootPoints;
		foreach (Transform transform in array2)
		{
			Object.Instantiate(muzzleFlash, transform.transform.position, transform.transform.rotation);
		}
		releasingHeat = false;
		tempColor.a = 1f;
		heatSinkSMR.sharedMaterials[2].SetColor("_TintColor", tempColor);
		if (variation == 1)
		{
			primaryCharge = 0;
			chargeSlider.value = 0f;
			sliderFill.color = origSliderColor;
		}
	}

	private void ShootSinks()
	{
		gunReady = false;
		base.transform.localPosition = wpos.currentDefault;
		Transform[] array = shootPoints;
		for (int i = 0; i < array.Length; i++)
		{
			_ = array[i];
			Object.Instantiate(grenade, cam.transform.position + cam.transform.forward * 0.5f, Random.rotation).GetComponent<Rigidbody>().AddForce(grenadeVector * (grenadeForce + 10f), ForceMode.VelocityChange);
		}
		Object.Instantiate(grenadeSoundBubble).GetComponent<AudioSource>().volume = 0.45f * Mathf.Sqrt(Mathf.Pow(1f, 2f) - Mathf.Pow(grenadeForce, 2f) / Mathf.Pow(60f, 2f));
		anim.SetTrigger("Secondary Fire");
		gunAud.clip = shootSound;
		gunAud.volume = 0.45f * (grenadeForce / 60f);
		gunAud.panStereo = 0f;
		gunAud.pitch = Random.Range(0.75f, 0.85f);
		gunAud.Play();
		cc.CameraShake(1f);
		array = shootPoints;
		foreach (Transform transform in array)
		{
			Object.Instantiate(muzzleFlash, transform.transform.position, transform.transform.rotation);
		}
		releasingHeat = false;
		tempColor.a = 0f;
		heatSinkSMR.sharedMaterials[2].SetColor("_TintColor", tempColor);
	}

	public void ReleaseHeat()
	{
		releasingHeat = true;
		ParticleSystem[] array = parts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		heatSinkAud.Play();
	}

	public void ClickSound()
	{
		if (sliderFill.color != origSliderColor)
		{
			gunAud.clip = clickChargeSound;
		}
		else
		{
			gunAud.clip = clickSound;
		}
		gunAud.volume = 0.5f;
		gunAud.pitch = Random.Range(0.95f, 1.05f);
		gunAud.panStereo = 0.1f;
		gunAud.Play();
	}

	public void ReadyGun()
	{
		gunReady = true;
		if (variation == 0)
		{
			chargeSlider.value = chargeSlider.maxValue;
			sliderFill.color = origSliderColor;
		}
	}

	public void Smack()
	{
		gunAud.clip = smackSound;
		gunAud.volume = 0.75f;
		gunAud.pitch = Random.Range(2f, 2.2f);
		gunAud.panStereo = 0.1f;
		gunAud.Play();
	}

	public void SkipShoot()
	{
		anim.ResetTrigger("Fire");
		anim.Play("FireWithReload", -1, 0.05f);
	}

	public void Pump1Sound()
	{
		AudioSource component = Object.Instantiate(grenadeSoundBubble).GetComponent<AudioSource>();
		component.pitch = Random.Range(0.95f, 1.05f);
		component.clip = pump1sound;
		component.volume = 1f;
		component.panStereo = 0.1f;
		component.Play();
		AudioSource component2 = Object.Instantiate(pumpChargeSound).GetComponent<AudioSource>();
		float num = primaryCharge;
		component2.pitch = 1f + num / 5f;
		component2.Play();
	}

	public void Pump2Sound()
	{
		AudioSource component = Object.Instantiate(grenadeSoundBubble).GetComponent<AudioSource>();
		component.pitch = Random.Range(0.95f, 1.05f);
		component.clip = pump2sound;
		component.volume = 1f;
		component.panStereo = 0.1f;
		component.Play();
	}
}
