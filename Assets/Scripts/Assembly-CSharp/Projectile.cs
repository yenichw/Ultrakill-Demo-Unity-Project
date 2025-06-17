using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Rigidbody rb;

	public float speed;

	public float turnSpeed;

	public float speedRandomizer;

	private AudioSource aud;

	public GameObject explosionEffect;

	public float damage;

	public bool friendly;

	public bool playerBullet;

	public string bulletType;

	public string weaponType;

	public bool decorative;

	private Vector3 origScale;

	private bool active = true;

	public EnemyType safeEnemyType;

	public bool explosive;

	public bool bigExplosion;

	public HomingType homingType;

	public Transform target;

	private float maxSpeed;

	private Quaternion targetRotation;

	public bool hittingPlayer;

	private NewMovement nmov;

	public bool boosted;

	private Collider col;

	public bool undeflectable;

	public LayerMask lmask;

	public bool keepTrail;

	public bool strong;

	public bool spreaded;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		aud.pitch = Random.Range(1.8f, 2f);
		if (aud.enabled)
		{
			aud.Play();
		}
		if (decorative)
		{
			origScale = base.transform.localScale;
			base.transform.localScale = Vector3.zero;
		}
		if (speed != 0f)
		{
			speed += Random.Range(0f - speedRandomizer, speedRandomizer);
		}
		if (col == null)
		{
			col = GetComponent<Collider>();
		}
		maxSpeed = speed;
	}

	private void Update()
	{
		if (homingType != 0 && !hittingPlayer)
		{
			switch (homingType)
			{
			case HomingType.Loose:
			{
				maxSpeed += Time.deltaTime * 10f;
				base.transform.LookAt(base.transform.position + rb.velocity);
				Vector3 normalized = (target.position - base.transform.position).normalized;
				rb.AddForce(normalized * speed * Time.deltaTime * 200f);
				rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
				break;
			}
			case HomingType.Horizontal:
			{
				base.transform.LookAt(target.position + rb.velocity);
				float num = Mathf.Clamp(target.position.x - base.transform.position.x, -25f, 25f);
				float num2 = Mathf.Clamp(target.position.z - base.transform.position.z, -25f, 25f);
				float x = Mathf.MoveTowards(rb.velocity.x, num, Time.deltaTime * 15f);
				float z = Mathf.MoveTowards(rb.velocity.z, num2, Time.deltaTime * 15f);
				rb.velocity = new Vector3(x, rb.velocity.y, z);
				break;
			}
			default:
				maxSpeed += Time.deltaTime * 10f;
				targetRotation = Quaternion.LookRotation(target.position - base.transform.position);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				rb.velocity = base.transform.forward * maxSpeed;
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!hittingPlayer && !undeflectable && !decorative && speed != 0f && homingType == HomingType.None)
		{
			rb.velocity = base.transform.forward * speed;
		}
		if (decorative && base.transform.localScale.x < origScale.x)
		{
			aud.pitch = base.transform.localScale.x / origScale.x * 2.8f;
			base.transform.localScale = Vector3.Slerp(base.transform.localScale, origScale, Time.deltaTime * speed);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!friendly && other.gameObject.tag == "Player")
		{
			if (spreaded)
			{
				ProjectileSpread componentInParent = GetComponentInParent<ProjectileSpread>();
				if (componentInParent != null && componentInParent.parried)
				{
					return;
				}
			}
			if (explosive)
			{
				Explode();
				return;
			}
			hittingPlayer = true;
			rb.velocity = Vector3.zero;
			if (keepTrail)
			{
				TrailRenderer componentInChildren = GetComponentInChildren<TrailRenderer>();
				if (componentInChildren != null)
				{
					componentInChildren.transform.parent = null;
				}
			}
			base.transform.position = new Vector3(other.transform.position.x, base.transform.position.y, other.transform.position.z);
			nmov = other.gameObject.GetComponentInParent<NewMovement>();
			Invoke("RecheckPlayerHit", 0.05f);
		}
		else if (active && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb") && other.gameObject.tag != "Armor")
		{
			EnemyIdentifierIdentifier componentInParent2 = other.gameObject.GetComponentInParent<EnemyIdentifierIdentifier>();
			EnemyIdentifier enemyIdentifier = null;
			if (componentInParent2 != null && componentInParent2.eid != null)
			{
				enemyIdentifier = componentInParent2.eid;
			}
			if ((!(enemyIdentifier != null) || enemyIdentifier.type == safeEnemyType) && !friendly)
			{
				return;
			}
			active = false;
			bool tryForExplode = false;
			bool dead = enemyIdentifier.dead;
			if (playerBullet)
			{
				enemyIdentifier.hitter = bulletType;
				if (!enemyIdentifier.hitterWeapons.Contains(weaponType))
				{
					enemyIdentifier.hitterWeapons.Add(weaponType);
				}
			}
			else if (!friendly)
			{
				enemyIdentifier.hitter = "enemy";
			}
			else
			{
				enemyIdentifier.hitter = "projectile";
				tryForExplode = true;
			}
			if (boosted && !enemyIdentifier.dead)
			{
				Object.FindObjectOfType<StyleHUD>().AddPoints(200, "<color=lime>PROJECTILE BOOST</color>");
			}
			bool flag = true;
			if (spreaded)
			{
				ProjectileSpread componentInParent3 = GetComponentInParent<ProjectileSpread>();
				if (componentInParent3 != null)
				{
					if (componentInParent3.hitEnemies.Contains(enemyIdentifier))
					{
						flag = false;
					}
					else
					{
						componentInParent3.hitEnemies.Add(enemyIdentifier);
					}
				}
			}
			if (!explosive)
			{
				if (flag)
				{
					if (playerBullet)
					{
						enemyIdentifier.DeliverDamage(other.gameObject, rb.velocity.normalized * 2500f, base.transform.position, damage / 4f, tryForExplode, 0f);
					}
					else if (friendly)
					{
						enemyIdentifier.DeliverDamage(other.gameObject, rb.velocity.normalized * 10000f, base.transform.position, damage / 4f, tryForExplode, 0f);
					}
					else
					{
						enemyIdentifier.DeliverDamage(other.gameObject, rb.velocity.normalized * 100f, base.transform.position, damage / 10f, tryForExplode, 0f);
					}
				}
				Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			}
			else
			{
				Explode();
			}
			if (keepTrail)
			{
				GetComponentInChildren<TrailRenderer>().transform.parent = null;
			}
			if (!dead)
			{
				GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().HitStop(0.005f);
			}
			if (!dead || other.gameObject.layer == 11)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				active = true;
			}
		}
		else if (other.gameObject.tag == "Armor")
		{
			if (Physics.Raycast(base.transform.position - rb.velocity.normalized, base.transform.forward, out var hitInfo, float.PositiveInfinity, lmask))
			{
				base.transform.forward = Vector3.Reflect(base.transform.forward, hitInfo.normal);
				EnemyIdentifier componentInParent4 = other.GetComponentInParent<EnemyIdentifier>();
				if (componentInParent4 != null)
				{
					Object.Instantiate(componentInParent4.ineffectiveSound, base.transform.position, Quaternion.identity);
				}
			}
		}
		else if (!hittingPlayer && (other.gameObject.layer == 8 || other.gameObject.layer == 24))
		{
			Breakable component = other.gameObject.GetComponent<Breakable>();
			if (component != null && !component.precisionOnly && (component.weak || strong))
			{
				component.Break();
			}
			if (explosive)
			{
				Explode();
				return;
			}
			if (keepTrail)
			{
				TrailRenderer componentInChildren2 = GetComponentInChildren<TrailRenderer>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.transform.parent = null;
				}
			}
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
		else if (other.gameObject.layer == 0)
		{
			Rigidbody componentInParent5 = other.GetComponentInParent<Rigidbody>();
			if (componentInParent5 != null)
			{
				componentInParent5.AddForce(base.transform.forward * 1000f);
			}
		}
	}

	public void Explode()
	{
		if (!active)
		{
			return;
		}
		active = false;
		if (keepTrail)
		{
			GetComponentInChildren<TrailRenderer>().transform.parent = null;
		}
		Explosion[] componentsInChildren = Object.Instantiate(explosionEffect, base.transform.position - rb.velocity * 0.02f, base.transform.rotation).GetComponentsInChildren<Explosion>();
		foreach (Explosion explosion in componentsInChildren)
		{
			if (bigExplosion)
			{
				explosion.maxSize *= 1.5f;
			}
			if (explosion.damage != 0)
			{
				explosion.damage = Mathf.RoundToInt(damage);
			}
			explosion.enemy = true;
		}
		Object.Destroy(base.gameObject);
	}

	private void RecheckPlayerHit()
	{
		if (hittingPlayer)
		{
			hittingPlayer = false;
			col.enabled = false;
			undeflectable = true;
			Invoke("TimeToDie", 0.01f);
		}
	}

	private void TimeToDie()
	{
		bool flag = false;
		if (spreaded)
		{
			ProjectileSpread componentInParent = GetComponentInParent<ProjectileSpread>();
			if (componentInParent != null && componentInParent.parried)
			{
				flag = true;
			}
		}
		Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
		if (!flag)
		{
			nmov.GetHurt(Mathf.RoundToInt(damage), invincible: true);
		}
		Object.Destroy(base.gameObject);
	}
}
