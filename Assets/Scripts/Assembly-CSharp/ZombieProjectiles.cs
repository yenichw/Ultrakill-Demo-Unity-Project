using UnityEngine;
using UnityEngine.AI;

public class ZombieProjectiles : MonoBehaviour
{
	public bool stationary;

	public bool smallRay;

	public bool wanderer;

	public bool afraid;

	public bool chaser;

	public bool hasMelee;

	private Zombie zmb;

	private GameObject player;

	private GameObject camObj;

	private NavMeshAgent nma;

	private NavMeshHit hit;

	private Animator anim;

	public Vector3 targetPosition;

	public float coolDown;

	private AudioSource aud;

	public TrailRenderer tr;

	public GameObject projectile;

	private GameObject currentProjectile;

	public Transform shootPos;

	public GameObject head;

	public bool playerSpotted;

	private RaycastHit rhit;

	private RaycastHit bhit;

	public LayerMask lookForPlayerMask;

	public bool seekingPlayer = true;

	private float raySize = 1f;

	private bool musicRequested;

	public GameObject decProjectileSpawner;

	public GameObject decProjectile;

	private GameObject currentDecProjectile;

	public bool swinging;

	private int difficulty;

	private float coolDownReduce;

	private EnemyIdentifier eid;

	private GameObject origWP;

	public Transform aimer;

	private bool aiming;

	private Quaternion origRotation;

	private float aimEase;

	private SwingCheck2[] swingChecks;

	private float lengthOfStop;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		player = GameObject.FindWithTag("Player");
		camObj = GameObject.FindWithTag("MainCamera");
		nma = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		if (stationary || smallRay)
		{
			raySize = 0.25f;
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			coolDownReduce = 1f;
		}
		eid = GetComponent<EnemyIdentifier>();
		origWP = eid.weakPoint;
		if (hasMelee)
		{
			swingChecks = GetComponentsInChildren<SwingCheck2>();
		}
		SlowUpdate();
	}

	private void OnEnable()
	{
		if (!musicRequested && playerSpotted && !zmb.friendly)
		{
			musicRequested = true;
			zmb.musicRequested = true;
			MusicManager musicManager = Object.FindObjectOfType<MusicManager>();
			if (musicManager != null)
			{
				musicManager.PlayBattleMusic();
			}
		}
	}

	private void OnDisable()
	{
		if (musicRequested && !zmb.friendly && !zmb.limp)
		{
			musicRequested = false;
			zmb.musicRequested = false;
			MusicManager musicManager = Object.FindObjectOfType<MusicManager>();
			if (musicManager != null)
			{
				musicManager.PlayCleanMusic();
			}
		}
	}

	private void SlowUpdate()
	{
		if (base.gameObject.activeInHierarchy && zmb.grounded && nma.enabled && !zmb.limp && zmb.target != null && !swinging)
		{
			Vector3 vector = zmb.target.position - base.transform.position;
			Vector3 normalized = (zmb.target.position - head.transform.position).normalized;
			float num = Vector3.Distance(zmb.target.position, base.transform.position);
			if (afraid && !swinging && num < 15f)
			{
				nma.updateRotation = true;
				targetPosition = new Vector3(base.transform.position.x + vector.normalized.x * -10f, base.transform.position.y, base.transform.position.z + vector.normalized.z * -10f);
				if (NavMesh.SamplePosition(targetPosition, out hit, 1f, nma.areaMask))
				{
					nma.SetDestination(targetPosition);
				}
				else
				{
					NavMesh.FindClosestEdge(targetPosition, out hit, nma.areaMask);
					targetPosition = hit.position;
					nma.SetDestination(targetPosition);
				}
				if (nma.velocity.magnitude < 1f)
				{
					lengthOfStop += 0.5f;
				}
				else
				{
					lengthOfStop = 0f;
				}
			}
			if (num > 15f || lengthOfStop > 0.75f || !afraid)
			{
				lengthOfStop = 0f;
				if (playerSpotted && (!chaser || Vector3.Distance(base.transform.position, zmb.target.position) < 3f || coolDown == 0f) && (Vector3.Distance(base.transform.position, zmb.target.position) < 30f || stationary) && !Physics.Raycast(head.transform.position, normalized, out bhit, Vector3.Distance(zmb.target.position, head.transform.position), lookForPlayerMask))
				{
					seekingPlayer = false;
					if (!wanderer)
					{
						nma.SetDestination(base.transform.position);
					}
					else if (wanderer && !chaser && coolDown <= 0f)
					{
						nma.SetDestination(base.transform.position);
					}
					if (hasMelee && Vector3.Distance(base.transform.position, zmb.target.position) <= 3f)
					{
						Melee();
					}
					else if (coolDown <= 0f && nma.velocity.magnitude <= 2.5f)
					{
						Swing();
					}
				}
				else if (!stationary)
				{
					if (chaser)
					{
						if (nma == null)
						{
							nma = zmb.nma;
						}
						if (zmb.grounded && nma != null && nma.enabled && zmb.target != null)
						{
							if (Physics.Raycast(zmb.target.position + Vector3.up * 0.1f, Vector3.down, out var hitInfo, float.PositiveInfinity, lookForPlayerMask))
							{
								nma.SetDestination(hitInfo.point);
							}
							else
							{
								nma.SetDestination(zmb.target.position);
							}
						}
					}
					else
					{
						seekingPlayer = true;
						nma.updateRotation = true;
						nma.SetDestination(zmb.target.position);
					}
				}
			}
		}
		if (chaser)
		{
			Invoke("SlowUpdate", 0.1f);
		}
		else
		{
			Invoke("SlowUpdate", 0.5f);
		}
	}

	private void Update()
	{
		if (!zmb.grounded || !nma.enabled || zmb.limp || !(zmb.target != null))
		{
			return;
		}
		if (coolDown > 0f)
		{
			coolDown = Mathf.MoveTowards(coolDown, 0f, Time.deltaTime);
		}
		if (!zmb.limp && Input.GetKey(KeyCode.Alpha8))
		{
			Debug.Log(nma.velocity);
		}
		if (nma.velocity.magnitude <= 2.5f && playerSpotted && !seekingPlayer && (!wanderer || !swinging || chaser))
		{
			anim.SetBool("Running", value: false);
			nma.updateRotation = false;
			base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		}
		else if (nma.velocity.magnitude > 2.5f)
		{
			anim.SetBool("Running", value: true);
			nma.updateRotation = true;
		}
		else if (nma.velocity.magnitude <= 2.5f && playerSpotted && !seekingPlayer && wanderer && swinging)
		{
			anim.SetBool("Running", value: false);
			nma.updateRotation = false;
			if (difficulty >= 2)
			{
				Vector3 vector = new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z);
				Quaternion b = Quaternion.LookRotation((vector - base.transform.position).normalized);
				if (difficulty == 2)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 3.5f);
				}
				else if (difficulty == 3)
				{
					base.transform.LookAt(vector);
				}
				else if (difficulty > 3)
				{
					base.transform.LookAt(vector);
				}
			}
		}
		if (playerSpotted)
		{
			return;
		}
		Vector3 normalized = (zmb.target.position - head.transform.position).normalized;
		if (Physics.Raycast(head.transform.position, normalized, out rhit, Vector3.Distance(zmb.target.position, head.transform.position), lookForPlayerMask))
		{
			return;
		}
		seekingPlayer = false;
		playerSpotted = true;
		coolDown = (float)Random.Range(1, 2) - coolDownReduce / 2f;
		if (zmb.target == zmb.player.transform && !musicRequested)
		{
			musicRequested = true;
			zmb.musicRequested = true;
			MusicManager musicManager = Object.FindObjectOfType<MusicManager>();
			if (musicManager != null)
			{
				musicManager.PlayBattleMusic();
			}
		}
	}

	private void LateUpdate()
	{
		if (aimer != null && aiming && (bool)zmb.target)
		{
			Quaternion b = Quaternion.LookRotation((zmb.target.position - aimer.position).normalized);
			if (aimEase < 1f)
			{
				aimEase = Mathf.MoveTowards(aimEase, 1f, Time.deltaTime * (20f - aimEase * 20f));
			}
			aimer.rotation = Quaternion.Slerp(origRotation, b, aimEase);
		}
	}

	public void Melee()
	{
		swinging = true;
		seekingPlayer = false;
		nma.updateRotation = false;
		base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		if (nma.enabled)
		{
			nma.isStopped = true;
		}
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.GetComponent<AudioSource>().Play();
		anim.SetTrigger("Melee");
	}

	public void MeleePrep()
	{
		zmb.attacking = true;
	}

	public void MeleeDamageStart()
	{
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		if (tr != null)
		{
			tr.enabled = true;
			tr.emitting = true;
		}
		SwingCheck2[] array = swingChecks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DamageStart();
		}
	}

	public void MeleeDamageEnd()
	{
		if (tr != null)
		{
			tr.emitting = false;
		}
		SwingCheck2[] array = swingChecks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DamageStop();
		}
		zmb.attacking = false;
	}

	public void Swing()
	{
		swinging = true;
		seekingPlayer = false;
		nma.updateRotation = false;
		base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		if (nma.enabled)
		{
			nma.isStopped = true;
		}
		if (zmb.target.position.y - 5f > base.transform.position.y || zmb.target.position.y + 5f < base.transform.position.y)
		{
			anim.SetFloat("AttackType", 1f);
		}
		else
		{
			anim.SetFloat("AttackType", Random.Range(0, 2));
		}
		anim.SetTrigger("Swing");
		coolDown = 99f;
	}

	public void SwingEnd()
	{
		swinging = false;
		if (nma.enabled)
		{
			nma.isStopped = false;
		}
		coolDown = Random.Range(1f, 2.5f) - coolDownReduce;
		if (wanderer)
		{
			if (nma.isOnNavMesh)
			{
				NavMesh.SamplePosition(Random.onUnitSphere * 10f + base.transform.position, out var navMeshHit, 10f, 1);
				nma.SetDestination(navMeshHit.position);
			}
			coolDown = Random.Range(0f, 2f) - coolDownReduce;
		}
		if (tr != null)
		{
			tr.enabled = false;
		}
	}

	public void SpawnProjectile()
	{
		currentDecProjectile = Object.Instantiate(decProjectile, decProjectileSpawner.transform.position, decProjectileSpawner.transform.rotation);
		currentDecProjectile.transform.SetParent(decProjectileSpawner.transform, worldPositionStays: true);
		eid.weakPoint = currentDecProjectile;
	}

	public void DamageStart()
	{
		if (!hasMelee)
		{
			if (tr == null)
			{
				tr = GetComponentInChildren<TrailRenderer>();
			}
			if (tr != null)
			{
				tr.enabled = true;
			}
		}
		zmb.attacking = true;
		if (aimer != null)
		{
			origRotation = aimer.rotation;
			aiming = true;
		}
	}

	public void ThrowProjectile()
	{
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		currentProjectile = Object.Instantiate(projectile, shootPos.position, base.transform.rotation);
		if (zmb.target == player.transform)
		{
			currentProjectile.transform.LookAt(camObj.transform);
		}
		else
		{
			currentProjectile.transform.LookAt(zmb.target.GetComponentInChildren<EnemyIdentifierIdentifier>().transform);
		}
		Projectile componentInChildren = currentProjectile.GetComponentInChildren<Projectile>();
		if (componentInChildren != null)
		{
			componentInChildren.safeEnemyType = EnemyType.Zombie;
			if (difficulty > 2)
			{
				componentInChildren.speed *= 1.35f;
			}
		}
	}

	public void ShootProjectile()
	{
		swinging = true;
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		currentProjectile = Object.Instantiate(projectile, decProjectileSpawner.transform.position, decProjectileSpawner.transform.rotation);
		currentProjectile.GetComponent<Projectile>().safeEnemyType = EnemyType.Zombie;
		if (difficulty > 2)
		{
			currentProjectile.GetComponent<Projectile>().speed *= 1.25f;
		}
	}

	public void StopTracking()
	{
	}

	public void DamageEnd()
	{
		if (!hasMelee && tr != null)
		{
			tr.enabled = false;
		}
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		zmb.attacking = false;
		if (aimer != null)
		{
			aimEase = 0f;
			aiming = false;
		}
	}

	public void CancelAttack()
	{
		Debug.Log("Cancelled Attack");
		Debug.Log("Cancelled Attack");
		swinging = false;
		coolDown = 0f;
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		if (tr != null)
		{
			tr.enabled = false;
		}
		zmb.attacking = false;
	}
}
