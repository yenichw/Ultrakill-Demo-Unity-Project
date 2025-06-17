using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public bool enemy;

	public bool harmless;

	public bool lowQuality;

	private CameraController cc;

	private Light light;

	private MeshRenderer mr;

	private Color materialColor;

	private bool fading;

	public float speed;

	public float maxSize;

	public LayerMask lmask;

	public int damage;

	public float enemyDamageMultiplier;

	public GameObject explosionChunk;

	public bool ignite;

	public bool safeForPlayer;

	public bool friendlyFire;

	private List<Collider> hitColliders = new List<Collider>();

	public string hitterWeapon;

	public bool halved;

	private SphereCollider scol;

	private void Start()
	{
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		float num = Vector3.Distance(base.transform.position, cc.transform.position);
		if (num < 3f * maxSize)
		{
			cc.CameraShake(1.5f);
		}
		else if (num < 85f)
		{
			cc.CameraShake((1.5f - (num - 20f) / 65f * 1.5f) / 6f * maxSize);
		}
		scol = GetComponent<SphereCollider>();
		scol.enabled = true;
		if (speed == 0f)
		{
			speed = 1f;
		}
		if (!lowQuality && PlayerPrefs.GetInt("SimExp", 0) == 1)
		{
			lowQuality = true;
		}
		if (lowQuality)
		{
			return;
		}
		light = GetComponentInChildren<Light>();
		light.enabled = true;
		if (explosionChunk != null)
		{
			for (int i = 0; i < Random.Range(24, 30); i++)
			{
				GameObject obj = Object.Instantiate(explosionChunk, base.transform.position, Random.rotation);
				Vector3 vector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				obj.GetComponent<Rigidbody>().AddForce(vector * 1000f);
			}
		}
	}

	private void FixedUpdate()
	{
		base.transform.localScale += Vector3.one * 0.05f * speed;
		if (light != null)
		{
			light.range += 0.05f * speed;
		}
		if (!fading && base.transform.lossyScale.x * scol.radius > maxSize)
		{
			Fade();
		}
		if (!halved && base.transform.lossyScale.x * scol.radius > maxSize / 2f)
		{
			halved = true;
			damage = Mathf.RoundToInt((float)damage / 1.5f);
		}
		if (fading)
		{
			materialColor.a -= 0.02f;
			if (light != null)
			{
				light.intensity -= 0.65f;
			}
			mr.material.SetColor("_Color", materialColor);
			if (materialColor.a <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!harmless && !hitColliders.Contains(other))
		{
			if (!Physics.Raycast(base.transform.position + (other.transform.position - base.transform.position).normalized * 0.1f, other.transform.position - base.transform.position, out var hitInfo, Vector3.Distance(base.transform.position + (other.transform.position - base.transform.position).normalized * 0.1f, other.transform.position), lmask, QueryTriggerInteraction.Ignore) || hitInfo.transform.gameObject == other.gameObject || other.gameObject.layer == 11)
			{
				if (other.gameObject.layer == 11)
				{
					Collider[] componentsInChildren = other.transform.GetComponentsInChildren<Collider>();
					bool flag = false;
					Collider[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].gameObject == other.gameObject)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return;
					}
				}
				if (other.gameObject.tag == "Player")
				{
					hitColliders.Add(other);
					if (!safeForPlayer)
					{
						if (damage > 0)
						{
							if (enemy)
							{
								other.gameObject.GetComponent<NewMovement>().GetHurt(damage, invincible: true, 1f, explosion: true);
							}
							else
							{
								other.gameObject.GetComponent<NewMovement>().GetHurt(damage, invincible: true, 0f, explosion: true);
							}
						}
						if (Mathf.Abs(base.transform.position.x - other.transform.position.x) < 0.25f && Mathf.Abs(base.transform.position.z - other.transform.position.z) < 0.25f)
						{
							other.gameObject.GetComponent<NewMovement>().Launch(other.transform.position, 200f, maxSize);
						}
						else
						{
							other.gameObject.GetComponent<NewMovement>().Launch(base.transform.position, 200f, maxSize);
						}
					}
				}
				else if (other.gameObject.layer == 10 || other.gameObject.layer == 11)
				{
					EnemyIdentifierIdentifier componentInParent = other.GetComponentInParent<EnemyIdentifierIdentifier>();
					if (componentInParent != null && componentInParent.eid != null)
					{
						Collider component = componentInParent.eid.GetComponent<Collider>();
						if (component != null && !hitColliders.Contains(component) && !componentInParent.eid.dead)
						{
							hitColliders.Add(component);
							if (componentInParent.eid.type != EnemyType.Spider)
							{
								if (friendlyFire)
								{
									componentInParent.eid.hitter = "ffexplosion";
								}
								else if (enemy)
								{
									componentInParent.eid.hitter = "enemy";
								}
								else
								{
									componentInParent.eid.hitter = "explosion";
								}
								if (!componentInParent.eid.hitterWeapons.Contains(hitterWeapon))
								{
									componentInParent.eid.hitterWeapons.Add(hitterWeapon);
								}
								Vector3 vector = (other.transform.position - base.transform.position).normalized;
								if (vector.y <= 0.5f)
								{
									vector = new Vector3(vector.x, vector.y + 0.5f, vector.z);
								}
								else if (vector.y < 1f)
								{
									vector = new Vector3(vector.x, 1f, vector.z);
								}
								if (damage == 0 && componentInParent.eid.type == EnemyType.Drone)
								{
									vector = Vector3.zero;
								}
								componentInParent.eid.DeliverDamage(other.gameObject, vector * 50000f, other.transform.position, (float)damage / 10f, tryForExplode: false, 0f);
								if (ignite)
								{
									Flammable componentInChildren = componentInParent.eid.GetComponentInChildren<Flammable>();
									if (componentInChildren != null)
									{
										componentInChildren.Burn(damage / 10);
									}
								}
							}
						}
						else if (componentInParent.eid.dead)
						{
							hitColliders.Add(other);
							if (enemy)
							{
								componentInParent.eid.hitter = "enemy";
							}
							else
							{
								componentInParent.eid.hitter = "explosion";
							}
							componentInParent.eid.DeliverDamage(other.gameObject, (other.transform.position - base.transform.position).normalized * 5000f, other.transform.position, (float)damage / 10f * enemyDamageMultiplier, tryForExplode: false, 0f);
							if (ignite)
							{
								Flammable componentInChildren2 = componentInParent.eid.GetComponentInChildren<Flammable>();
								if (componentInChildren2 != null)
								{
									componentInChildren2.Burn(damage / 10);
								}
							}
						}
					}
				}
				else if (other.GetComponent<Breakable>() != null)
				{
					other.GetComponent<Breakable>().Break();
				}
				else if (other.GetComponent<Glass>() != null)
				{
					other.GetComponent<Glass>().Shatter();
				}
				else if (ignite && other.GetComponent<Flammable>() != null)
				{
					other.GetComponent<Flammable>().Burn(4f);
				}
			}
			else if (hitInfo.collider == other && other.GetComponent<Breakable>() != null)
			{
				other.GetComponent<Breakable>().Break();
			}
			else if (other.GetComponent<Glass>() != null)
			{
				other.GetComponent<Glass>().Shatter();
			}
		}
		if (harmless)
		{
			return;
		}
		if (other.gameObject.tag != "Player" && other.GetComponent<Rigidbody>() != null)
		{
			if (!hitColliders.Contains(other))
			{
				hitColliders.Add(other);
			}
			Vector3 normalized = (other.transform.position - base.transform.position).normalized;
			normalized = new Vector3(normalized.x * (5f - Vector3.Distance(other.transform.position, base.transform.position)) * 7500f, 18750f, normalized.z * (5f - Vector3.Distance(other.transform.position, base.transform.position)) * 7500f);
			other.GetComponent<Rigidbody>().AddForce(normalized);
		}
		if (other.gameObject.layer == 14)
		{
			ThrownSword component2 = other.GetComponent<ThrownSword>();
			Projectile component3 = other.GetComponent<Projectile>();
			if (component2 != null)
			{
				component2.deflected = true;
			}
			if (component3 != null)
			{
				other.transform.LookAt(2f * other.transform.position - base.transform.position);
				component3.friendly = true;
			}
		}
	}

	private void Fade()
	{
		harmless = true;
		mr = GetComponent<MeshRenderer>();
		materialColor = mr.material.GetColor("_Color");
		fading = true;
		speed /= 4f;
	}

	private void BecomeHarmless()
	{
		harmless = true;
	}
}
