using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
	public bool trigger;

	public bool burn;

	public float damage;

	private bool hurtingPlayer;

	private float playerHurtCooldown;

	private List<EnemyIdentifier> hurtList = new List<EnemyIdentifier>();

	private List<EnemyIdentifier> toRemove = new List<EnemyIdentifier>();

	private List<float> hurtTimes = new List<float>();

	private NewMovement nmov;

	private void FixedUpdate()
	{
		if (hurtingPlayer && playerHurtCooldown <= 0f)
		{
			if (!nmov.dead)
			{
				nmov.GetHurt((int)damage, invincible: false);
			}
			else
			{
				hurtingPlayer = false;
			}
			playerHurtCooldown = 1f;
		}
		else if (playerHurtCooldown > 0f)
		{
			playerHurtCooldown -= Time.deltaTime;
		}
		if (hurtList.Count <= 0)
		{
			return;
		}
		foreach (EnemyIdentifier hurt in hurtList)
		{
			if (hurt != null)
			{
				float num = hurtTimes[hurtList.IndexOf(hurt)];
				num -= Time.deltaTime;
				if (num <= 0f)
				{
					if (burn)
					{
						hurt.hitter = "fire";
					}
					hurt.DeliverDamage(hurt.gameObject, Vector3.zero, hurt.transform.position, damage / 2f, tryForExplode: false, 0f);
					if (burn && !hurt.dead)
					{
						Flammable componentInChildren = hurt.GetComponentInChildren<Flammable>();
						if (componentInChildren != null)
						{
							componentInChildren.Burn(4f);
						}
					}
					num = 1f;
				}
				hurtTimes[hurtList.IndexOf(hurt)] = num;
			}
			else
			{
				toRemove.Add(hurt);
			}
		}
		if (toRemove.Count <= 0)
		{
			return;
		}
		foreach (EnemyIdentifier item in toRemove)
		{
			hurtList.Remove(item);
		}
		toRemove.Clear();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!trigger)
		{
			return;
		}
		if (other.gameObject.tag == "Player" && !hurtingPlayer)
		{
			hurtingPlayer = true;
			if (nmov == null)
			{
				nmov = other.gameObject.GetComponentInParent<NewMovement>();
			}
		}
		else
		{
			if ((other.gameObject.layer != 10 || !(other.gameObject.tag != "Body")) && other.gameObject.layer != 11)
			{
				return;
			}
			EnemyIdentifierIdentifier component = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || !(component.eid != null) || hurtList.Contains(component.eid))
			{
				return;
			}
			hurtList.Add(component.eid);
			hurtTimes.Add(1f);
			if (burn)
			{
				component.eid.hitter = "fire";
			}
			component.eid.DeliverDamage(other.gameObject, Vector3.zero, other.transform.position, damage / 2f, tryForExplode: false, 0f);
			if (burn)
			{
				Flammable componentInChildren = component.eid.GetComponentInChildren<Flammable>();
				if (componentInChildren != null)
				{
					componentInChildren.Burn(4f);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!trigger)
		{
			return;
		}
		if (other.gameObject.tag == "Player")
		{
			hurtingPlayer = false;
		}
		else if (other.gameObject.layer == 10 || other.gameObject.layer == 11)
		{
			EnemyIdentifierIdentifier component = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (component != null && hurtList.Contains(component.eid))
			{
				hurtTimes.RemoveAt(hurtList.IndexOf(component.eid));
				hurtList.Remove(component.eid);
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (trigger)
		{
			return;
		}
		if (other.gameObject.tag == "Player" && !hurtingPlayer)
		{
			hurtingPlayer = true;
			if (nmov == null)
			{
				nmov = other.gameObject.GetComponentInParent<NewMovement>();
			}
		}
		else
		{
			if ((other.gameObject.layer != 10 || !(other.gameObject.tag != "Body")) && other.gameObject.layer != 11)
			{
				return;
			}
			EnemyIdentifierIdentifier component = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || hurtList.Contains(component.eid))
			{
				return;
			}
			hurtList.Add(component.eid);
			hurtTimes.Add(1f);
			if (burn)
			{
				component.eid.hitter = "fire";
			}
			component.eid.DeliverDamage(other.gameObject, Vector3.zero, other.transform.position, damage / 2f, tryForExplode: false, 0f);
			if (burn)
			{
				Flammable componentInChildren = component.eid.GetComponentInChildren<Flammable>();
				if (componentInChildren != null)
				{
					componentInChildren.Burn(4f);
				}
			}
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (trigger)
		{
			return;
		}
		if (other.gameObject.tag == "Player")
		{
			hurtingPlayer = false;
		}
		else if (other.gameObject.layer == 10 || other.gameObject.layer == 11)
		{
			EnemyIdentifierIdentifier component = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (component != null && hurtList.Contains(component.eid))
			{
				hurtTimes.RemoveAt(hurtList.IndexOf(component.eid));
				hurtList.Remove(component.eid);
			}
		}
	}
}
