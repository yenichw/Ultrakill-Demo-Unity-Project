using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	private Rigidbody[] rbs;

	public bool limp;

	public GameObject player;

	public NavMeshAgent nma;

	private Animator anim;

	private float currentSpeed;

	public float coolDown;

	public bool damaging;

	private Rigidbody rb;

	private TrailRenderer tr;

	private bool track;

	private AudioSource aud;

	private GroundCheck gc;

	public bool grounded;

	private float defaultSpeed;

	public Vector3 agentVelocity;

	private void Start()
	{
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		gc = GetComponentInChildren<GroundCheck>();
		nma.radius = Random.Range(2, 5);
		if (spawnIn)
		{
			Object.Instantiate(position: new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), original: spawnEffect, rotation: base.transform.rotation);
		}
		defaultSpeed = nma.speed;
	}

	private void FixedUpdate()
	{
		if (grounded && nma != null && nma.enabled)
		{
			nma.SetDestination(player.transform.position);
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.SetBool("Running", value: false);
			}
			else
			{
				anim.SetBool("Running", value: true);
			}
		}
		if (grounded && nma.isOnOffMeshLink)
		{
			grounded = false;
			nma.speed = 8f;
			base.transform.position = base.transform.position + Vector3.up + base.transform.forward;
			rb.AddForce(base.transform.up + agentVelocity * 10f, ForceMode.VelocityChange);
		}
		else if (!grounded && gc.onGround)
		{
			grounded = true;
			nma.speed = defaultSpeed;
		}
		_ = grounded;
	}

	private void Update()
	{
		if (damaging)
		{
			base.transform.Translate(Vector3.forward * 18f * Time.deltaTime, Space.Self);
		}
		if (grounded && nma != null && nma.enabled)
		{
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.speed = 1f;
			}
			else
			{
				anim.speed = nma.velocity.magnitude / nma.speed;
			}
		}
		if (coolDown != 0f)
		{
			if (coolDown - Time.deltaTime > 0f)
			{
				coolDown -= Time.deltaTime;
			}
			else
			{
				coolDown = 0f;
			}
		}
		if (anim != null)
		{
			anim.SetFloat("Speed", anim.speed);
		}
		if (track)
		{
			base.transform.LookAt(new Vector3(player.transform.position.x, base.transform.position.y, player.transform.position.z));
		}
	}

	public void GoLimp()
	{
		track = false;
		Object.Destroy(nma);
		nma = null;
		Object.Destroy(anim);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		Object.Destroy(base.gameObject.GetComponentInChildren<SwingCheck>());
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		Object.Destroy(rb);
		if (!limp)
		{
			Rigidbody[] array = rbs;
			foreach (Rigidbody obj in array)
			{
				obj.isKinematic = false;
				obj.useGravity = true;
			}
		}
		if (!limp)
		{
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
		}
		limp = true;
	}

	public void Swing()
	{
		if (aud == null)
		{
			aud = GetComponentInChildren<SwingCheck>().GetComponent<AudioSource>();
		}
		track = true;
		coolDown = 1.5f;
		nma.isStopped = true;
		anim.SetTrigger("Swing");
	}

	public void SwingEnd()
	{
		nma.isStopped = false;
		tr.enabled = false;
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
	}

	public void DamageEnd()
	{
		damaging = false;
	}

	public void StopTracking()
	{
		track = false;
	}
}
