using UnityEngine;
using UnityEngine.AI;

public class SpiderBody : MonoBehaviour
{
	private Rigidbody[] rbs;

	public bool limp;

	private GameObject player;

	private Transform cam;

	private NewMovement nmov;

	private NavMeshAgent nma;

	private Quaternion followPlayerRot;

	public GameObject proj;

	private RaycastHit hit;

	private RaycastHit hit2;

	public LayerMask aimlm;

	private bool readyToShoot = true;

	private float burstCharge;

	private int currentBurst;

	public float health;

	public bool dead;

	private Rigidbody rb;

	private bool falling;

	private Enemy enemy;

	private Transform firstChild;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private Transform[] bodyChild;

	public GameObject impactParticle;

	public GameObject impactSprite;

	private Quaternion spriteRot;

	private Vector3 spritePos;

	public Transform mouth;

	private GameObject currentProj;

	private bool charging;

	public GameObject chargeEffect;

	private GameObject currentCE;

	private float beamCharge;

	private AudioSource ceAud;

	private Light ceLight;

	private Vector3 predictedPlayerPos;

	public GameObject spiderBeam;

	private GameObject currentBeam;

	public GameObject beamExplosion;

	private GameObject currentExplosion;

	private float beamProbability;

	private Quaternion predictedRot;

	private bool rotating;

	public GameObject dripBlood;

	private GameObject currentDrip;

	public GameObject smallBlood;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	public GameObject spark;

	private int difficulty;

	private float coolDownMultiplier = 1f;

	private int beamsAmount = 1;

	private float maxHealth;

	public GameObject enrageEffect;

	private GameObject currentEnrageEffect;

	public Material enrageMaterial;

	public Material enrageMaterial2;

	private Material origMaterial;

	private bool parryable;

	private MusicManager muman;

	private bool requestedMusic;

	private GoreZone gz;

	private Transform headModel;

	public GameObject breakParticle;

	private bool corpseBroken;

	private void Start()
	{
		burstCharge = 5f;
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		cam = GameObject.FindWithTag("MainCamera").transform;
		nma = GetComponent<NavMeshAgent>();
		nmov = player.GetComponent<NewMovement>();
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		maxHealth = health;
		if (difficulty != 2 && difficulty >= 3)
		{
			coolDownMultiplier = 1.25f;
		}
		origMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
		if (gz == null)
		{
			gz = GetComponentInParent<GoreZone>();
		}
		headModel = GetComponentInChildren<Animator>().transform;
		nma.updateRotation = false;
	}

	private void OnDisable()
	{
		if (!dead)
		{
			requestedMusic = false;
			if (muman == null)
			{
				muman = Object.FindObjectOfType<MusicManager>();
			}
			if ((bool)muman)
			{
				muman.PlayCleanMusic();
			}
		}
	}

	private void Update()
	{
		followPlayerRot = Quaternion.LookRotation((player.transform.position - base.transform.position).normalized);
		if (dead)
		{
			return;
		}
		if (beamCharge < 1f)
		{
			headModel.transform.rotation = Quaternion.RotateTowards(headModel.transform.rotation, followPlayerRot, (Quaternion.Angle(headModel.transform.rotation, followPlayerRot) + 10f) * Time.deltaTime * 15f);
		}
		else if (rotating && beamCharge == 1f)
		{
			headModel.transform.rotation = Quaternion.RotateTowards(headModel.transform.rotation, predictedRot, Quaternion.Angle(headModel.transform.rotation, predictedRot) * Time.deltaTime * 20f);
		}
		else if (!rotating && beamCharge == 1f)
		{
			predictedRot = Quaternion.LookRotation(player.transform.position - base.transform.position);
			headModel.transform.rotation = Quaternion.RotateTowards(headModel.transform.rotation, predictedRot, (Quaternion.Angle(headModel.transform.rotation, predictedRot) + 10f) * Time.deltaTime * 10f);
		}
		if (difficulty > 2 && currentEnrageEffect == null && health < maxHealth / 2f)
		{
			Enrage();
		}
		if (!requestedMusic)
		{
			requestedMusic = true;
			if (!muman)
			{
				muman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
			}
			if ((bool)muman)
			{
				muman.PlayBattleMusic();
			}
		}
		if (!charging && beamCharge == 0f)
		{
			if (nma != null && !nma.enabled)
			{
				nma.enabled = true;
				nma.isStopped = false;
				nma.speed = 3.5f;
			}
			if (nma != null)
			{
				nma.SetDestination(player.transform.position);
			}
			if (currentBurst > 5 && burstCharge == 0f)
			{
				currentBurst = 0;
				burstCharge = 5f;
			}
			if (burstCharge > 0f)
			{
				burstCharge = Mathf.MoveTowards(burstCharge, 0f, Time.deltaTime * coolDownMultiplier * 5f);
			}
			if (burstCharge < 0f)
			{
				burstCharge = 0f;
			}
			if (!readyToShoot || burstCharge != 0f || (!(Quaternion.Angle(headModel.rotation, followPlayerRot) < 1f) && !(Vector3.Distance(base.transform.position, player.transform.position) < 10f)) || Physics.Raycast(base.transform.position, player.transform.position - base.transform.position, out hit, Vector3.Distance(base.transform.position, player.transform.position), aimlm))
			{
				return;
			}
			if (currentBurst != 0)
			{
				ShootProj();
				return;
			}
			if ((Random.Range(0f, health * 0.4f) >= beamProbability && beamProbability <= 5f) || Vector3.Distance(base.transform.position, player.transform.position) > 50f)
			{
				ShootProj();
				beamProbability += 1f;
				return;
			}
			ChargeBeam();
			if (difficulty > 2 && health < maxHealth / 2f)
			{
				beamsAmount = 2;
			}
			if (health > 10f)
			{
				beamProbability = 0f;
			}
			else
			{
				beamProbability = 1f;
			}
		}
		else if (charging)
		{
			if (beamCharge + 0.5f * coolDownMultiplier * Time.deltaTime < 1f)
			{
				nma.speed = 0f;
				nma.SetDestination(base.transform.position);
				nma.isStopped = true;
				beamCharge += 0.5f * coolDownMultiplier * Time.deltaTime;
				currentCE.transform.localScale = Vector3.one * beamCharge * 2.5f;
				ceAud.pitch = beamCharge * 2f;
				ceLight.intensity = beamCharge * 30f;
			}
			else
			{
				beamCharge = 1f;
				charging = false;
				BeamChargeEnd();
			}
		}
	}

	public void GetHurt(GameObject target, Vector3 force, Vector3 hitPoint, float multiplier)
	{
		bool flag = false;
		bool flag2 = true;
		if (PlayerPrefs.GetInt("BlOn", 1) == 0)
		{
			flag2 = false;
		}
		GameObject gameObject = Object.Instantiate(smallBlood, hitPoint, Quaternion.identity);
		gameObject.transform.SetParent(gz.goreZone, worldPositionStays: true);
		if (health > 0f)
		{
			gameObject.GetComponent<Bloodsplatter>().GetReady();
		}
		if (multiplier >= 1f)
		{
			gameObject.GetComponent<Bloodsplatter>().hpAmount = 30;
		}
		if (flag2)
		{
			gameObject.GetComponent<ParticleSystem>().Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (eid.hitter != "shotgun" && eid.hitter != "drill")
		{
			currentDrip = Object.Instantiate(dripBlood, hitPoint, Quaternion.identity);
			currentDrip.transform.parent = base.transform;
			currentDrip.transform.LookAt(base.transform);
			currentDrip.transform.Rotate(180f, 180f, 180f);
			if (flag2)
			{
				currentDrip.GetComponent<ParticleSystem>().Play();
			}
		}
		if (!dead)
		{
			health -= 1f * multiplier;
			if (scalc == null)
			{
				scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
			}
			if (health <= 0f)
			{
				flag = true;
			}
			if (parryable && (eid.hitter == "shotgunzone" || eid.hitter == "punch"))
			{
				parryable = false;
				player.GetComponentInChildren<Punch>().Parry();
				currentExplosion = Object.Instantiate(beamExplosion, base.transform.position, Quaternion.identity);
				health -= 5f;
				Explosion[] componentsInChildren = currentExplosion.GetComponentsInChildren<Explosion>();
				foreach (Explosion obj in componentsInChildren)
				{
					obj.maxSize *= 1.75f;
					obj.damage = 50;
					obj.safeForPlayer = true;
					obj.friendlyFire = true;
				}
				if (currentEnrageEffect == null)
				{
					CancelInvoke("BeamFire");
					Invoke("StopWaiting", 1f);
					Object.Destroy(currentCE);
				}
			}
			scalc.HitCalculator(eid.hitter, "spider", "", flag, base.gameObject);
			if (health <= 0f && !dead)
			{
				dead = true;
				Die();
			}
		}
		else if (eid.hitter == "ground slam")
		{
			BreakCorpse();
		}
	}

	public void Die()
	{
		rb = GetComponentInChildren<Rigidbody>();
		falling = true;
		parryable = false;
		rb.isKinematic = false;
		rb.useGravity = true;
		for (int i = 1; i < base.transform.parent.childCount - 1; i++)
		{
			Object.Destroy(base.transform.parent.GetChild(i).gameObject);
		}
		if (currentCE != null)
		{
			Object.Destroy(currentCE);
		}
		Object.Destroy(nma);
		gz = GetComponentInParent<GoreZone>();
		if (gz != null && gz.checkpoint != null)
		{
			gz.AddDeath();
			gz.checkpoint.sm.kills++;
		}
		else
		{
			GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().kills++;
		}
		ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
		if (componentInParent != null)
		{
			componentInParent.deadEnemies++;
		}
		if (muman == null)
		{
			muman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		}
		if ((bool)muman)
		{
			muman.PlayCleanMusic();
		}
		if (currentEnrageEffect != null)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material = origMaterial;
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].material = origMaterial;
			}
			Object.Destroy(currentEnrageEffect);
		}
		if (eid.hitter == "ground slam" || eid.hitter == "breaker")
		{
			BreakCorpse();
		}
	}

	private void ShootProj()
	{
		currentBurst++;
		currentProj = Object.Instantiate(proj, mouth.position, headModel.transform.rotation);
		currentProj.transform.rotation = Quaternion.LookRotation(cam.transform.position - mouth.position);
		currentProj.GetComponent<Projectile>().safeEnemyType = EnemyType.Spider;
		if (difficulty > 2)
		{
			currentProj.GetComponent<Projectile>().speed *= 1.25f;
		}
		readyToShoot = false;
		if (difficulty > 2)
		{
			Invoke("ReadyToShoot", 0.1f);
		}
		else
		{
			Invoke("ReadyToShoot", 0.1f);
		}
	}

	private void ChargeBeam()
	{
		charging = true;
		currentCE = Object.Instantiate(chargeEffect, mouth);
		currentCE.transform.localScale = Vector3.zero;
		ceAud = currentCE.GetComponent<AudioSource>();
		ceLight = currentCE.GetComponent<Light>();
	}

	private void BeamChargeEnd()
	{
		if (beamsAmount <= 1)
		{
			ceAud.Stop();
		}
		Vector3 vector = new Vector3(nmov.rb.velocity.x, nmov.rb.velocity.y / 2f, nmov.rb.velocity.z);
		predictedPlayerPos = player.transform.position + vector / 2f;
		nma.enabled = false;
		predictedRot = Quaternion.LookRotation(predictedPlayerPos - base.transform.position);
		rotating = true;
		Object.Instantiate(spark, mouth.position, mouth.rotation).transform.LookAt(predictedPlayerPos);
		if (difficulty > 2)
		{
			Invoke("BeamFire", 0.5f);
		}
		else
		{
			Invoke("BeamFire", 0.5f);
		}
		parryable = true;
	}

	private void BeamFire()
	{
		parryable = false;
		if (!dead)
		{
			currentBeam = Object.Instantiate(spiderBeam, mouth.position, mouth.rotation);
			LineRenderer component = currentBeam.GetComponent<LineRenderer>();
			component.SetPosition(0, mouth.position);
			Vector3 zero = Vector3.zero;
			zero = ((!Physics.Raycast(mouth.position, predictedPlayerPos - mouth.position, out hit2, float.PositiveInfinity, aimlm)) ? ((predictedPlayerPos - mouth.position) * 500f) : hit2.point);
			component.SetPosition(1, zero);
			currentExplosion = Object.Instantiate(beamExplosion, zero + (base.transform.position - zero).normalized, Quaternion.identity);
			currentExplosion.transform.forward = hit2.normal;
			Explosion[] componentsInChildren = currentExplosion.GetComponentsInChildren<Explosion>();
			foreach (Explosion obj in componentsInChildren)
			{
				obj.maxSize *= 2.25f;
				obj.damage = 50;
				obj.enemy = true;
			}
			rotating = false;
			if (beamsAmount > 1)
			{
				beamsAmount--;
				ceAud.pitch = 4f;
				ceAud.volume = 1f;
				Invoke("BeamChargeEnd", 0.5f);
			}
			else
			{
				Object.Destroy(currentCE);
				Invoke("StopWaiting", 1f);
			}
		}
	}

	private void StopWaiting()
	{
		if (!dead)
		{
			beamCharge = 0f;
		}
	}

	private void ReadyToShoot()
	{
		readyToShoot = true;
	}

	public void TriggerHit(Collider other)
	{
		if (falling)
		{
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component == null)
			{
				component = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			}
			if (component != null)
			{
				component.DeliverDamage(other.gameObject, Vector3.zero, other.transform.position, 999999f, tryForExplode: true, 0f);
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (falling && other.gameObject.tag == "Floor")
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			Object.Instantiate(impactParticle, base.transform.position, base.transform.rotation);
			spriteRot.eulerAngles = new Vector3(other.contacts[0].normal.x + 90f, other.contacts[0].normal.y, other.contacts[0].normal.z);
			spritePos = new Vector3(other.contacts[0].point.x, other.contacts[0].point.y + 0.1f, other.contacts[0].point.z);
			Object.Instantiate(impactSprite, spritePos, spriteRot).transform.SetParent(GetComponentInParent<GoreZone>().goreZone, worldPositionStays: true);
			base.transform.position = base.transform.position - base.transform.up * 1.5f;
			falling = false;
			rb.GetComponent<NavMeshObstacle>().enabled = true;
			player.GetComponentInChildren<CameraController>().CameraShake(2f);
		}
		else if (falling && other.gameObject.tag == "Moving")
		{
			BreakCorpse();
			player.GetComponentInChildren<CameraController>().CameraShake(2f);
		}
	}

	public void Enrage()
	{
		if (!dead)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material = enrageMaterial;
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = enrageMaterial2;
			}
			currentEnrageEffect = Object.Instantiate(enrageEffect, base.transform);
			currentEnrageEffect.transform.localScale = Vector3.one * 0.2f;
		}
	}

	public void BreakCorpse()
	{
		if (!corpseBroken)
		{
			corpseBroken = true;
			if (breakParticle != null)
			{
				Object.Instantiate(breakParticle, base.transform.position, base.transform.rotation).transform.SetParent(GetComponentInParent<GoreZone>().gibZone);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
