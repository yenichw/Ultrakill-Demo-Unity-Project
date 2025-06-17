using UnityEngine;
using UnityEngine.AI;

public class ZombieMelee : MonoBehaviour
{
	public bool damaging;

	public TrailRenderer tr;

	public bool track;

	private AudioSource aud;

	public float coolDown;

	public Zombie zmb;

	private NavMeshAgent nma;

	private GameObject player;

	private Animator anim;

	private bool customStart;

	private bool musicRequested;

	private int difficulty;

	private float defaultCoolDown = 0.5f;

	public GameObject swingSound;

	public bool attacking;

	public LayerMask lmask;

	private Rigidbody rb;

	private SwingCheck2 swingCheck;

	private SkinnedMeshRenderer smr;

	private Material origMaterial;

	public Material biteMaterial;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		nma = zmb.nma;
		anim = zmb.anim;
		player = zmb.player;
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			defaultCoolDown = 0.25f;
		}
		if (!musicRequested && !zmb.friendly)
		{
			musicRequested = true;
			zmb.musicRequested = true;
			MusicManager musicManager = Object.FindObjectOfType<MusicManager>();
			if ((bool)musicManager)
			{
				musicManager.PlayBattleMusic();
			}
		}
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		origMaterial = smr.sharedMaterial;
		TrackTick();
	}

	private void Update()
	{
		if (damaging)
		{
			if (rb == null)
			{
				rb = GetComponent<Rigidbody>();
			}
			rb.isKinematic = false;
			rb.velocity = base.transform.forward * 40f;
		}
		if (track && zmb.target != null)
		{
			base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		}
		if (coolDown != 0f)
		{
			if (coolDown - Time.deltaTime > 0f)
			{
				coolDown -= Time.deltaTime / 2.5f;
			}
			else
			{
				coolDown = 0f;
			}
		}
		else if (zmb.target != null && Vector3.Distance(zmb.target.position, base.transform.position) < 3f)
		{
			Swing();
		}
	}

	private void OnEnable()
	{
		if (zmb == null)
		{
			zmb = GetComponent<Zombie>();
		}
		if (!musicRequested && !zmb.friendly)
		{
			musicRequested = true;
			zmb.musicRequested = true;
			MusicManager musicManager = Object.FindObjectOfType<MusicManager>();
			if ((bool)musicManager)
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
			if ((bool)musicManager)
			{
				musicManager.PlayCleanMusic();
			}
		}
	}

	private void FixedUpdate()
	{
		if (anim == null)
		{
			anim = zmb.anim;
		}
		if (nma == null)
		{
			nma = zmb.nma;
		}
		if (zmb.grounded && nma != null && nma.enabled && zmb.target != null)
		{
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.SetBool("Running", value: false);
			}
			else
			{
				anim.SetBool("Running", value: true);
			}
		}
		else if (nma == null)
		{
			nma = zmb.nma;
		}
	}

	public void Swing()
	{
		if (aud == null)
		{
			aud = GetComponentInChildren<SwingCheck2>().GetComponent<AudioSource>();
		}
		if (nma == null)
		{
			nma = zmb.nma;
		}
		zmb.stopped = true;
		anim.speed = 1f;
		track = true;
		coolDown = defaultCoolDown;
		if (nma.enabled)
		{
			nma.isStopped = true;
		}
		anim.SetTrigger("Swing");
		Object.Instantiate(swingSound, base.transform);
	}

	public void SwingEnd()
	{
		if (nma.isOnNavMesh)
		{
			nma.isStopped = false;
		}
		zmb.stopped = false;
	}

	public void DamageStart()
	{
		damaging = true;
		aud.Play();
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = true;
		if (swingCheck == null)
		{
			swingCheck = GetComponentInChildren<SwingCheck2>();
		}
		swingCheck.damage = 30;
		swingCheck.enemyDamage = 10;
		swingCheck.DamageStart();
		MouthClose();
	}

	public void DamageEnd()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		damaging = false;
		zmb.attacking = false;
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
		tr.enabled = false;
		if (swingCheck == null)
		{
			swingCheck = GetComponentInChildren<SwingCheck2>();
		}
		swingCheck.DamageStop();
	}

	public void StopTracking()
	{
		track = false;
		zmb.attacking = true;
	}

	public void CancelAttack()
	{
		damaging = false;
		zmb.attacking = false;
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = false;
		tr.enabled = false;
		zmb.stopped = false;
		track = false;
		coolDown = defaultCoolDown;
	}

	public void TrackTick()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (nma == null)
			{
				nma = zmb.nma;
			}
			if (zmb.grounded && nma != null && nma.enabled && zmb.target != null)
			{
				if (Physics.Raycast(zmb.target.position + Vector3.up * 0.1f, Vector3.down, out var hitInfo, float.PositiveInfinity, lmask))
				{
					nma.SetDestination(hitInfo.point);
				}
				else
				{
					nma.SetDestination(zmb.target.position);
				}
			}
		}
		Invoke("TrackTick", 0.1f);
	}

	public void MouthClose()
	{
		smr.sharedMaterial = biteMaterial;
		CancelInvoke("MouthOpen");
		Invoke("MouthOpen", 1f);
	}

	private void MouthOpen()
	{
		smr.sharedMaterial = origMaterial;
	}
}
