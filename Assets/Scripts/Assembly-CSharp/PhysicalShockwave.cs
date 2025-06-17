using System.Collections.Generic;
using UnityEngine;

public class PhysicalShockwave : MonoBehaviour
{
	public int damage;

	public float speed;

	public float maxSize;

	public float force;

	public bool hasHurtPlayer;

	public bool enemy;

	private List<Collider> hitColliders = new List<Collider>();

	public EnemyType enemyType;

	public GameObject soundEffect;

	private void Start()
	{
		if (soundEffect != null)
		{
			Object.Instantiate(soundEffect, base.transform.position, Quaternion.identity);
		}
	}

	private void Update()
	{
		base.transform.localScale = new Vector3(base.transform.localScale.x + Time.deltaTime * speed, base.transform.localScale.y, base.transform.localScale.z + Time.deltaTime * speed);
		if (base.transform.localScale.x > maxSize || base.transform.localScale.z > maxSize)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!hasHurtPlayer && collision.gameObject.tag == "Player" && collision.gameObject.layer != 15)
		{
			hasHurtPlayer = true;
			NewMovement componentInParent = collision.gameObject.GetComponentInParent<NewMovement>();
			componentInParent.GetHurt(damage, invincible: true);
			componentInParent.Launch(componentInParent.transform.position + Vector3.down, 30f, 30f);
		}
		else
		{
			if (collision.gameObject.layer != 10)
			{
				return;
			}
			EnemyIdentifierIdentifier component = collision.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || !(component.eid != null) || (enemy && component.eid.type == enemyType))
			{
				return;
			}
			if (!hitColliders.Contains(component.eid.GetComponent<Collider>()) && !component.eid.dead)
			{
				hitColliders.Add(component.eid.GetComponent<Collider>());
				if (enemy)
				{
					component.eid.hitter = "enemy";
				}
				else
				{
					component.eid.hitter = "explosion";
				}
				component.eid.DeliverDamage(collision.gameObject, Vector3.up * force, collision.transform.position, (float)damage / 10f, tryForExplode: false, 0f);
			}
			else if (component.eid.dead)
			{
				hitColliders.Add(collision.gameObject.GetComponent<Collider>());
				component.eid.hitter = "explosion";
				component.eid.DeliverDamage(collision.gameObject, Vector3.up * 2000f, collision.transform.position, (float)damage / 10f, tryForExplode: false, 0f);
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (!hasHurtPlayer && collision.gameObject.tag == "Player" && collision.gameObject.layer != 15)
		{
			hasHurtPlayer = true;
			NewMovement componentInParent = collision.gameObject.GetComponentInParent<NewMovement>();
			componentInParent.GetHurt(damage, invincible: true);
			componentInParent.Launch(componentInParent.transform.position + Vector3.down, 30f, 30f);
		}
		else
		{
			if (collision.gameObject.layer != 10)
			{
				return;
			}
			EnemyIdentifierIdentifier component = collision.gameObject.GetComponent<EnemyIdentifierIdentifier>();
			if (!(component != null) || !(component.eid != null) || (enemy && component.eid.type == enemyType))
			{
				return;
			}
			Collider component2 = component.eid.GetComponent<Collider>();
			if (component2 != null && !hitColliders.Contains(component2) && !component.eid.dead)
			{
				hitColliders.Add(component2);
				if (enemy)
				{
					component.eid.hitter = "enemy";
				}
				else
				{
					component.eid.hitter = "explosion";
				}
				component.eid.DeliverDamage(collision.gameObject, Vector3.up * force, collision.transform.position, (float)damage / 10f, tryForExplode: false, 0f);
			}
			else if (component2 != null && component.eid.dead)
			{
				hitColliders.Add(component2);
				component.eid.hitter = "explosion";
				component.eid.DeliverDamage(collision.gameObject, Vector3.up * 2000f, collision.transform.position, (float)damage / 10f, tryForExplode: false, 0f);
			}
		}
	}
}
