using UnityEngine;
using UnityEngine.UI;

public class Revolver : MonoBehaviour
{
	private InputManager inman;

	public int gunVariation;

	public Transform kickBackPos;

	private AudioSource gunAud;

	private AudioSource superGunAud;

	public AudioClip[] gunShots;

	public AudioClip[] superGunShots;

	private int currentGunShot;

	public GameObject gunBarrel;

	private bool gunReady;

	private bool shootReady = true;

	private bool pierceReady = true;

	public float shootCharge;

	public float pierceCharge;

	public LayerMask pierceLayerMask;

	public LayerMask enemyLayerMask;

	private bool chargingPierce;

	public float pierceShotCharge;

	public Vector3 shotHitPoint;

	public GameObject revolverBeam;

	public GameObject revolverBeamSuper;

	public RaycastHit hit;

	public RaycastHit[] allHits;

	private int currentHit;

	private int currentHitMultiplier;

	public float recoilFOV;

	public GameObject chargeEffect;

	private AudioSource ceaud;

	private Light celight;

	private GameObject camObj;

	private Camera cam;

	private CameraController cc;

	private Vector3 tempCamPos;

	public Vector3 beamReflectPos;

	private GameObject beamDirectionSetter;

	public MeshRenderer screenMR;

	public Material batteryFull;

	public Material batteryMid;

	public Material batteryLow;

	public Material[] batteryCharges;

	private AudioSource screenAud;

	public AudioClip chargedSound;

	public AudioClip chargingSound;

	private int bodiesPierced;

	public GameObject blood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject dripBlood;

	private Enemy enemy;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private GameObject limb;

	private Transform firstChild;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject[] giblet;

	public GameObject brainChunk;

	public GameObject jawHalf;

	private int bulletForce;

	private bool slowMo;

	private bool timeStopped;

	private float untilTimeResume;

	private int enemiesLeftToHit;

	private int enemiesPierced;

	private RaycastHit subHit;

	private float damageMultiplier = 1f;

	private SecondaryRevolver secRev;

	private bool twirling;

	private AudioClip chargeEffectSound;

	public AudioClip twirlSound;

	public bool twirlRecovery;

	private GameObject currentDrip;

	public GameObject coin;

	public LayerMask ignoreEnemyTrigger;

	private Turn cylinder;

	private SwitchMaterial rimLight;

	public GunControl gc;

	private Animator anim;

	private Punch punch;

	private NewMovement nmov;

	private WeaponPos wpos;

	public Image[] coinPanels;

	private float coinCharge = 400f;

	private Color panelColor;

	private WeaponCharges wc;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		gunReady = false;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		camObj = cam.gameObject;
		cc = camObj.GetComponent<CameraController>();
		nmov = cc.player.GetComponent<NewMovement>();
		shootCharge = 0f;
		pierceShotCharge = 0f;
		pierceCharge = 100f;
		pierceReady = false;
		shootReady = false;
		gunAud = GetComponent<AudioSource>();
		kickBackPos = base.transform.parent.GetChild(0);
		superGunAud = kickBackPos.GetComponent<AudioSource>();
		if (gunVariation == 0)
		{
			screenAud = screenMR.gameObject.GetComponent<AudioSource>();
		}
		else if (gunVariation == 1)
		{
			screenAud = GetComponentInChildren<Canvas>().GetComponent<AudioSource>();
		}
		secRev = base.transform.parent.GetComponentInChildren<SecondaryRevolver>();
		chargeEffect = gunBarrel.transform.GetChild(0).gameObject;
		ceaud = chargeEffect.GetComponent<AudioSource>();
		celight = chargeEffect.GetComponent<Light>();
		chargeEffectSound = ceaud.clip;
		if (gunVariation == 0)
		{
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			if (gunVariation == 0)
			{
				screenAud.pitch = 1f;
			}
			else if (gunVariation == 1)
			{
				screenAud.pitch = 1.25f;
			}
			screenAud.volume = 0.25f;
			screenAud.Play();
		}
		if (gunVariation == 2)
		{
			secRev.gameObject.SetActive(value: true);
			secRev.enabled = true;
		}
		cylinder = GetComponentInChildren<Turn>();
		gc = GetComponentInParent<GunControl>();
		beamDirectionSetter = new GameObject();
		anim = GetComponentInChildren<Animator>();
		if (gunVariation == 1)
		{
			panelColor = coinPanels[0].color;
		}
		wc = Object.FindObjectOfType<WeaponCharges>();
		wpos = GetComponent<WeaponPos>();
	}

	private void OnDisable()
	{
		if (wc == null)
		{
			wc = Object.FindObjectOfType<WeaponCharges>();
		}
		if (gunVariation == 0)
		{
			wc.rev0charge = pierceCharge;
		}
		else if (gunVariation == 1)
		{
			wc.rev1charge = coinCharge;
		}
		pierceShotCharge = 0f;
		gunReady = false;
	}

	private void OnEnable()
	{
		if (wc == null)
		{
			wc = Object.FindObjectOfType<WeaponCharges>();
		}
		if (gunVariation == 0)
		{
			pierceCharge = wc.rev0charge;
		}
		else if (gunVariation == 1)
		{
			coinCharge = wc.rev1charge;
			for (int i = 0; i < coinPanels.Length; i++)
			{
				coinPanels[i].fillAmount = coinCharge / 100f - (float)i;
				if (coinPanels[i].fillAmount < 1f)
				{
					coinPanels[i].color = Color.red;
				}
				else if (coinPanels[i].color == Color.red)
				{
					screenAud.pitch = 1f + (float)i / 2f;
					screenAud.Play();
					coinPanels[i].color = panelColor;
				}
			}
		}
		gunReady = false;
	}

	private void Update()
	{
		if (gunVariation == 2)
		{
			pierceCharge = shootCharge;
		}
		if (!shootReady)
		{
			if (gunVariation != 0)
			{
				if (shootCharge + 175f * Time.deltaTime < 100f)
				{
					shootCharge += 175f * Time.deltaTime;
				}
				else
				{
					shootCharge = 100f;
					shootReady = true;
				}
			}
			else if (shootCharge + 200f * Time.deltaTime < 100f)
			{
				shootCharge += 200f * Time.deltaTime;
			}
			else
			{
				shootCharge = 100f;
				shootReady = true;
			}
		}
		if (!pierceReady)
		{
			if (gunVariation == 0)
			{
				if (pierceCharge + 40f * Time.deltaTime < 100f)
				{
					pierceCharge += 40f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
					screenAud.clip = chargedSound;
					screenAud.loop = false;
					screenAud.volume = 0.35f;
					screenAud.pitch = Random.Range(1f, 1.1f);
					screenAud.Play();
				}
				if (pierceCharge < 50f)
				{
					screenMR.material = batteryLow;
				}
				else if (pierceCharge < 100f)
				{
					screenMR.material = batteryMid;
				}
				else
				{
					screenMR.material = batteryFull;
				}
			}
			else if (gunVariation == 1)
			{
				if (pierceCharge + 480f * Time.deltaTime < 100f)
				{
					pierceCharge += 480f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
				}
			}
		}
		else if (pierceShotCharge != 0f)
		{
			if (pierceShotCharge < 50f)
			{
				screenMR.material = batteryCharges[0];
			}
			else if (pierceShotCharge < 100f)
			{
				screenMR.material = batteryCharges[1];
			}
			else
			{
				screenMR.material = batteryCharges[2];
			}
			base.transform.localPosition = new Vector3(wpos.currentDefault.x + pierceShotCharge / 250f * Random.Range(-0.05f, 0.05f), wpos.currentDefault.y + pierceShotCharge / 250f * Random.Range(-0.05f, 0.05f), wpos.currentDefault.z + pierceShotCharge / 250f * Random.Range(-0.05f, 0.05f));
			cylinder.spinSpeed = pierceShotCharge;
		}
		else
		{
			if (gunVariation == 0 && screenMR.material != batteryFull)
			{
				screenMR.material = batteryFull;
			}
			if (cylinder.spinSpeed != 0f)
			{
				cylinder.spinSpeed = 0f;
			}
		}
		if (gc.activated)
		{
			if (gunVariation == 0 && gunReady)
			{
				if ((Input.GetKeyUp(inman.Inputs["Fire2"]) || Input.GetKey(inman.Inputs["Fire1"])) && shootReady && pierceShotCharge == 100f)
				{
					Shoot(2);
					pierceShotCharge = 0f;
				}
				else if (Input.GetKey(inman.Inputs["Fire1"]) && shootReady && !chargingPierce)
				{
					Shoot(1);
				}
				else if (Input.GetKey(inman.Inputs["Fire2"]) && shootReady && pierceReady)
				{
					chargingPierce = true;
					if (pierceShotCharge + 175f * Time.deltaTime < 100f)
					{
						pierceShotCharge += 175f * Time.deltaTime;
					}
					else
					{
						pierceShotCharge = 100f;
					}
				}
				else
				{
					chargingPierce = false;
					if (pierceShotCharge - 175f * Time.deltaTime > 0f)
					{
						pierceShotCharge -= 175f * Time.deltaTime;
					}
					else
					{
						pierceShotCharge = 0f;
					}
				}
			}
			else if (gunVariation == 1)
			{
				if (Input.GetKeyDown(inman.Inputs["Fire2"]) && pierceReady && coinCharge >= 100f)
				{
					coinCharge -= 100f;
					if (punch == null || !punch.gameObject.activeInHierarchy)
					{
						punch = cc.GetComponentInChildren<Punch>();
					}
					punch.CoinFlip();
					Object.Instantiate(coin, camObj.transform.position + camObj.transform.up * -0.5f, camObj.transform.rotation).GetComponent<Rigidbody>().AddForce(camObj.transform.forward * 20f + Vector3.up * 15f + cc.player.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
					pierceCharge = 0f;
					pierceReady = false;
				}
				else if (gunReady && Input.GetKey(inman.Inputs["Fire1"]) && shootReady)
				{
					Shoot(1);
					twirling = false;
					if (ceaud.volume != 0f)
					{
						ceaud.volume = 0f;
					}
					twirlRecovery = false;
				}
				else
				{
					twirling = false;
				}
			}
			else if (gunVariation == 2 && gunReady && Input.GetKeyDown(inman.Inputs["Fire2"]) && shootReady)
			{
				Shoot(1);
			}
		}
		if (pierceShotCharge == 0f && celight.enabled)
		{
			celight.enabled = false;
		}
		else if (pierceShotCharge != 0f)
		{
			celight.enabled = true;
			celight.range = pierceShotCharge * 0.01f;
		}
		chargeEffect.transform.localScale = Vector3.one * pierceShotCharge * 0.02f;
		if (gunVariation == 0)
		{
			ceaud.volume = 0.25f + pierceShotCharge * 0.005f;
			ceaud.pitch = pierceShotCharge * 0.005f;
		}
		if (gunVariation != 1 || coinCharge == 400f)
		{
			return;
		}
		coinCharge = Mathf.MoveTowards(coinCharge, 400f, Time.deltaTime * 25f);
		for (int i = 0; i < coinPanels.Length; i++)
		{
			coinPanels[i].fillAmount = coinCharge / 100f - (float)i;
			if (coinPanels[i].fillAmount < 1f)
			{
				coinPanels[i].color = Color.red;
			}
			else if (coinPanels[i].color == Color.red)
			{
				screenAud.pitch = 1f + (float)i / 2f;
				screenAud.Play();
				coinPanels[i].color = panelColor;
			}
		}
	}

	private void Shoot(int shotType)
	{
		cc.StopShake();
		shootReady = false;
		shootCharge = 0f;
		switch (shotType)
		{
		case 1:
		{
			RevolverBeam component2 = Object.Instantiate(revolverBeam, cc.transform.position, cc.transform.rotation).GetComponent<RevolverBeam>();
			component2.alternateStartPoint = gunBarrel.transform.position;
			component2.gunVariation = gunVariation;
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("PickUp"))
			{
				component2.quickDraw = true;
			}
			currentGunShot = Random.Range(0, gunShots.Length);
			gunAud.clip = gunShots[currentGunShot];
			gunAud.volume = 0.55f;
			gunAud.pitch = Random.Range(0.95f, 1.05f);
			gunAud.Play();
			cam.fieldOfView += cc.defaultFov / 40f;
			break;
		}
		case 2:
		{
			RevolverBeam component = Object.Instantiate(revolverBeamSuper, cc.transform.position, cc.transform.rotation).GetComponent<RevolverBeam>();
			component.alternateStartPoint = gunBarrel.transform.position;
			component.gunVariation = gunVariation;
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("PickUp"))
			{
				component.quickDraw = true;
			}
			pierceReady = false;
			pierceCharge = 0f;
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			screenAud.pitch = 1f;
			screenAud.volume = 0.55f;
			screenAud.Play();
			currentGunShot = Random.Range(0, superGunShots.Length);
			superGunAud.clip = superGunShots[currentGunShot];
			superGunAud.volume = 0.5f;
			superGunAud.pitch = Random.Range(0.95f, 1.05f);
			superGunAud.Play();
			cam.fieldOfView += cc.defaultFov / 20f;
			break;
		}
		}
		cylinder.DoTurn();
		anim.SetFloat("RandomChance", Random.Range(0f, 1f));
		if (shotType == 1)
		{
			anim.SetTrigger("Shoot");
		}
		else
		{
			anim.SetTrigger("ChargeShoot");
		}
		gunReady = false;
	}

	private void ReadyToShoot()
	{
		shootReady = true;
	}

	public void Punch()
	{
		gunReady = false;
		anim.SetTrigger("ChargeShoot");
	}

	public void ReadyGun()
	{
		gunReady = true;
	}

	private void MaxCharge()
	{
		if (gunVariation == 0)
		{
			pierceCharge = 100f;
		}
		else
		{
			if (gunVariation != 1)
			{
				return;
			}
			coinCharge = 400f;
			for (int i = 0; i < coinPanels.Length; i++)
			{
				coinPanels[i].fillAmount = coinCharge / 100f - (float)i;
				if (coinPanels[i].fillAmount < 1f)
				{
					coinPanels[i].color = Color.red;
				}
				else if (coinPanels[i].color == Color.red)
				{
					screenAud.pitch = 1f + (float)i / 2f;
					screenAud.Play();
					coinPanels[i].color = panelColor;
				}
			}
		}
	}
}
