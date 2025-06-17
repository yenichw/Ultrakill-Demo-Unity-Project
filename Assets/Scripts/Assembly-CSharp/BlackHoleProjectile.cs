using System.Collections.Generic;
using UnityEngine;

public class BlackHoleProjectile : MonoBehaviour
{
	private Rigidbody rb;

	public float speed;

	private Light bhlight;

	private float targetRange;

	private RaycastHit rhit;

	public LayerMask lmask;

	private AudioSource aud;

	public GameObject lightningBolt;

	public GameObject lightningBolt2;

	private Transform aura;

	public Material additive;

	private bool activated;

	private bool collapsing;

	private float power;

	private StyleCalculator scalc;

	private int killAmount;

	public List<EnemyIdentifier> shootList = new List<EnemyIdentifier>();

	private List<Rigidbody> caughtList = new List<Rigidbody>();

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		bhlight = GetComponent<Light>();
		targetRange = Random.Range(0, 15);
		aura = base.transform.GetChild(0);
		aud = GetComponent<AudioSource>();
		Invoke("ShootRandomLightning", Random.Range(0.5f, 1.5f));
	}

	private void FixedUpdate()
	{
		if (!activated)
		{
			rb.velocity = base.transform.forward * speed;
		}
	}

	private void Update()
	{
		if (bhlight.range != targetRange)
		{
			bhlight.range = Mathf.MoveTowards(bhlight.range, targetRange, 100f * Time.deltaTime);
		}
		else if (activated)
		{
			targetRange = Random.Range(10, 20);
		}
		else
		{
			targetRange = Random.Range(0, 15);
		}
		if (activated)
		{
			aura.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
		}
		else
		{
			aura.transform.localPosition = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
		}
		if (collapsing)
		{
			if (aud.pitch > 0f)
			{
				aud.pitch -= Time.deltaTime;
			}
			else if (aud.pitch != 0f)
			{
				aud.pitch = 0f;
			}
			foreach (Rigidbody caught in caughtList)
			{
				if (caught != null)
				{
					caught.transform.position = base.transform.position;
				}
			}
			if (base.transform.localScale.x > 0f)
			{
				base.transform.localScale -= Vector3.one * Time.deltaTime;
				return;
			}
			EnemyIdentifierIdentifier[] componentsInChildren = GetComponentsInChildren<EnemyIdentifierIdentifier>();
			foreach (EnemyIdentifierIdentifier enemyIdentifierIdentifier in componentsInChildren)
			{
				if (!(enemyIdentifierIdentifier.gameObject.tag == "EndLimb") && !(enemyIdentifierIdentifier.gameObject.tag == "Head"))
				{
					continue;
				}
				enemyIdentifierIdentifier.eid.DeliverDamage(enemyIdentifierIdentifier.gameObject, Vector3.zero, enemyIdentifierIdentifier.gameObject.transform.position, 100f, tryForExplode: false, 0f);
				if (!enemyIdentifierIdentifier.eid.exploded)
				{
					enemyIdentifierIdentifier.eid.exploded = true;
					if (scalc == null)
					{
						scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
					}
					killAmount++;
					scalc.shud.AddPoints(50 - killAmount * 10, "COMPRESSED");
					scalc.HitCalculator("", "", "", dead: true, base.gameObject);
				}
			}
			Object.Destroy(base.gameObject);
		}
		else
		{
			if (!activated)
			{
				return;
			}
			aud.pitch += Time.deltaTime / 2f;
			power += Time.deltaTime;
			if (caughtList.Count == 0)
			{
				return;
			}
			List<Rigidbody> list = new List<Rigidbody>();
			foreach (Rigidbody caught2 in caughtList)
			{
				if (caught2 == null)
				{
					list.Add(caught2);
					continue;
				}
				if (Vector3.Distance(caught2.transform.position, base.transform.position) < 9f)
				{
					caught2.transform.position = Vector3.MoveTowards(caught2.transform.position, base.transform.position, power * Time.deltaTime * (10f - Vector3.Distance(caught2.transform.position, base.transform.position)));
				}
				else
				{
					caught2.transform.position = Vector3.MoveTowards(caught2.transform.position, base.transform.position, power * Time.deltaTime);
				}
				if (Vector3.Distance(caught2.transform.position, base.transform.position) < 1f)
				{
					CharacterJoint component = caught2.GetComponent<CharacterJoint>();
					if (component != null)
					{
						Object.Destroy(component);
					}
					caught2.GetComponent<Collider>().enabled = false;
				}
				if (!(Vector3.Distance(caught2.transform.position, base.transform.position) < 0.25f))
				{
					continue;
				}
				List<Rigidbody> list2 = new List<Rigidbody>();
				list.Add(caught2);
				caught2.useGravity = false;
				caught2.velocity = Vector3.zero;
				caught2.isKinematic = true;
				caught2.transform.SetParent(base.transform);
				caught2.transform.localPosition = Vector3.zero;
				if (list2.Count != 0)
				{
					foreach (Rigidbody item in list2)
					{
						caughtList.Remove(item);
					}
				}
				list2.Clear();
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (Rigidbody item2 in list)
			{
				caughtList.Remove(item2);
			}
		}
	}

	private void ShootRandomLightning()
	{
		int num = Random.Range(2, 6);
		for (int i = 0; i < num; i++)
		{
			if (Physics.Raycast(base.transform.position, Random.insideUnitSphere.normalized, out rhit, 8f, lmask))
			{
				LineRenderer component = Object.Instantiate(lightningBolt, base.transform.position, base.transform.rotation).GetComponent<LineRenderer>();
				component.SetPosition(0, base.transform.position);
				component.SetPosition(1, rhit.point);
			}
		}
		if (!activated)
		{
			Invoke("ShootRandomLightning", Random.Range(0.5f, 3f));
		}
	}

	private void ShootTargetLightning()
	{
		if (shootList.Count != 0)
		{
			List<EnemyIdentifier> list = new List<EnemyIdentifier>();
			foreach (EnemyIdentifier shoot in shootList)
			{
				LineRenderer component = Object.Instantiate(lightningBolt2, base.transform.position, base.transform.rotation).GetComponent<LineRenderer>();
				component.SetPosition(0, base.transform.position);
				component.SetPosition(1, shoot.transform.position);
				shoot.hitter = "secret";
				shoot.DeliverDamage(shoot.GetComponentInChildren<EnemyIdentifierIdentifier>().gameObject, Vector3.zero, shoot.transform.position, 1f, tryForExplode: false, 0f);
				if (shoot.dead)
				{
					list.Add(shoot);
					Rigidbody[] componentsInChildren = shoot.GetComponentsInChildren<Rigidbody>();
					foreach (Rigidbody item in componentsInChildren)
					{
						caughtList.Add(item);
					}
				}
			}
			if (list.Count != 0)
			{
				foreach (EnemyIdentifier item2 in list)
				{
					shootList.Remove(item2);
				}
				list.Clear();
			}
		}
		ShootRandomLightning();
		Invoke("ShootTargetLightning", 0.5f);
	}

	public void Activate()
	{
		activated = true;
		rb.velocity = Vector3.zero;
		base.transform.GetChild(0).GetComponent<SpriteRenderer>().material = additive;
		GetComponentInChildren<ParticleSystem>().Play();
		ShootTargetLightning();
		Invoke("Collapse", 3f);
	}

	private void Collapse()
	{
		collapsing = true;
	}
}
