using UnityEngine;

public class Punch : MonoBehaviour
{
	private InputManager inman;

	public FistType type;

	private string hitter;

	private float damage;

	private float screenShakeMultiplier;

	private float force;

	private bool tryForExplode;

	private float cooldownCost;

	public bool ready = true;

	[HideInInspector]
	public Animator anim;

	private SkinnedMeshRenderer smr;

	private Revolver rev;

	private AudioSource aud;

	private AudioSource aud2;

	private GameObject camObj;

	private CameraController cc;

	private RaycastHit hit;

	public LayerMask deflectionLayerMask;

	public LayerMask ignoreEnemyTrigger;

	public LayerMask environmentMask;

	private NewMovement nmov;

	private TrailRenderer tr;

	private Light parryLight;

	private GameObject currentDustParticle;

	public GameObject dustParticle;

	private GameObject parryFlash;

	public AudioClip normalHit;

	public AudioClip heavyHit;

	public AudioClip specialHit;

	private StyleHUD shud;

	private StatsManager sman;

	public bool holding;

	public Transform holder;

	public GameObject heldItem;

	private FistControl fc;

	private bool shopping;

	private int shopRequests;

	public GameObject parriedProjectileHitObject;

	private ProjectileParryZone ppz;

	private bool returnToOrigRot;

	public GameObject blastWave;

	private bool holdingInput;

	public GameObject shell;

	public Transform shellEjector;

	private AudioSource ejectorAud;

	public bool crowdReactions;

	private CrowdReactions crorea;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		anim = GetComponent<Animator>();
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		rev = base.transform.parent.parent.GetComponentInChildren<Revolver>();
		camObj = GameObject.FindWithTag("MainCamera");
		cc = camObj.GetComponent<CameraController>();
		aud = GetComponent<AudioSource>();
		aud2 = base.transform.GetChild(2).GetComponent<AudioSource>();
		nmov = GetComponentInParent<NewMovement>();
		tr = GetComponentInChildren<TrailRenderer>();
		parryLight = aud2.GetComponent<Light>();
		parryFlash = Object.FindObjectOfType<HUDOptions>().transform.Find("ParryFlash").gameObject;
		shud = camObj.GetComponentInChildren<StyleHUD>();
		sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		holdingInput = false;
		if (fc == null)
		{
			fc = GetComponentInParent<FistControl>();
		}
		switch (type)
		{
		case FistType.Standard:
			damage = 1f;
			screenShakeMultiplier = 1f;
			force = 25f;
			tryForExplode = false;
			cooldownCost = 2f;
			hitter = "punch";
			break;
		case FistType.Heavy:
			damage = 2.5f;
			screenShakeMultiplier = 2f;
			force = 100f;
			tryForExplode = true;
			cooldownCost = 3f;
			hitter = "heavypunch";
			break;
		}
		crorea = Object.FindObjectOfType<CrowdReactions>();
		if (crorea != null)
		{
			crowdReactions = true;
		}
	}

	private void OnEnable()
	{
		holdingInput = false;
		ReadyToPunch();
		if (fc == null)
		{
			fc = GetComponentInParent<FistControl>();
			anim = GetComponent<Animator>();
		}
		if (fc.heldObject != null)
		{
			heldItem = fc.heldObject;
			heldItem.transform.SetParent(holder, worldPositionStays: true);
			holding = true;
			anim.SetBool("Holding", value: true);
			ItemIdentifier component = fc.heldObject.GetComponent<ItemIdentifier>();
			if (component.reverseTransformSettings)
			{
				heldItem.transform.localPosition = component.putDownPosition;
				heldItem.transform.localScale = component.putDownScale;
				heldItem.transform.localRotation = Quaternion.Euler(component.putDownRotation);
			}
			else
			{
				heldItem.transform.localPosition = Vector3.zero;
				heldItem.transform.localScale = Vector3.one;
				heldItem.transform.localRotation = Quaternion.identity;
			}
		}
	}

	private void OnDisable()
	{
		holding = false;
		anim.SetBool("Holding", value: false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(inman.Inputs["Punch"]) && ready && !shopping && fc.fistCooldown <= 0f && fc.activated)
		{
			fc.weightCooldown += cooldownCost * 0.25f + fc.weightCooldown * cooldownCost * 0.1f;
			fc.fistCooldown += fc.weightCooldown;
			PunchStart();
			holdingInput = true;
		}
		if (holdingInput && Input.GetKeyUp(inman.Inputs["Punch"]))
		{
			holdingInput = false;
		}
		float layerWeight = anim.GetLayerWeight(1);
		if (shopping && layerWeight < 1f)
		{
			anim.SetLayerWeight(1, Mathf.MoveTowards(layerWeight, 1f, Time.deltaTime / 10f + 5f * Time.deltaTime * (1f - layerWeight)));
		}
		else if (!shopping && layerWeight > 0f)
		{
			anim.SetLayerWeight(1, Mathf.MoveTowards(layerWeight, 0f, Time.deltaTime / 10f + 5f * Time.deltaTime * layerWeight));
		}
		if (Input.GetButtonDown("Fire1") && shopping)
		{
			anim.SetTrigger("ShopTap");
		}
		if (returnToOrigRot)
		{
			base.transform.parent.localRotation = Quaternion.RotateTowards(base.transform.parent.localRotation, Quaternion.identity, (Quaternion.Angle(base.transform.parent.localRotation, Quaternion.identity) * 5f + 5f) * Time.deltaTime * 5f);
			if (base.transform.parent.localRotation == Quaternion.identity)
			{
				returnToOrigRot = false;
			}
		}
		if (fc.shopping && !shopping)
		{
			ShopMode();
		}
		else if (!fc.shopping && shopping)
		{
			StopShop();
		}
	}

	private void PunchStart()
	{
		ready = false;
		anim.SetFloat("PunchRandomizer", Random.Range(0f, 1f));
		anim.SetTrigger("Punch");
		aud.pitch = Random.Range(0.9f, 1.1f);
		aud.Play();
		tr.widthMultiplier = 0.5f;
		if (holding)
		{
			GetComponentInChildren<ItemIdentifier>().SendMessage("PunchWith", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ActiveStart()
	{
		returnToOrigRot = false;
		if (type == FistType.Standard)
		{
			if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 4f, deflectionLayerMask) || Physics.BoxCast(camObj.transform.position, Vector3.one * 0.3f, camObj.transform.forward, out hit, camObj.transform.rotation, 4f, deflectionLayerMask))
			{
				if (hit.transform.gameObject.layer == 14)
				{
					Projectile component = hit.transform.gameObject.GetComponent<Projectile>();
					ThrownSword component2 = hit.transform.gameObject.GetComponent<ThrownSword>();
					if (component != null && !component.undeflectable)
					{
						ParryProjectile(component);
					}
					else if (component2 != null && !component2.friendly && component2.active)
					{
						component2.GetParried();
						anim.Play("Hook", -1, 0.065f);
						Parry();
					}
				}
			}
			else
			{
				if (ppz == null)
				{
					ppz = base.transform.parent.GetComponentInChildren<ProjectileParryZone>();
				}
				if (ppz != null)
				{
					Projectile projectile = ppz.CheckParryZone();
					if (projectile != null && !projectile.undeflectable)
					{
						ParryProjectile(projectile);
					}
				}
			}
		}
		bool flag = false;
		if (holding)
		{
			flag = true;
		}
		if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 4f, ignoreEnemyTrigger) || Physics.SphereCast(camObj.transform.position, 1f, camObj.transform.forward, out hit, 4f, ignoreEnemyTrigger))
		{
			bool flag2 = false;
			if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out var hitInfo, 4f, environmentMask) && Vector3.Distance(base.transform.position, hit.point) > Vector3.Distance(base.transform.position, hitInfo.point))
			{
				base.transform.parent.localRotation = Quaternion.identity;
				cc.CameraShake(0.2f * screenShakeMultiplier);
				aud2.clip = normalHit;
				aud2.volume = 0.6f;
				aud2.Play();
				currentDustParticle = Object.Instantiate(dustParticle, hit.point, base.transform.rotation);
				currentDustParticle.transform.forward = hit.normal;
				Breakable component3 = hit.collider.gameObject.GetComponent<Breakable>();
				if (component3 != null && component3.weak)
				{
					component3.Break();
				}
				flag2 = true;
			}
			if (!flag2)
			{
				base.transform.parent.LookAt(hit.point);
				if (Quaternion.Angle(base.transform.parent.localRotation, Quaternion.identity) > 45f)
				{
					Quaternion localRotation = base.transform.parent.localRotation;
					float num = localRotation.eulerAngles.x;
					float num2 = localRotation.eulerAngles.y;
					float num3 = localRotation.eulerAngles.z;
					if (num > 180f)
					{
						num -= 360f;
					}
					if (num2 > 180f)
					{
						num2 -= 360f;
					}
					if (num3 > 180f)
					{
						num3 -= 360f;
					}
					localRotation.eulerAngles = new Vector3(Mathf.Clamp(num, -45f, 45f), Mathf.Clamp(num2, -45f, 45f), Mathf.Clamp(num3, -45f, 45f));
					base.transform.parent.localRotation = localRotation;
				}
				if (hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "Head" || hit.transform.gameObject.tag == "Body" || hit.transform.gameObject.tag == "Limb" || hit.transform.gameObject.tag == "EndLimb")
				{
					if (anim.GetFloat("PunchRandomizer") < 0.5f)
					{
						anim.Play("Jab", -1, 0.075f);
					}
					else
					{
						anim.Play("Jab2", -1, 0.075f);
					}
					aud2.clip = heavyHit;
					aud2.volume = 0.8f;
					aud2.Play();
					cc.HitStop(0.1f);
					cc.CameraShake(0.5f * screenShakeMultiplier);
					EnemyIdentifier eid = hit.transform.GetComponent<EnemyIdentifierIdentifier>().eid;
					eid.rhit = hit;
					eid.hitter = hitter;
					eid.DeliverDamage(hit.transform.gameObject, (eid.transform.position - base.transform.position).normalized * force * 1000f, hit.point, damage, tryForExplode, 0f);
					if (holding)
					{
						heldItem.SendMessage("HitWith", hit.transform.gameObject, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		else if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 4f, environmentMask))
		{
			if (hit.transform.gameObject.layer == 22)
			{
				ItemIdentifier component4 = hit.transform.GetComponent<ItemIdentifier>();
				ItemPlaceZone component5 = hit.transform.GetComponent<ItemPlaceZone>();
				if (holding && component5 != null)
				{
					holding = false;
					anim.SetBool("Holding", value: false);
					ItemIdentifier component6 = heldItem.GetComponent<ItemIdentifier>();
					heldItem.transform.SetParent(component5.transform);
					component6.pickedUp = false;
					if (component6.reverseTransformSettings)
					{
						heldItem.transform.localPosition = Vector3.zero;
						heldItem.transform.localScale = Vector3.one;
						heldItem.transform.localRotation = Quaternion.identity;
					}
					else
					{
						heldItem.transform.localPosition = component6.putDownPosition;
						heldItem.transform.localScale = component6.putDownScale;
						heldItem.transform.localRotation = Quaternion.Euler(component6.putDownRotation);
					}
					heldItem.layer = 22;
					Rigidbody componentInChildren = heldItem.GetComponentInChildren<Rigidbody>();
					if (componentInChildren != null)
					{
						componentInChildren.isKinematic = false;
					}
					Collider componentInChildren2 = heldItem.GetComponentInChildren<Collider>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.enabled = true;
					}
					heldItem = null;
					fc.heldObject = null;
					component5.CheckItem();
					Object.Instantiate(component6.pickUpSound);
				}
				else if (!holding && component4 != null)
				{
					holding = true;
					anim.SetBool("Holding", value: true);
					ItemPlaceZone componentInParent = component4.GetComponentInParent<ItemPlaceZone>();
					heldItem = component4.gameObject;
					component4.transform.SetParent(holder);
					fc.heldObject = component4.gameObject;
					component4.pickedUp = true;
					if (component4.reverseTransformSettings)
					{
						heldItem.transform.localPosition = component4.putDownPosition;
						heldItem.transform.localScale = component4.putDownScale;
						heldItem.transform.localRotation = Quaternion.Euler(component4.putDownRotation);
					}
					else
					{
						heldItem.transform.localPosition = Vector3.zero;
						heldItem.transform.localScale = Vector3.one;
						heldItem.transform.localRotation = Quaternion.identity;
					}
					heldItem.layer = 13;
					Rigidbody componentInChildren3 = heldItem.GetComponentInChildren<Rigidbody>();
					if (componentInChildren3 != null)
					{
						componentInChildren3.isKinematic = true;
					}
					Collider componentInChildren4 = heldItem.GetComponentInChildren<Collider>();
					if (componentInChildren4 != null)
					{
						componentInChildren4.enabled = false;
					}
					Object.Instantiate(component4.pickUpSound);
					heldItem.SendMessage("PickUp", SendMessageOptions.DontRequireReceiver);
					if (componentInParent != null)
					{
						componentInParent.CheckItem();
					}
				}
			}
			else if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 24)
			{
				base.transform.parent.localRotation = Quaternion.identity;
				cc.CameraShake(0.2f * screenShakeMultiplier);
				aud2.clip = normalHit;
				aud2.volume = 0.6f;
				aud2.Play();
				currentDustParticle = Object.Instantiate(dustParticle, hit.point, base.transform.rotation);
				currentDustParticle.transform.forward = hit.normal;
				Breakable component7 = hit.collider.gameObject.GetComponent<Breakable>();
				if (component7 != null && component7.weak)
				{
					component7.Break();
				}
			}
		}
		if (flag && holding && heldItem != null)
		{
			Rigidbody componentInChildren5 = heldItem.GetComponentInChildren<Rigidbody>();
			if (componentInChildren5 != null)
			{
				ItemIdentifier component8 = heldItem.GetComponent<ItemIdentifier>();
				heldItem.transform.SetParent(null, worldPositionStays: true);
				component8.pickedUp = false;
				if (component8.reverseTransformSettings)
				{
					heldItem.transform.localScale = Vector3.one;
				}
				else
				{
					heldItem.transform.localScale = component8.putDownScale;
				}
				heldItem.layer = 22;
				Collider componentInChildren6 = heldItem.GetComponentInChildren<Collider>();
				if (componentInChildren6 != null)
				{
					componentInChildren6.enabled = true;
				}
				heldItem.transform.position = base.transform.parent.position + base.transform.parent.forward;
				heldItem.SendMessage("PutDown", SendMessageOptions.DontRequireReceiver);
				componentInChildren5.isKinematic = false;
				componentInChildren5.AddForce((base.transform.parent.forward + Vector3.up * 0.1f) * 5000f);
				anim.SetBool("Holding", value: false);
				holding = false;
				fc.heldObject = null;
				heldItem = null;
			}
			cc.CameraShake(0.2f * screenShakeMultiplier);
		}
		else
		{
			cc.CameraShake(0.2f * screenShakeMultiplier);
		}
	}

	public void CoinFlip()
	{
		if (ready)
		{
			anim.SetTrigger("CoinFlip");
		}
	}

	private void ActiveEnd()
	{
		tr.widthMultiplier = 0f;
	}

	private void PunchEnd()
	{
	}

	private void ReadyToPunch()
	{
		returnToOrigRot = true;
		holdingInput = false;
		ready = true;
	}

	public void Parry()
	{
		aud.pitch = Random.Range(0.7f, 0.8f);
		nmov.GetHealth(999, silent: true);
		ParryFlash();
		shud.AddPoints(100, "<color=lime>PARRY</color>");
	}

	public void ParryFlash()
	{
		parryLight.enabled = true;
		parryFlash.SetActive(value: true);
		Invoke("hideFlash", 0.1f);
		aud2.clip = specialHit;
		aud2.volume = 0.6f;
		aud2.Play();
		parryFlash.GetComponent<AudioSource>().Play();
		cc.TrueStop(0.25f);
		cc.CameraShake(0.5f);
	}

	private void ParryProjectile(Projectile proj)
	{
		proj.hittingPlayer = false;
		proj.friendly = true;
		proj.speed *= 2f;
		proj.homingType = HomingType.None;
		proj.explosionEffect = parriedProjectileHitObject;
		if (proj.playerBullet)
		{
			proj.boosted = true;
			proj.GetComponent<SphereCollider>().radius *= 4f;
			proj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			proj.damage = 0f;
		}
		if (proj.explosive)
		{
			proj.explosive = false;
		}
		Rigidbody component = proj.GetComponent<Rigidbody>();
		if ((bool)component && component.useGravity)
		{
			component.useGravity = false;
		}
		anim.Play("Hook", -1, 0.065f);
		if (!proj.playerBullet)
		{
			Parry();
		}
		else
		{
			ParryFlash();
		}
		Vector3 zero = Vector3.zero;
		zero = ((!Physics.Raycast(camObj.transform.position, camObj.transform.forward, out var hitInfo, float.PositiveInfinity, ignoreEnemyTrigger)) ? (camObj.transform.position + camObj.transform.forward * 10f) : hitInfo.point);
		proj.transform.LookAt(zero);
		if (proj.speed == 0f)
		{
			component.velocity = (zero - base.transform.position).normalized * 250f;
		}
		if (proj.spreaded)
		{
			ProjectileSpread componentInParent = proj.GetComponentInParent<ProjectileSpread>();
			if (componentInParent != null)
			{
				componentInParent.parried = true;
			}
		}
	}

	public void BlastCheck()
	{
		if (holdingInput)
		{
			holdingInput = false;
			anim.SetTrigger("PunchBlast");
			Object.Instantiate(blastWave, base.transform.position + base.transform.forward * 2f, base.transform.rotation);
		}
	}

	public void Eject()
	{
		if (ejectorAud == null)
		{
			ejectorAud = shellEjector.GetComponent<AudioSource>();
		}
		ejectorAud.Play();
		for (int i = 0; i < 2; i++)
		{
			GameObject gameObject = Object.Instantiate(shell, shellEjector.position + shellEjector.right * 0.075f, shellEjector.rotation);
			if (i == 1)
			{
				gameObject.transform.position = gameObject.transform.position - shellEjector.right * 0.15f;
			}
			gameObject.transform.Rotate(Vector3.forward, Random.Range(-45, 45), Space.Self);
			gameObject.GetComponent<Rigidbody>().AddForce((shellEjector.forward / 1.75f + shellEjector.up / 2f + Vector3.up / 1.75f) * Random.Range(8, 12), ForceMode.VelocityChange);
		}
	}

	private void hideFlash()
	{
		parryLight.enabled = false;
		parryFlash.SetActive(value: false);
		if (crowdReactions)
		{
			if (crorea == null)
			{
				crorea = Object.FindObjectOfType<CrowdReactions>();
			}
			if (crorea.enabled)
			{
				crorea.React(crorea.cheer);
			}
			else
			{
				crowdReactions = false;
			}
		}
	}

	public void Hide()
	{
	}

	public void ShopMode()
	{
		shopping = true;
		holdingInput = false;
		shopRequests++;
	}

	public void StopShop()
	{
		shopRequests--;
		if (shopRequests <= 0)
		{
			shopping = false;
		}
	}

	public void EquipAnimation()
	{
		if (anim == null)
		{
			anim = GetComponent<Animator>();
		}
		anim.SetTrigger("Equip");
	}
}
