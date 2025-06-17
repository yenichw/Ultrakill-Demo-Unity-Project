using System.Collections.Generic;
using UnityEngine;

public class SwingCheck : MonoBehaviour
{
	private EnemyIdentifier eid;

	public EnemyType type;

	public bool playerOnly;

	private ZombieMelee zombie;

	private SwordsMachine swm;

	private StatueBoss sb;

	private bool playerInZone;

	private bool playerBeenHit;

	private NewMovement nmov;

	public int damage;

	public LayerMask lmask;

	private List<EnemyIdentifier> hitEnemies = new List<EnemyIdentifier>();

	public bool strong;

	public float pulseTime = 0.1f;

	private List<EnemyIdentifierIdentifier> inZone = new List<EnemyIdentifierIdentifier>();

	private List<Collider> zoneLimb = new List<Collider>();

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
		type = eid.type;
		swm = GetComponentInParent<SwordsMachine>();
	}

	private void FixedUpdate()
	{
		if (pulseTime <= 0f)
		{
			if (inZone.Count > 0 || playerInZone)
			{
				Pulse();
			}
		}
		else
		{
			pulseTime = Mathf.MoveTowards(pulseTime, 0f, 0.001f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerInZone = true;
			if (type == EnemyType.Machine)
			{
				if (swm == null)
				{
					swm = GetComponentInParent<SwordsMachine>();
				}
				if (swm.damaging && !playerBeenHit && other.gameObject.layer != 15)
				{
					if (nmov == null)
					{
						nmov = other.GetComponent<NewMovement>();
					}
					nmov.GetHurt(swm.damage, invincible: true);
					playerBeenHit = true;
				}
			}
			else if (type == EnemyType.Zombie)
			{
				if (zombie == null)
				{
					zombie = GetComponentInParent<ZombieMelee>();
				}
				if (zombie.damaging && !playerBeenHit && other.gameObject.layer != 15)
				{
					if (nmov == null)
					{
						nmov = other.GetComponent<NewMovement>();
					}
					nmov.GetHurt(30, invincible: true);
					playerBeenHit = true;
				}
				else if (zombie.coolDown == 0f)
				{
					zombie.Swing();
				}
			}
			else
			{
				if (type != EnemyType.Statue)
				{
					return;
				}
				if (sb == null)
				{
					sb = GetComponentInParent<StatueBoss>();
				}
				if (sb.damaging && !playerBeenHit && other.gameObject.layer != 15)
				{
					if (nmov == null)
					{
						nmov = other.GetComponent<NewMovement>();
					}
					nmov.GetHurt(sb.damage, invincible: true);
					playerBeenHit = true;
					if (sb.launching)
					{
						nmov.Launch(nmov.transform.position + sb.transform.forward * -1f, 100f, 100f);
					}
				}
			}
		}
		else
		{
			if (other.gameObject.layer != 10 || playerOnly)
			{
				return;
			}
			EnemyIdentifierIdentifier component = other.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || !(component.eid != null) || component.eid.type == type)
			{
				return;
			}
			if (!inZone.Contains(component))
			{
				inZone.Add(component);
				zoneLimb.Add(other);
			}
			if (type == EnemyType.Machine)
			{
				if (swm == null)
				{
					swm = GetComponentInParent<SwordsMachine>();
				}
				if (swm.damaging)
				{
					EnemyIdentifier enemyIdentifier = component.eid;
					if ((!hitEnemies.Contains(enemyIdentifier) || (enemyIdentifier.dead && other.gameObject.tag == "Head")) && (!enemyIdentifier.dead || (enemyIdentifier.dead && other.gameObject.tag != "Body")))
					{
						damage = swm.damage;
						enemyIdentifier.hitter = "enemy";
						enemyIdentifier.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 10000f, other.transform.position, damage / 10, tryForExplode: false, 0f);
						hitEnemies.Add(enemyIdentifier);
					}
				}
				else if (hitEnemies.Count > 0)
				{
					hitEnemies.Clear();
				}
			}
			if (type == EnemyType.Zombie)
			{
				if (zombie == null)
				{
					zombie = GetComponentInParent<ZombieMelee>();
				}
				else
				{
					if (zombie.coolDown == 0f && !component.eid.dead)
					{
						zombie.Swing();
					}
					if (zombie.damaging)
					{
						zombie.damaging = false;
						EnemyIdentifier enemyIdentifier2 = component.eid;
						enemyIdentifier2.hitter = "enemy";
						enemyIdentifier2.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 10000f, other.transform.position, 1f, tryForExplode: false, 0f);
					}
				}
			}
			if (type != EnemyType.Statue)
			{
				return;
			}
			if (sb == null)
			{
				sb = GetComponentInParent<StatueBoss>();
			}
			else if (sb.damaging)
			{
				EnemyIdentifier enemyIdentifier3 = component.eid;
				if ((!hitEnemies.Contains(enemyIdentifier3) || (enemyIdentifier3.dead && other.gameObject.tag == "Head")) && (!enemyIdentifier3.dead || (enemyIdentifier3.dead && other.gameObject.tag != "Body")))
				{
					damage = sb.damage;
					enemyIdentifier3.hitter = "enemy";
					enemyIdentifier3.DeliverDamage(other.gameObject, ((base.transform.position - other.transform.position).normalized + Vector3.up) * 10000f, other.transform.position, damage / 10, tryForExplode: false, 0f);
					hitEnemies.Add(enemyIdentifier3);
				}
			}
			else if (hitEnemies.Count > 0)
			{
				hitEnemies.Clear();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerInZone = false;
		}
		if (other.gameObject.layer == 10 && !playerOnly && zoneLimb.Contains(other))
		{
			int index = zoneLimb.IndexOf(other);
			inZone.RemoveAt(index);
			zoneLimb.Remove(other);
		}
	}

	private void Pulse()
	{
		pulseTime = 0.1f;
		CheckZone();
	}

	public void DamageStop()
	{
		playerBeenHit = false;
	}

	public void CheckZone()
	{
		if (playerInZone)
		{
			if (nmov == null)
			{
				nmov = Object.FindObjectOfType<NewMovement>();
			}
			if (type == EnemyType.Machine)
			{
				if (swm == null)
				{
					swm = GetComponentInParent<SwordsMachine>();
				}
				if (swm.damaging && !playerBeenHit && nmov.gameObject.layer != 15)
				{
					nmov.GetHurt(swm.damage, invincible: true);
					playerBeenHit = true;
				}
				else if (!swm.damaging)
				{
					playerBeenHit = false;
				}
			}
			else if (type == EnemyType.Zombie)
			{
				if (zombie == null)
				{
					zombie = GetComponentInParent<ZombieMelee>();
				}
				if (zombie.damaging)
				{
					Debug.Log("Step 1");
				}
				if (zombie.damaging && !playerBeenHit && nmov.gameObject.layer != 15)
				{
					Debug.Log("Step 2");
					nmov.GetHurt(30, invincible: true);
					playerBeenHit = true;
				}
				else if (!zombie.damaging)
				{
					Debug.Log("Step 3");
					playerBeenHit = false;
					if (zombie.coolDown == 0f)
					{
						zombie.Swing();
					}
				}
			}
			else if (type == EnemyType.Statue)
			{
				if (sb == null)
				{
					sb = GetComponentInParent<StatueBoss>();
				}
				if (sb.damaging && !playerBeenHit && nmov.gameObject.layer != 15)
				{
					nmov.GetHurt(sb.damage, invincible: true);
					playerBeenHit = true;
					if (sb.launching)
					{
						nmov.Launch(nmov.transform.position + sb.transform.forward * -1f, 100f, 100f);
					}
				}
				else if (!sb.damaging)
				{
					playerBeenHit = false;
				}
			}
		}
		for (int i = 0; i < inZone.Count; i++)
		{
			List<int> list = new List<int>();
			EnemyIdentifierIdentifier enemyIdentifierIdentifier = inZone[i];
			Collider collider = zoneLimb[i];
			if (enemyIdentifierIdentifier == null || collider == null || enemyIdentifierIdentifier.eid == null || enemyIdentifierIdentifier.eid.type == type)
			{
				list.Add(i);
			}
			else
			{
				if (type == EnemyType.Machine)
				{
					if (swm == null)
					{
						swm = GetComponentInParent<SwordsMachine>();
					}
					if (swm.damaging)
					{
						EnemyIdentifier enemyIdentifier = enemyIdentifierIdentifier.eid;
						if ((!hitEnemies.Contains(enemyIdentifier) || (enemyIdentifier.dead && collider.gameObject.tag == "Head")) && (!enemyIdentifier.dead || (enemyIdentifier.dead && collider.gameObject.tag != "Body")))
						{
							damage = swm.damage;
							enemyIdentifier.hitter = "enemy";
							enemyIdentifier.DeliverDamage(collider.gameObject, (base.transform.position - collider.transform.position).normalized * 10000f, collider.transform.position, damage / 10, tryForExplode: false, 0f);
							hitEnemies.Add(enemyIdentifier);
						}
					}
					else if (hitEnemies.Count > 0)
					{
						hitEnemies.Clear();
					}
				}
				if (type == EnemyType.Zombie)
				{
					if (zombie == null)
					{
						zombie = GetComponentInParent<ZombieMelee>();
					}
					else
					{
						if (zombie.coolDown == 0f && !enemyIdentifierIdentifier.eid.dead)
						{
							zombie.Swing();
						}
						if (zombie.damaging)
						{
							zombie.damaging = false;
							EnemyIdentifier enemyIdentifier2 = enemyIdentifierIdentifier.eid;
							enemyIdentifier2.hitter = "enemy";
							enemyIdentifier2.DeliverDamage(collider.gameObject, (base.transform.position - collider.transform.position).normalized * 10000f, collider.transform.position, 1f, tryForExplode: false, 0f);
						}
					}
				}
				if (type == EnemyType.Statue)
				{
					if (sb == null)
					{
						sb = GetComponentInParent<StatueBoss>();
					}
					else if (sb.damaging)
					{
						EnemyIdentifier enemyIdentifier3 = enemyIdentifierIdentifier.eid;
						if ((!hitEnemies.Contains(enemyIdentifier3) || (enemyIdentifier3.dead && collider.gameObject.tag == "Head")) && (!enemyIdentifier3.dead || (enemyIdentifier3.dead && collider.gameObject.tag != "Body")))
						{
							damage = sb.damage;
							enemyIdentifier3.hitter = "enemy";
							enemyIdentifier3.DeliverDamage(collider.gameObject, ((base.transform.position - collider.transform.position).normalized + Vector3.up) * 10000f, collider.transform.position, damage / 10, tryForExplode: false, 0f);
							hitEnemies.Add(enemyIdentifier3);
						}
					}
					else if (hitEnemies.Count > 0)
					{
						hitEnemies.Clear();
					}
				}
			}
			if (list.Count <= 0)
			{
				continue;
			}
			foreach (int item in list)
			{
				_ = item;
				zoneLimb.RemoveAt(i);
				inZone.RemoveAt(i);
			}
			list.Clear();
		}
	}
}
