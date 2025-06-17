using System.Collections.Generic;
using UnityEngine;

public class SwingCheck2 : MonoBehaviour
{
	private EnemyIdentifier eid;

	public EnemyType type;

	public bool playerOnly;

	public bool playerBeenHit;

	private NewMovement nmov;

	public int damage;

	public int enemyDamage;

	public float knockBackForce;

	public LayerMask lmask;

	private List<EnemyIdentifier> hitEnemies = new List<EnemyIdentifier>();

	public bool strong;

	private Collider col;

	public bool useRaycastCheck;

	private AudioSource aud;

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
		type = eid.type;
		col = GetComponent<Collider>();
		col.enabled = false;
		aud = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log("Hit Player!!!");
			if (playerBeenHit || other.gameObject.layer == 15)
			{
				return;
			}
			bool flag = false;
			if (useRaycastCheck)
			{
				Vector3 vector = new Vector3(eid.transform.position.x, base.transform.position.y, eid.transform.position.z);
				if (Physics.Raycast(vector, other.transform.position - vector, Vector3.Distance(vector, other.transform.position), lmask))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (nmov == null)
				{
					nmov = other.GetComponent<NewMovement>();
				}
				nmov.GetHurt(damage, invincible: true);
				playerBeenHit = true;
				if (knockBackForce > 0f)
				{
					nmov.Launch(nmov.transform.position + base.transform.forward * -1f, knockBackForce, knockBackForce);
				}
				eid.SendMessage("PlayerBeenHit", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (other.gameObject.layer == 10 && !playerOnly && eid != null)
		{
			EnemyIdentifierIdentifier component = other.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || !(component.eid != null) || component.eid.type == type)
			{
				return;
			}
			EnemyIdentifier enemyIdentifier = component.eid;
			if ((hitEnemies.Contains(enemyIdentifier) && (!enemyIdentifier.dead || !(other.gameObject.tag == "Head"))) || (enemyIdentifier.dead && (!enemyIdentifier.dead || !(other.gameObject.tag != "Body"))))
			{
				return;
			}
			bool flag2 = false;
			if (useRaycastCheck)
			{
				Vector3 vector2 = new Vector3(eid.transform.position.x, base.transform.position.y, eid.transform.position.z);
				if (Physics.Raycast(vector2, other.transform.position - vector2, Vector3.Distance(vector2, other.transform.position), lmask))
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				enemyIdentifier.hitter = "enemy";
				if (enemyDamage == 0)
				{
					enemyDamage = damage;
				}
				enemyIdentifier.DeliverDamage(other.gameObject, ((base.transform.position - other.transform.position).normalized + Vector3.up) * 10000f, other.transform.position, enemyDamage / 10, tryForExplode: false, 0f);
				hitEnemies.Add(enemyIdentifier);
			}
		}
		else if (other.gameObject.tag == "Breakable")
		{
			Breakable component2 = other.gameObject.GetComponent<Breakable>();
			if (component2 != null && (strong || component2.weak) && !component2.playerOnly)
			{
				component2.Break();
			}
		}
	}

	public void DamageStart()
	{
		col.enabled = true;
		if (aud != null)
		{
			aud.Play();
		}
	}

	public void DamageStop()
	{
		playerBeenHit = false;
		if (hitEnemies.Count > 0)
		{
			hitEnemies.Clear();
		}
		col.enabled = false;
	}
}
