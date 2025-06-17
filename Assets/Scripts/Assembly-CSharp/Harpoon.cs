using UnityEngine;

public class Harpoon : MonoBehaviour
{
	public bool magnet;

	public bool drill;

	private bool drilling;

	private float drillCooldown;

	private bool hit;

	private bool stopped;

	public float damage;

	private float damageLeft;

	private AudioSource aud;

	public AudioClip environmentHitSound;

	public AudioClip enemyHitSound;

	private Rigidbody rb;

	private EnemyIdentifierIdentifier target;

	public GameObject drillSound;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		damageLeft = damage;
	}

	private void Update()
	{
		if (!stopped)
		{
			base.transform.LookAt(base.transform.position + rb.velocity);
		}
		else if (drilling && (bool)target)
		{
			base.transform.Rotate(Vector3.forward, 14400f * Time.deltaTime);
			if (drillCooldown != 0f)
			{
				drillCooldown = Mathf.MoveTowards(drillCooldown, 0f, Time.deltaTime);
				return;
			}
			drillCooldown = 0.025f;
			target.eid.hitter = "drill";
			target.eid.DeliverDamage(target.gameObject, Vector3.zero, base.transform.position, 0.15f, tryForExplode: false, 0f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GoreZone componentInParent = other.GetComponentInParent<GoreZone>();
		if (!hit && (other.gameObject.layer == 10 || other.gameObject.layer == 11) && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb"))
		{
			hit = true;
			target = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			EnemyIdentifier eid = target.eid;
			eid.hitter = "harpoon";
			eid.DeliverDamage(other.gameObject, Vector3.zero, base.transform.position, damageLeft, tryForExplode: false, 0f);
			damageLeft /= 3f;
			if (other.gameObject.layer == 10)
			{
				base.gameObject.AddComponent<FixedJoint>().connectedBody = other.gameObject.GetComponentInParent<Rigidbody>();
				if (componentInParent != null)
				{
					base.transform.SetParent(componentInParent.transform, worldPositionStays: true);
				}
			}
			else
			{
				rb.velocity = Vector3.zero;
				rb.useGravity = false;
				rb.constraints = RigidbodyConstraints.FreezeAll;
				base.transform.SetParent(other.transform, worldPositionStays: true);
			}
			if (!magnet && eid.dead && other.gameObject.layer == 10)
			{
				other.gameObject.transform.position = base.transform.position;
				GetComponent<Rigidbody>().AddForce(base.transform.forward, ForceMode.VelocityChange);
			}
			else
			{
				stopped = true;
				if (drill)
				{
					drilling = true;
					Object.Instantiate(drillSound, base.transform.position, base.transform.rotation).transform.SetParent(base.transform, worldPositionStays: true);
				}
				rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				GetComponent<TrailRenderer>().emitting = false;
				TimeBomb component = GetComponent<TimeBomb>();
				if (component != null)
				{
					component.StartCountdown();
				}
				Magnet componentInChildren = GetComponentInChildren<Magnet>();
				if (componentInChildren != null)
				{
					componentInChildren.onEnemy = true;
					componentInChildren.ignoredEids.Add(eid);
				}
			}
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = enemyHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.4f;
			aud.Play();
		}
		else
		{
			if (stopped || (other.gameObject.layer != 8 && other.gameObject.layer != 24))
			{
				return;
			}
			stopped = true;
			hit = true;
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rb.isKinematic = true;
			if (other.gameObject.tag == "Door" || other.gameObject.tag == "Moving")
			{
				base.gameObject.AddComponent<FixedJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
				rb.isKinematic = false;
				hit = true;
				base.transform.SetParent(other.transform, worldPositionStays: true);
			}
			else if ((bool)componentInParent)
			{
				base.transform.SetParent(componentInParent.transform, worldPositionStays: true);
			}
			else
			{
				GoreZone[] array = Object.FindObjectsOfType<GoreZone>();
				if (array != null && array.Length != 0)
				{
					GoreZone goreZone = array[0];
					if (array.Length > 1)
					{
						for (int i = 1; i < array.Length; i++)
						{
							if (array[i].gameObject.activeInHierarchy && Vector3.Distance(goreZone.transform.position, base.transform.position) > Vector3.Distance(array[i].transform.position, base.transform.position))
							{
								goreZone = array[i];
							}
						}
					}
					base.transform.SetParent(goreZone.transform, worldPositionStays: true);
				}
			}
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = environmentHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.4f;
			aud.Play();
			GetComponent<TrailRenderer>().emitting = false;
			TimeBomb component2 = GetComponent<TimeBomb>();
			if (component2 != null)
			{
				component2.StartCountdown();
			}
		}
	}
}
