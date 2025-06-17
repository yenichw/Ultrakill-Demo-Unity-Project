using System;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBeam : MonoBehaviour
{
	public class RaycastResult : IComparable<RaycastResult>
	{
		public float distance;

		public Transform transform;

		public RaycastHit rrhit;

		public RaycastResult(RaycastHit hit)
		{
			distance = hit.distance;
			transform = hit.transform;
			rrhit = hit;
		}

		public int CompareTo(RaycastResult other)
		{
			return distance.CompareTo(other.distance);
		}
	}

	public BeamType beamType;

	private LineRenderer lr;

	private AudioSource aud;

	private Light muzzleLight;

	public Vector3 alternateStartPoint;

	[HideInInspector]
	public int bodiesPierced;

	private int enemiesPierced;

	private RaycastHit[] allHits;

	[HideInInspector]
	public List<RaycastResult> hitList = new List<RaycastResult>();

	private GunControl gc;

	private RaycastHit hit;

	private Vector3 shotHitPoint;

	private CameraController cc;

	public GameObject hitParticle;

	public int bulletForce;

	public bool quickDraw;

	public int gunVariation;

	public float damage;

	public int hitAmount;

	public int maxHitsPerTarget;

	private int currentHits;

	public bool noMuzzleflash;

	private bool fadeOut;

	private bool didntHit;

	private LayerMask ignoreEnemyTrigger;

	private LayerMask enemyLayerMask;

	private LayerMask pierceLayerMask;

	public GameObject ricochetSound;

	public GameObject enemyHitSound;

	public bool fake;

	[HideInInspector]
	public List<EnemyIdentifier> hitEids = new List<EnemyIdentifier>();

	private void Start()
	{
		muzzleLight = GetComponent<Light>();
		lr = GetComponent<LineRenderer>();
		cc = UnityEngine.Object.FindObjectOfType<CameraController>();
		enemyLayerMask = (int)enemyLayerMask | 0x400;
		enemyLayerMask = (int)enemyLayerMask | 0x800;
		pierceLayerMask = (int)pierceLayerMask | 0x100;
		pierceLayerMask = (int)pierceLayerMask | 0x1000000;
		pierceLayerMask = (int)pierceLayerMask | 0x4000000;
		ignoreEnemyTrigger = (int)enemyLayerMask | (int)pierceLayerMask;
		if (!fake)
		{
			Shoot();
		}
		else
		{
			fadeOut = true;
		}
		if (maxHitsPerTarget == 0)
		{
			maxHitsPerTarget = 99;
		}
	}

	private void Update()
	{
		if (fadeOut)
		{
			lr.widthMultiplier -= Time.deltaTime * 1.5f;
			if (muzzleLight != null)
			{
				muzzleLight.intensity -= Time.deltaTime * 100f;
			}
			if (lr.widthMultiplier <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public void FakeShoot(Vector3 target)
	{
		Vector3 position = base.transform.position;
		if (alternateStartPoint != Vector3.zero)
		{
			position = alternateStartPoint;
		}
		lr.SetPosition(0, position);
		lr.SetPosition(1, target);
		Transform child = base.transform.GetChild(0);
		if (!noMuzzleflash)
		{
			child.position = position;
			child.rotation = base.transform.rotation;
		}
		else
		{
			child.gameObject.SetActive(value: false);
		}
	}

	private void Shoot()
	{
		if (hitAmount == 1)
		{
			fadeOut = true;
			if (beamType == BeamType.Railgun)
			{
				cc.CameraShake(2f);
			}
			if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, float.PositiveInfinity, ignoreEnemyTrigger))
			{
				bool flag = false;
				if (hit.transform.gameObject.layer == 8)
				{
					if (Physics.SphereCast(base.transform.position, 0.4f, base.transform.forward, out var hitInfo, Vector3.Distance(base.transform.position, hit.point), enemyLayerMask))
					{
						ExecuteHits(hitInfo);
					}
					else
					{
						ExecuteHits(hit);
					}
				}
				else if (beamType == BeamType.Railgun && hit.transform.gameObject.tag == "Coin")
				{
					flag = true;
					lr.SetPosition(1, hit.transform.position);
					GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject, hit.point, base.transform.rotation);
					gameObject.SetActive(value: false);
					RevolverBeam component = gameObject.GetComponent<RevolverBeam>();
					component.bodiesPierced = 0;
					component.noMuzzleflash = true;
					component.alternateStartPoint = Vector3.zero;
					Coin component2 = hit.transform.gameObject.GetComponent<Coin>();
					if (component2 != null)
					{
						component2.DelayedReflectRevolver(hit.point, gameObject);
					}
					fadeOut = true;
				}
				else
				{
					ExecuteHits(hit);
				}
				shotHitPoint = hit.point;
				if (hit.transform.gameObject.tag != "Armor" && !flag)
				{
					UnityEngine.Object.Instantiate(hitParticle, shotHitPoint, base.transform.rotation).transform.forward = hit.normal;
				}
			}
			else if (Physics.SphereCast(base.transform.position, 0.4f, base.transform.forward, out hit, Vector3.Distance(base.transform.position, hit.point), enemyLayerMask))
			{
				ExecuteHits(hit);
				shotHitPoint = hit.point;
				if (hit.transform.gameObject.tag != "Armor")
				{
					UnityEngine.Object.Instantiate(hitParticle, shotHitPoint, base.transform.rotation).transform.forward = hit.normal;
				}
			}
			else
			{
				shotHitPoint = base.transform.position + base.transform.forward * 1000f;
			}
		}
		else
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, float.PositiveInfinity, pierceLayerMask))
			{
				shotHitPoint = hit.point;
				if (hit.transform.gameObject.tag != "Armor")
				{
					UnityEngine.Object.Instantiate(hitParticle, shotHitPoint, base.transform.rotation).transform.forward = hit.normal;
				}
			}
			else
			{
				shotHitPoint = base.transform.position + base.transform.forward * 999f;
				didntHit = true;
			}
			float radius = 0.6f;
			if (beamType == BeamType.Railgun)
			{
				radius = 1.2f;
			}
			allHits = Physics.SphereCastAll(base.transform.position, radius, base.transform.forward, Vector3.Distance(base.transform.position, shotHitPoint), enemyLayerMask);
		}
		Vector3 position = base.transform.position;
		if (alternateStartPoint != Vector3.zero)
		{
			position = alternateStartPoint;
		}
		lr.SetPosition(0, position);
		lr.SetPosition(1, shotHitPoint);
		if (hitAmount != 1)
		{
			PiercingShotOrder();
		}
		Transform child = base.transform.GetChild(0);
		if (!noMuzzleflash)
		{
			child.position = position;
			child.rotation = base.transform.rotation;
		}
		else
		{
			child.gameObject.SetActive(value: false);
		}
	}

	private void PiercingShotOrder()
	{
		hitList.Clear();
		RaycastHit[] array = allHits;
		foreach (RaycastHit raycastHit in array)
		{
			hitList.Add(new RaycastResult(raycastHit));
		}
		bool flag = false;
		if (!didntHit && (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 24) && hit.transform.GetComponent<Breakable>() != null)
		{
			flag = true;
		}
		if (!didntHit && (flag || hit.transform.gameObject.tag == "Glass" || hit.transform.gameObject.tag == "GlassFloor" || hit.transform.gameObject.tag == "Armor"))
		{
			hitList.Add(new RaycastResult(hit));
		}
		hitList.Sort();
		PiercingShotCheck();
	}

	private void PiercingShotCheck()
	{
		if (enemiesPierced < hitList.Count)
		{
			if (hitList[enemiesPierced].transform == null)
			{
				enemiesPierced++;
				PiercingShotCheck();
				return;
			}
			if (hitList[enemiesPierced].transform.gameObject.tag == "Armor")
			{
				lr.SetPosition(1, hitList[enemiesPierced].rrhit.point);
				GameObject obj = UnityEngine.Object.Instantiate(base.gameObject, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
				obj.transform.forward = Vector3.Reflect(base.transform.forward, hitList[enemiesPierced].rrhit.normal);
				RevolverBeam component = obj.GetComponent<RevolverBeam>();
				component.noMuzzleflash = true;
				component.alternateStartPoint = Vector3.zero;
				component.bodiesPierced = bodiesPierced;
				UnityEngine.Object.Instantiate(ricochetSound, hitList[enemiesPierced].rrhit.point, Quaternion.identity);
				fadeOut = true;
				enemiesPierced = hitList.Count;
				return;
			}
			if (hitList[enemiesPierced].transform.gameObject.tag == "Coin")
			{
				lr.SetPosition(1, hitList[enemiesPierced].transform.position);
				GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
				gameObject.SetActive(value: false);
				RevolverBeam component2 = gameObject.GetComponent<RevolverBeam>();
				component2.bodiesPierced = 0;
				component2.noMuzzleflash = true;
				component2.alternateStartPoint = Vector3.zero;
				Coin component3 = hitList[enemiesPierced].transform.gameObject.GetComponent<Coin>();
				if (component3 != null)
				{
					component3.DelayedReflectRevolver(hitList[enemiesPierced].rrhit.point, gameObject);
				}
				fadeOut = true;
				return;
			}
			if ((hitList[enemiesPierced].transform.gameObject.layer == 10 || hitList[enemiesPierced].transform.gameObject.layer == 11) && hitList[enemiesPierced].transform.gameObject.tag != "Breakable" && hitList[enemiesPierced].transform.gameObject.tag != "Armor" && bodiesPierced < hitAmount)
			{
				EnemyIdentifierIdentifier componentInParent = hitList[enemiesPierced].transform.gameObject.GetComponentInParent<EnemyIdentifierIdentifier>();
				if (!componentInParent)
				{
					enemiesPierced++;
					currentHits = 0;
					PiercingShotCheck();
					return;
				}
				EnemyIdentifier eid = componentInParent.eid;
				if (eid != null)
				{
					if (!hitEids.Contains(eid) || (beamType == BeamType.Revolver && enemiesPierced == hitList.Count - 1))
					{
						bool flag = false;
						if (eid.dead)
						{
							flag = true;
						}
						ExecuteHits(hitList[enemiesPierced].rrhit);
						if (!flag || hitList[enemiesPierced].transform.gameObject.layer == 11 || (beamType == BeamType.Revolver && enemiesPierced == hitList.Count - 1))
						{
							currentHits++;
							bodiesPierced++;
							UnityEngine.Object.Instantiate(hitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
							cc.HitStop(0.05f);
						}
						else
						{
							if (beamType == BeamType.Revolver)
							{
								hitEids.Add(eid);
							}
							enemiesPierced++;
							currentHits = 0;
						}
						if (currentHits >= maxHitsPerTarget)
						{
							hitEids.Add(eid);
							currentHits = 0;
							enemiesPierced++;
						}
						if (beamType == BeamType.Revolver && !flag)
						{
							Invoke("PiercingShotCheck", 0.05f);
						}
						else if (beamType == BeamType.Revolver)
						{
							PiercingShotCheck();
						}
						else if (!flag)
						{
							Invoke("PiercingShotCheck", 0.025f);
						}
						else
						{
							Invoke("PiercingShotCheck", 0.01f);
						}
					}
					else
					{
						enemiesPierced++;
						currentHits = 0;
						PiercingShotCheck();
					}
				}
				else
				{
					ExecuteHits(hitList[enemiesPierced].rrhit);
					enemiesPierced++;
					PiercingShotCheck();
				}
				return;
			}
			if (hitList[enemiesPierced].transform.gameObject.tag == "Glass" || hitList[enemiesPierced].transform.gameObject.tag == "GlassFloor")
			{
				Glass component4 = hitList[enemiesPierced].transform.gameObject.GetComponent<Glass>();
				if (!component4.broken)
				{
					component4.Shatter();
				}
				enemiesPierced++;
				PiercingShotCheck();
				return;
			}
			Breakable component5 = hitList[enemiesPierced].transform.GetComponent<Breakable>();
			if (component5 != null && (beamType == BeamType.Railgun || component5.weak))
			{
				if (component5.interrupt)
				{
					cc.GetComponentInChildren<StyleHUD>().AddPoints(100, "<color=lime>INTERRUPTION</color>");
					cc.GetComponentInChildren<Punch>().ParryFlash();
				}
				component5.Break();
			}
			enemiesPierced++;
			PiercingShotCheck();
		}
		else
		{
			enemiesPierced = 0;
			fadeOut = true;
		}
	}

	public void ExecuteHits(RaycastHit currentHit)
	{
		if (!(currentHit.transform != null))
		{
			return;
		}
		if (gc == null)
		{
			gc = UnityEngine.Object.FindObjectOfType<GunControl>();
		}
		if (currentHit.transform.gameObject.tag == "Enemy" || currentHit.transform.gameObject.tag == "Body" || currentHit.transform.gameObject.tag == "Limb" || currentHit.transform.gameObject.tag == "EndLimb" || currentHit.transform.gameObject.tag == "Head")
		{
			if (hitAmount > 1)
			{
				cc.CameraShake(1f);
			}
			else
			{
				cc.CameraShake(0.5f);
			}
			EnemyIdentifier eid = currentHit.transform.GetComponentInParent<EnemyIdentifierIdentifier>().eid;
			if (eid != null)
			{
				if (!eid.dead && quickDraw)
				{
					cc.GetComponentInChildren<StyleHUD>().AddPoints(50, "<color=cyan>QUICKDRAW</color>");
				}
				eid.rhit = currentHit;
				string text = "";
				if (beamType == BeamType.Revolver)
				{
					text = "revolver";
				}
				else if (beamType == BeamType.Railgun)
				{
					text = "railcannon";
				}
				eid.hitter = text;
				if (!eid.hitterWeapons.Contains(text + gunVariation))
				{
					eid.hitterWeapons.Add(text + gunVariation);
				}
				if (!eid.dead && currentHit.transform.gameObject.tag == "Head" && beamType != BeamType.Railgun)
				{
					gc.headshots++;
					gc.headShotComboTime = 3f;
				}
				else if (currentHit.transform.gameObject.tag != "Head" || beamType == BeamType.Railgun)
				{
					gc.headshots = 0;
					gc.headShotComboTime = 0f;
				}
				int num = 1;
				if (beamType == BeamType.Railgun)
				{
					num = 0;
				}
				float multiplier = damage;
				eid.DeliverDamage(currentHit.transform.gameObject, (currentHit.transform.position - base.transform.position).normalized * bulletForce, currentHit.point, multiplier, tryForExplode: false, num);
				if (gc.headshots > 1)
				{
					cc.GetComponentInChildren<StyleHUD>().AddPoints(gc.headshots * 20, "<color=cyan>HEADSHOT COMBO x" + gc.headshots + "</color>");
				}
			}
			UnityEngine.Object.Instantiate(enemyHitSound, currentHit.point, Quaternion.identity);
		}
		else
		{
			gc.headshots = 0;
			gc.headShotComboTime = 0f;
			if (currentHit.transform.gameObject.tag == "Armor")
			{
				GameObject obj = UnityEngine.Object.Instantiate(base.gameObject, currentHit.point, base.transform.rotation);
				obj.transform.forward = Vector3.Reflect(base.transform.forward, currentHit.normal);
				RevolverBeam component = obj.GetComponent<RevolverBeam>();
				component.noMuzzleflash = true;
				component.alternateStartPoint = Vector3.zero;
				UnityEngine.Object.Instantiate(ricochetSound, currentHit.point, Quaternion.identity);
			}
		}
		Breakable component2 = currentHit.transform.GetComponent<Breakable>();
		if (component2 != null && (hitAmount > 1 || component2.weak))
		{
			if (component2.interrupt)
			{
				cc.GetComponentInChildren<StyleHUD>().AddPoints(100, "<color=lime>INTERRUPTION</color>");
				cc.GetComponentInChildren<Punch>().ParryFlash();
			}
			component2.Break();
		}
		Coin component3 = currentHit.transform.GetComponent<Coin>();
		if (component3 != null && beamType == BeamType.Revolver)
		{
			if (quickDraw)
			{
				component3.quickDraw = true;
			}
			component3.DelayedReflectRevolver(currentHit.point);
		}
	}
}
