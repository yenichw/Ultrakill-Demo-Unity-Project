using UnityEngine;

public class Drone : MonoBehaviour
{
	public bool friendly;

	public bool spawnIn;

	public bool dontStartAware;

	public GameObject spawnEffect;

	public float health;

	public bool crashing;

	private Vector3 crashTarget;

	private Rigidbody rb;

	private bool canInterruptCrash;

	private Transform modelTransform;

	public bool playerSpotted;

	public bool toLastKnownPos;

	private Vector3 lastKnownPos;

	public LayerMask lmask;

	private Vector3 nextRandomPos;

	public float checkCooldown;

	public float blockCooldown;

	private GameObject camObj;

	private Transform target;

	public GameObject smallBlood;

	public GameObject bigBlood;

	public GameObject explosion;

	public GameObject gib;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private AudioSource aud;

	public AudioClip hurtSound;

	public AudioClip deathSound;

	public AudioClip windUpSound;

	public AudioClip spotSound;

	public AudioClip loseSound;

	private float dodgeCooldown;

	private float attackCooldown;

	public GameObject projectile;

	private Material origMaterial;

	public Material shootMaterial;

	private MeshRenderer[] mrs;

	private ParticleSystem part;

	private bool killedByPlayer;

	private bool parried;

	private bool exploded;

	private Vector3 viewTarget;

	[HideInInspector]
	public bool musicRequested;

	private GoreZone gz;

	private int difficulty;

	private void Start()
	{
		camObj = GameObject.FindWithTag("MainCamera");
		rb = GetComponent<Rigidbody>();
		part = GetComponentInChildren<ParticleSystem>();
		if (!friendly)
		{
			target = camObj.transform;
		}
		dodgeCooldown = Random.Range(0.5f, 3f);
		attackCooldown = Random.Range(1f, 3f);
		if (spawnIn)
		{
			Object.Instantiate(spawnEffect, base.transform.position, Quaternion.identity);
		}
		if (!dontStartAware)
		{
			playerSpotted = true;
		}
		modelTransform = base.transform.Find("drone");
		mrs = modelTransform.GetComponentsInChildren<MeshRenderer>();
		origMaterial = mrs[0].material;
		SlowUpdate();
		if (!musicRequested)
		{
			musicRequested = true;
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayBattleMusic();
		}
		if (gz == null)
		{
			gz = GetComponentInParent<GoreZone>();
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
	}

	private void OnDisable()
	{
		if (musicRequested)
		{
			musicRequested = false;
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
	}

	private void OnEnable()
	{
		if (!musicRequested)
		{
			musicRequested = true;
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayBattleMusic();
		}
	}

	private void Update()
	{
		if (crashing || !playerSpotted)
		{
			return;
		}
		viewTarget = target.position;
		if (dodgeCooldown > 0f)
		{
			dodgeCooldown = Mathf.MoveTowards(dodgeCooldown, 0f, Time.deltaTime * (float)(difficulty / 2));
		}
		else
		{
			dodgeCooldown = Random.Range(1f, 3f);
			RandomDodge();
		}
		if (attackCooldown > 0f)
		{
			attackCooldown = Mathf.MoveTowards(attackCooldown, 0f, Time.deltaTime * (float)(difficulty / 2));
			return;
		}
		attackCooldown = Random.Range(2f, 4f);
		PlaySound(windUpSound);
		part.Play();
		MeshRenderer[] array = mrs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material = shootMaterial;
		}
		Invoke("Shoot", 0.75f);
	}

	private void SlowUpdate()
	{
		if (!crashing)
		{
			if (playerSpotted)
			{
				if (Physics.Raycast(base.transform.position, target.transform.position - base.transform.position, Vector3.Distance(base.transform.position, target.transform.position) - 1f, lmask))
				{
					playerSpotted = false;
					PlaySound(loseSound);
					lastKnownPos = target.transform.position;
					blockCooldown = 0f;
					checkCooldown = 0f;
					toLastKnownPos = true;
				}
			}
			else if (!Physics.Raycast(base.transform.position, target.transform.position - base.transform.position, Vector3.Distance(base.transform.position, target.transform.position) - 1f, lmask))
			{
				PlaySound(spotSound);
				playerSpotted = true;
			}
		}
		Invoke("SlowUpdate", 0.25f);
	}

	private void FixedUpdate()
	{
		if (rb.velocity.magnitude > 40f && rb.collisionDetectionMode != CollisionDetectionMode.Continuous)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}
		else if (rb.velocity.magnitude < 40f && rb.collisionDetectionMode != 0)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
		if (crashing)
		{
			if (!parried)
			{
				rb.AddForce(base.transform.forward * 50f, ForceMode.Acceleration);
				modelTransform.Rotate(0f, 0f, 10f, Space.Self);
			}
			else
			{
				rb.velocity = base.transform.forward * 50f;
				modelTransform.Rotate(0f, 0f, 50f, Space.Self);
			}
		}
		else if (playerSpotted)
		{
			rb.velocity *= 0.95f;
			if (Vector3.Distance(base.transform.position, target.transform.position) > 15f)
			{
				rb.AddForce(base.transform.forward * 50f, ForceMode.Acceleration);
			}
			else if (Vector3.Distance(base.transform.position, target.transform.position) < 5f)
			{
				rb.AddForce(base.transform.forward * -50f, ForceMode.Impulse);
			}
		}
		else if (toLastKnownPos)
		{
			if (blockCooldown == 0f)
			{
				viewTarget = lastKnownPos;
			}
			else
			{
				blockCooldown = Mathf.MoveTowards(blockCooldown, 0f, 0.01f);
			}
			rb.AddForce(base.transform.forward * 10f, ForceMode.Acceleration);
			if (checkCooldown == 0f && Vector3.Distance(base.transform.position, lastKnownPos) > 5f)
			{
				checkCooldown = 0.1f;
				if (Physics.BoxCast(base.transform.position - (viewTarget - base.transform.position).normalized, Vector3.one, viewTarget - base.transform.position, base.transform.rotation, 4f, lmask))
				{
					blockCooldown = Random.Range(1.5f, 3f);
					Vector3 vector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
					viewTarget = base.transform.position + vector * 100f;
				}
			}
			else if (Vector3.Distance(base.transform.position, lastKnownPos) <= 3f)
			{
				Physics.Raycast(base.transform.position, Random.onUnitSphere, out var hitInfo, float.PositiveInfinity, lmask);
				lastKnownPos = hitInfo.point;
			}
			if (checkCooldown != 0f)
			{
				checkCooldown = Mathf.MoveTowards(checkCooldown, 0f, 0.01f);
			}
		}
		if (!crashing)
		{
			Quaternion b = Quaternion.LookRotation(viewTarget - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 0.075f + 0.00025f * Quaternion.Angle(base.transform.rotation, b));
			rb.velocity = Vector3.ClampMagnitude(rb.velocity, 50f);
		}
	}

	public void RandomDodge()
	{
		rb.AddForce(base.transform.up * Random.Range(-5f, 5f) + (base.transform.right * Random.Range(-5f, 5f)).normalized * 50f, ForceMode.Impulse);
	}

	public void GetHurt(Vector3 force, float multiplier)
	{
		bool dead = false;
		if (!crashing)
		{
			health -= 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (health <= 0f)
			{
				dead = true;
			}
			if (eid.hitter != "enemy")
			{
				if (scalc == null)
				{
					scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
				}
				scalc.HitCalculator(eid.hitter, "drone", "", dead, base.gameObject);
			}
			if (health <= 0f && !crashing)
			{
				crashing = true;
				if (eid.hitter != "enemy")
				{
					killedByPlayer = true;
				}
				GoreZone componentInParent = GetComponentInParent<GoreZone>();
				if (componentInParent != null && componentInParent.checkpoint != null)
				{
					componentInParent.AddDeath();
					componentInParent.checkpoint.sm.kills++;
				}
				else
				{
					GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().kills++;
				}
				crashTarget = target.transform.position;
				base.transform.LookAt(crashTarget);
				if (aud == null)
				{
					aud = GetComponent<AudioSource>();
				}
				aud.clip = deathSound;
				aud.volume = 0.75f;
				aud.pitch = Random.Range(0.85f, 1.35f);
				aud.priority = 11;
				aud.Play();
				GameObject obj = Object.Instantiate(bigBlood, base.transform.position, Quaternion.identity);
				obj.GetComponent<Bloodsplatter>().GetReady();
				obj.GetComponent<ParticleSystem>().Play();
				obj.transform.SetParent(componentInParent.goreZone, worldPositionStays: true);
				ActivateNextWave componentInParent2 = GetComponentInParent<ActivateNextWave>();
				if (componentInParent2 != null)
				{
					componentInParent2.deadEnemies++;
				}
				Invoke("CanInterruptCrash", 0.5f);
				Invoke("Explode", 5f);
				return;
			}
			PlaySound(hurtSound);
			GameObject gameObject = Object.Instantiate(smallBlood, base.transform.position, Quaternion.identity);
			gameObject.transform.SetParent(gz.goreZone, worldPositionStays: true);
			if (health > 0f)
			{
				gameObject.GetComponent<Bloodsplatter>().GetReady();
				rb.velocity /= 10f;
				rb.AddForce(force.normalized * (force.magnitude / 100f), ForceMode.Impulse);
				if (rb.velocity.magnitude > 50f)
				{
					rb.velocity = Vector3.ClampMagnitude(rb.velocity, 50f);
				}
			}
			if (multiplier >= 1f)
			{
				gameObject.GetComponent<Bloodsplatter>().hpAmount = 30;
				for (int i = 0; (float)i <= multiplier; i++)
				{
					Object.Instantiate(gib, base.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
				}
			}
			gameObject.GetComponent<ParticleSystem>().Play();
		}
		else if (eid.hitter == "punch" && !parried)
		{
			parried = true;
			rb.velocity = Vector3.zero;
			base.transform.rotation = camObj.transform.rotation;
			Punch componentInChildren = camObj.GetComponentInChildren<Punch>();
			componentInChildren.GetComponent<Animator>().Play("Hook", -1, 0.065f);
			componentInChildren.Parry();
		}
		else if (multiplier >= 1f || canInterruptCrash)
		{
			Explode();
		}
	}

	public void PlaySound(AudioClip clippe)
	{
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		aud.clip = clippe;
		aud.volume = 0.5f;
		aud.pitch = Random.Range(0.85f, 1.35f);
		aud.priority = 12;
		aud.Play();
	}

	public void Explode()
	{
		if (exploded)
		{
			return;
		}
		exploded = true;
		GameObject gameObject = Object.Instantiate(explosion, base.transform.position, Quaternion.identity);
		if (killedByPlayer)
		{
			Explosion[] componentsInChildren = gameObject.GetComponentsInChildren<Explosion>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].friendlyFire = true;
			}
		}
		Object.Destroy(base.gameObject);
		if (musicRequested)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
	}

	public void Shoot()
	{
		if (!crashing)
		{
			GameObject gameObject = Object.Instantiate(projectile, base.transform.position + base.transform.forward, base.transform.rotation);
			gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, Random.Range(0, 360));
			gameObject.transform.localScale *= 0.5f;
			Projectile component = gameObject.GetComponent<Projectile>();
			component.safeEnemyType = EnemyType.Drone;
			component.speed = 35f;
			GameObject gameObject2 = Object.Instantiate(projectile, gameObject.transform.position + gameObject.transform.up, gameObject.transform.rotation);
			if (difficulty > 2)
			{
				gameObject2.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x + 1f, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
			}
			gameObject2.transform.localScale *= 0.5f;
			Projectile component2 = gameObject2.GetComponent<Projectile>();
			component2.safeEnemyType = EnemyType.Drone;
			component2.speed = 35f;
			gameObject2 = Object.Instantiate(projectile, gameObject.transform.position - gameObject.transform.up, gameObject.transform.rotation);
			if (difficulty > 2)
			{
				gameObject2.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x + 1f, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
			}
			gameObject2.transform.localScale *= 0.5f;
			Projectile component3 = gameObject2.GetComponent<Projectile>();
			component3.safeEnemyType = EnemyType.Drone;
			component3.speed = 35f;
			MeshRenderer[] array = mrs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = origMaterial;
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (crashing && (collision.gameObject.layer == 0 || collision.gameObject.layer == 8 || collision.gameObject.layer == 24 || collision.gameObject.layer == 10 || collision.gameObject.layer == 11 || collision.gameObject.layer == 12 || collision.gameObject.layer == 21 || collision.gameObject.tag == "Player"))
		{
			Explode();
		}
	}

	private void CanInterruptCrash()
	{
		canInterruptCrash = true;
	}
}
