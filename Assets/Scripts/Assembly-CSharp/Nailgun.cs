using UnityEngine;
using UnityEngine.UI;

public class Nailgun : MonoBehaviour
{
	private InputManager inman;

	public int variation;

	public GameObject[] shootPoints;

	private Spin[] barrels;

	private float spinSpeed;

	private int barrelNum;

	private Light[] barrelLights;

	private Material barrelHeat;

	private float heatUp;

	private bool burnOut;

	public GameObject muzzleFlash;

	public GameObject muzzleFlash2;

	public float fireRate;

	private float currentFireRate;

	private float fireCooldown;

	private AudioSource aud;

	private AudioSource barrelAud;

	public GameObject nail;

	public GameObject heatedNail;

	public GameObject magnetNail;

	private CameraController cc;

	public float spread;

	private float currentSpread;

	private int burstAmount;

	private Animator anim;

	private bool canShoot;

	private NewMovement nm;

	private float harpoonCharge = 1f;

	private Slider heatSlider;

	private Image sliderBg;

	public Color emptyColor;

	public Color fullColor;

	private float heatSinks = 2f;

	private float heatSinkFill = 2f;

	public Image[] heatSinkImages;

	private Color sinkColor;

	private ParticleSystem heatSteam;

	private AudioSource heatSteamAud;

	private float heatCharge = 1f;

	private WeaponCharges wc;

	private GunControl gc;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		barrels = GetComponentsInChildren<Spin>();
		barrelLights = barrels[0].transform.parent.GetComponentsInChildren<Light>();
		barrelHeat = barrelLights[0].transform.parent.GetComponent<MeshRenderer>().sharedMaterial;
		barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, 0f);
		barrelAud = barrels[0].GetComponent<AudioSource>();
		aud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		nm = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
		heatSlider = GetComponentInChildren<Slider>();
		sliderBg = heatSlider.GetComponentInParent<Image>();
		currentFireRate = fireRate;
		if (heatSinkImages.Length != 0)
		{
			sinkColor = heatSinkImages[0].color;
		}
		heatSteam = GetComponentInChildren<ParticleSystem>();
		heatSteamAud = heatSteam.GetComponent<AudioSource>();
		if (wc == null)
		{
			wc = Object.FindObjectOfType<WeaponCharges>();
		}
		gc = GetComponentInParent<GunControl>();
	}

	private void OnDisable()
	{
		canShoot = false;
		harpoonCharge = 1f;
		if (variation == 0)
		{
			wc.nai0charge = heatSinks;
			wc.nai0heatUp = heatUp;
		}
		else if (variation == 1)
		{
			wc.nai1heatUp = heatUp;
		}
	}

	private void OnEnable()
	{
		if (wc == null)
		{
			wc = Object.FindObjectOfType<WeaponCharges>();
		}
		if (variation == 0)
		{
			if (heatSinks < 2f)
			{
				if (wc.nai0charge == 2f)
				{
					heatSinks = 1.99f;
				}
				else
				{
					heatSinks = wc.nai0charge;
				}
				heatSinkFill = heatSinks;
			}
			heatUp = wc.nai0heatUp;
		}
		else if (variation == 1)
		{
			heatUp = wc.nai1heatUp;
		}
	}

	private void Update()
	{
		if (burnOut || heatSinks < 1f)
		{
			heatUp = Mathf.MoveTowards(heatUp, 0f, Time.deltaTime);
			if (burnOut && heatUp <= 0f)
			{
				burnOut = false;
				heatSteam.Stop();
				heatSteamAud.Stop();
			}
		}
		else if (canShoot && Input.GetKey(inman.Inputs["Fire1"]) && heatUp < 1f && gc.activated)
		{
			heatUp = Mathf.MoveTowards(heatUp, 1f, Time.deltaTime);
		}
		else if (heatUp > 0f && (!canShoot || !Input.GetKey(inman.Inputs["Fire1"])))
		{
			heatUp = Mathf.MoveTowards(heatUp, 0f, Time.deltaTime * 0.3f);
		}
		if (heatSlider.value != heatUp)
		{
			heatSlider.value = heatUp;
			sliderBg.color = Color.Lerp(emptyColor, fullColor, heatUp);
			if (heatUp <= 0f && heatSinks < 1f)
			{
				sliderBg.color = new Color(0f, 0f, 0f, 0f);
			}
		}
		else if (heatUp == 0f && heatSinks >= 1f && sliderBg.color.a == 0f)
		{
			sliderBg.color = emptyColor;
		}
		if (canShoot && Input.GetKey(inman.Inputs["Fire1"]) && gc.activated)
		{
			spinSpeed = 250f + heatUp * 2250f;
			if (!burnOut)
			{
				anim.SetLayerWeight(1, 0.9f - heatUp * 0.9f);
			}
			else
			{
				anim.SetLayerWeight(1, 0f);
			}
		}
		else
		{
			spinSpeed = Mathf.MoveTowards(spinSpeed, 0f, Time.deltaTime * 1000f);
			anim.SetLayerWeight(1, 0f);
		}
		if (burnOut)
		{
			currentFireRate = fireRate - 2.5f;
		}
		else if (heatSinks >= 1f)
		{
			currentFireRate = fireRate + 3.5f - heatUp * 3.5f;
		}
		else
		{
			currentFireRate = fireRate + 10f;
		}
		barrelAud.pitch = spinSpeed / 1500f * 2f;
		Spin[] array = barrels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].speed = spinSpeed;
		}
		if (heatUp > 0f)
		{
			Light[] array2 = barrelLights;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].intensity = heatUp * 10f;
			}
			barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, heatUp * 0.25f);
		}
		else
		{
			Light[] array2 = barrelLights;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].intensity = 0f;
			}
			barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, 0f);
		}
		if (canShoot && (Input.GetKey(inman.Inputs["Fire1"]) || burnOut) && gc.activated && fireCooldown == 0f)
		{
			Shoot();
		}
		if (canShoot && !burnOut && heatSinks >= 1f && heatUp >= 0.1f && variation == 0 && Input.GetKeyDown(inman.Inputs["Fire2"]) && gc.activated)
		{
			burnOut = true;
			heatSinks -= 1f;
			heatSteam.Play();
			heatSteamAud.Play();
		}
		if (canShoot && fireCooldown == 0f && Input.GetKeyDown(inman.Inputs["Fire2"]) && variation == 1)
		{
			ShootMagnet();
		}
		if (heatSinks < 2f && !Input.GetKey(inman.Inputs["Fire1"]))
		{
			heatSinks = Mathf.MoveTowards(heatSinks, 2f, Time.deltaTime * 0.125f);
		}
		if (heatSinkFill != heatSinks)
		{
			heatSinkFill = Mathf.MoveTowards(heatSinkFill, heatSinks, Time.deltaTime * (Mathf.Abs((heatSinks - heatSinkFill) * 20f) + 1f));
			for (int j = 0; j < heatSinkImages.Length; j++)
			{
				if (heatSinkFill > (float)j)
				{
					heatSinkImages[j].fillAmount = heatSinkFill - (float)j;
					if (heatSinkFill >= (float)(j + 1) && heatSinkImages[j].color != sinkColor)
					{
						aud.pitch = (float)j * 0.5f + 1f;
						aud.Play();
						heatSinkImages[j].color = sinkColor;
					}
					else if (heatSinkFill < (float)(j + 1))
					{
						heatSinkImages[j].color = emptyColor;
					}
				}
				else
				{
					heatSinkImages[j].fillAmount = 0f;
				}
			}
		}
		if (fireCooldown > 0f)
		{
			fireCooldown = Mathf.MoveTowards(fireCooldown, 0f, Time.deltaTime * 100f);
		}
	}

	private void Shoot()
	{
		fireCooldown = currentFireRate;
		anim.SetTrigger("Shoot");
		barrelNum++;
		if (barrelNum >= shootPoints.Length)
		{
			barrelNum = 0;
		}
		GameObject gameObject = ((!burnOut) ? Object.Instantiate(muzzleFlash, shootPoints[barrelNum].transform) : Object.Instantiate(muzzleFlash2, shootPoints[barrelNum].transform));
		if (burnOut)
		{
			gameObject.GetComponent<AudioSource>().volume = 0.65f;
			gameObject.GetComponent<AudioSource>().pitch = 2f;
			currentSpread = spread * 2f;
		}
		else
		{
			if (heatSinks < 1f)
			{
				gameObject.GetComponent<AudioSource>().pitch = 0.25f;
				gameObject.GetComponent<AudioSource>().volume = 0.25f;
			}
			else
			{
				gameObject.GetComponent<AudioSource>().volume = 0.45f + heatUp * 0.2f;
			}
			currentSpread = spread;
		}
		GameObject gameObject2 = ((!burnOut) ? Object.Instantiate(nail, cc.transform.position + cc.transform.forward, base.transform.rotation) : Object.Instantiate(heatedNail, cc.transform.position + cc.transform.forward, base.transform.rotation));
		gameObject2.transform.forward = base.transform.forward * -1f;
		gameObject2.transform.Rotate(Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f));
		gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * -100f, ForceMode.VelocityChange);
		gameObject2.GetComponent<Nail>().weaponType = "nailgun" + variation;
		if (!burnOut)
		{
			cc.CameraShake(0.1f);
		}
		else
		{
			cc.CameraShake(0.35f);
		}
	}

	public void BurstFire()
	{
		burstAmount--;
		barrelNum++;
		if (barrelNum >= shootPoints.Length)
		{
			barrelNum = 0;
		}
		Object.Instantiate(muzzleFlash2, shootPoints[barrelNum].transform);
		currentSpread = spread;
		GameObject gameObject = Object.Instantiate(heatedNail, base.transform.position + base.transform.forward, base.transform.rotation);
		gameObject.transform.forward = base.transform.forward * -1f;
		gameObject.transform.Rotate(Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f));
		gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * -100f, ForceMode.VelocityChange);
		gameObject.GetComponent<Nail>().weaponType = "nailgun" + variation;
		cc.CameraShake(0.5f);
		if (burstAmount > 0)
		{
			Invoke("BurstFire", 0.03f);
		}
	}

	public void ShootMagnet()
	{
		GameObject gameObject = Object.Instantiate(magnetNail, cc.transform.position + cc.transform.forward, base.transform.rotation);
		gameObject.transform.forward = base.transform.forward;
		gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 100f, ForceMode.VelocityChange);
		anim.SetTrigger("Shoot");
		fireCooldown = 10f;
	}

	public void CanShoot()
	{
		canShoot = true;
	}

	private void MaxCharge()
	{
		if (variation == 0 && heatSinks < 2f)
		{
			heatSinks = 1.99f;
			heatSinkFill = heatSinks;
		}
	}
}
