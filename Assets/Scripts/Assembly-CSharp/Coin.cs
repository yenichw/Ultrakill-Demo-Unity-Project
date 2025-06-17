using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	private Rigidbody rb;

	private bool checkingSpeed;

	private float timeToDelete = 1f;

	public LayerMask lmask;

	public GameObject refBeam;

	public Vector3 hitPoint = Vector3.zero;

	private Collider[] cols;

	public bool shot;

	public GameObject coinBreak;

	public float power;

	private EnemyIdentifier eid;

	public bool quickDraw;

	public Material uselessMaterial;

	private GameObject altBeam;

	public GameObject coinHitSound;

	private void Start()
	{
		Invoke("GetDeleted", 5f);
		Invoke("StartCheckingSpeed", 0.1f);
		rb = GetComponent<Rigidbody>();
		cols = GetComponents<Collider>();
		Collider[] array = cols;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	private void Update()
	{
		if (!shot)
		{
			if (checkingSpeed && rb.velocity.magnitude < 1f)
			{
				timeToDelete -= Time.deltaTime * 10f;
			}
			else
			{
				timeToDelete = 1f;
			}
			if (timeToDelete <= 0f)
			{
				GetDeleted();
			}
		}
	}

	public void DelayedReflectRevolver(Vector3 hitp, GameObject beam = null)
	{
		if (checkingSpeed)
		{
			rb.isKinematic = true;
			shot = true;
			hitPoint = hitp;
			altBeam = beam;
			Invoke("ReflectRevolver", 0.1f);
		}
	}

	public void ReflectRevolver()
	{
		Coin[] array = Object.FindObjectsOfType<Coin>();
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		GetComponent<SphereCollider>().enabled = false;
		bool flag = false;
		bool flag2 = false;
		if (array.Length > 1)
		{
			Coin[] array2 = array;
			foreach (Coin coin in array2)
			{
				if (coin != this && !coin.shot)
				{
					float sqrMagnitude = (coin.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num && !Physics.Raycast(base.transform.position, coin.transform.position - base.transform.position, out var _, Vector3.Distance(base.transform.position, coin.transform.position) - 0.5f, lmask))
					{
						gameObject = coin.gameObject;
						num = sqrMagnitude;
					}
				}
			}
			if (gameObject != null)
			{
				flag = true;
				Coin component = gameObject.GetComponent<Coin>();
				component.power = power + 1f;
				if (quickDraw)
				{
					component.quickDraw = true;
				}
				AudioSource[] array3 = null;
				if (altBeam == null)
				{
					component.DelayedReflectRevolver(gameObject.transform.position);
					LineRenderer component2 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
					array3 = component2.GetComponents<AudioSource>();
					if (hitPoint == Vector3.zero)
					{
						component2.SetPosition(0, base.transform.position);
					}
					else
					{
						component2.SetPosition(0, hitPoint);
					}
					component2.SetPosition(1, gameObject.transform.position);
					if (power > 2f)
					{
						AudioSource[] array4 = array3;
						foreach (AudioSource obj in array4)
						{
							obj.pitch = 1f + (power - 2f) / 5f;
							obj.Play();
						}
					}
				}
			}
		}
		if (!flag)
		{
			GameObject[] array5 = GameObject.FindGameObjectsWithTag("Enemy");
			gameObject = null;
			num = float.PositiveInfinity;
			position = base.transform.position;
			GameObject[] array6 = array5;
			foreach (GameObject gameObject2 in array6)
			{
				float sqrMagnitude2 = (gameObject2.transform.position - position).sqrMagnitude;
				if (!(sqrMagnitude2 < num))
				{
					continue;
				}
				eid = gameObject2.GetComponent<EnemyIdentifier>();
				if (eid != null && !eid.dead)
				{
					Transform transform = ((!(eid.weakPoint != null)) ? eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform : eid.weakPoint.transform);
					if (!Physics.Raycast(base.transform.position, transform.position - base.transform.position, out var _, Vector3.Distance(base.transform.position, transform.position) - 0.5f, lmask))
					{
						gameObject = gameObject2;
						num = sqrMagnitude2;
					}
					else
					{
						eid = null;
					}
				}
				else
				{
					eid = null;
				}
			}
			if (gameObject != null)
			{
				if (eid == null)
				{
					eid = gameObject.GetComponent<EnemyIdentifier>();
				}
				flag2 = true;
				if (altBeam == null)
				{
					StyleHUD componentInChildren = GameObject.FindWithTag("MainCamera").GetComponentInChildren<StyleHUD>();
					LineRenderer component3 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
					AudioSource[] components = component3.GetComponents<AudioSource>();
					if (hitPoint == Vector3.zero)
					{
						component3.SetPosition(0, base.transform.position);
					}
					else
					{
						component3.SetPosition(0, hitPoint);
					}
					Vector3 zero = Vector3.zero;
					zero = ((!(eid.weakPoint != null)) ? eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position : eid.weakPoint.transform.position);
					component3.SetPosition(1, zero);
					if (eid.weakPoint != null && eid.weakPoint.GetComponent<EnemyIdentifierIdentifier>() != null)
					{
						componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
						if (quickDraw)
						{
							componentInChildren.AddPoints(50, "<color=cyan>QUICKDRAW</color>");
						}
						eid.hitter = "revolver";
						if (!eid.hitterWeapons.Contains("revolver1"))
						{
							eid.hitterWeapons.Add("revolver1");
						}
						eid.DeliverDamage(eid.weakPoint, (base.transform.position - eid.weakPoint.transform.position).normalized * 10000f, zero, power, tryForExplode: false, 1f);
					}
					else if (eid.weakPoint != null)
					{
						Breakable componentInChildren2 = eid.weakPoint.GetComponentInChildren<Breakable>();
						componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
						if (componentInChildren2.precisionOnly)
						{
							componentInChildren.AddPoints(100, "<color=lime>INTERRUPTION</color>");
							GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().ParryFlash();
						}
						componentInChildren2.Break();
					}
					else
					{
						componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
						eid.hitter = "revolver";
						eid.DeliverDamage(eid.GetComponentInChildren<EnemyIdentifierIdentifier>().gameObject, (eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position - base.transform.position).normalized * 10000f, hitPoint, power, tryForExplode: false, 1f);
					}
					if (power > 2f)
					{
						AudioSource[] array4 = components;
						foreach (AudioSource obj2 in array4)
						{
							obj2.pitch = 1f + (power - 2f) / 5f;
							obj2.Play();
						}
					}
					eid = null;
				}
			}
			else
			{
				gameObject = null;
				List<GameObject> list = new List<GameObject>();
				array6 = GameObject.FindGameObjectsWithTag("Glass");
				foreach (GameObject item in array6)
				{
					list.Add(item);
				}
				array6 = GameObject.FindGameObjectsWithTag("GlassFloor");
				foreach (GameObject item2 in array6)
				{
					list.Add(item2);
				}
				if (list.Count > 0)
				{
					gameObject = null;
					num = float.PositiveInfinity;
					position = base.transform.position;
					foreach (GameObject item3 in list)
					{
						float sqrMagnitude3 = (item3.transform.position - position).sqrMagnitude;
						if (!(sqrMagnitude3 < num))
						{
							continue;
						}
						Glass componentInChildren3 = item3.GetComponentInChildren<Glass>();
						if (componentInChildren3 != null && !componentInChildren3.broken)
						{
							Transform transform2 = item3.transform;
							if (!Physics.Raycast(base.transform.position, transform2.position - base.transform.position, out var hitInfo3, Vector3.Distance(base.transform.position, transform2.position) - 0.5f, lmask) || hitInfo3.transform.gameObject.tag == "Glass" || hitInfo3.transform.gameObject.tag == "GlassFloor")
							{
								gameObject = item3;
								num = sqrMagnitude3;
							}
						}
					}
					if (gameObject != null && altBeam == null)
					{
						gameObject.GetComponentInChildren<Glass>().Shatter();
						LineRenderer component4 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
						if (power > 2f)
						{
							AudioSource[] array4 = component4.GetComponents<AudioSource>();
							foreach (AudioSource obj3 in array4)
							{
								obj3.pitch = 1f + (power - 2f) / 5f;
								obj3.Play();
							}
						}
						if (hitPoint == Vector3.zero)
						{
							component4.SetPosition(0, base.transform.position);
						}
						else
						{
							component4.SetPosition(0, hitPoint);
						}
						component4.SetPosition(1, gameObject.transform.position);
					}
				}
				if ((list.Count == 0 || gameObject == null) && altBeam == null)
				{
					Vector3 normalized = Random.insideUnitSphere.normalized;
					LineRenderer component5 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
					if (power > 2f)
					{
						AudioSource[] array4 = component5.GetComponents<AudioSource>();
						foreach (AudioSource obj4 in array4)
						{
							obj4.pitch = 1f + (power - 2f) / 5f;
							obj4.Play();
						}
					}
					if (hitPoint == Vector3.zero)
					{
						component5.SetPosition(0, base.transform.position);
					}
					else
					{
						component5.SetPosition(0, hitPoint);
					}
					if (Physics.Raycast(base.transform.position, normalized, out var hitInfo4, float.PositiveInfinity, lmask))
					{
						component5.SetPosition(1, hitInfo4.point);
					}
					else
					{
						component5.SetPosition(1, base.transform.position + normalized * 1000f);
					}
				}
			}
		}
		if (altBeam != null)
		{
			AudioSource[] components2 = Object.Instantiate(coinHitSound, base.transform.position, Quaternion.identity).GetComponents<AudioSource>();
			RevolverBeam component6 = altBeam.GetComponent<RevolverBeam>();
			altBeam.transform.position = base.transform.position;
			if (flag2)
			{
				if (eid.weakPoint != null)
				{
					altBeam.transform.LookAt(eid.weakPoint.transform.position);
				}
				else
				{
					altBeam.transform.LookAt(eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position);
				}
				StyleHUD componentInChildren4 = GameObject.FindWithTag("MainCamera").GetComponentInChildren<StyleHUD>();
				componentInChildren4.AddPoints(50, "<color=cyan>RICOSHOT</color>");
				if (quickDraw)
				{
					componentInChildren4.AddPoints(50, "<color=cyan>QUICKDRAW</color>");
				}
				if (component6.beamType == BeamType.Revolver)
				{
					eid.hitter = "revolver";
					if (component6.hitAmount > 1)
					{
						if (!eid.hitterWeapons.Contains("revolver0"))
						{
							eid.hitterWeapons.Add("revolver0");
						}
					}
					else if (!eid.hitterWeapons.Contains("revolver1"))
					{
						eid.hitterWeapons.Add("revolver1");
					}
				}
				else
				{
					eid.hitter = "railcannon";
					if (!eid.hitterWeapons.Contains("railcannon0"))
					{
						eid.hitterWeapons.Add("railcannon0");
					}
				}
			}
			else if (gameObject != null)
			{
				altBeam.transform.LookAt(gameObject.transform.position);
			}
			else
			{
				altBeam.transform.forward = Random.insideUnitSphere.normalized;
			}
			if (!flag)
			{
				component6.damage += power / 2f;
			}
			if (power > 2f)
			{
				AudioSource[] array4 = components2;
				foreach (AudioSource obj5 in array4)
				{
					obj5.pitch = 1f + (power - 2f) / 5f;
					obj5.Play();
				}
			}
			altBeam.SetActive(value: true);
		}
		base.gameObject.SetActive(value: false);
		new GameObject().AddComponent<CoinCollector>().coin = base.gameObject;
		CancelInvoke("GetDeleted");
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 8 || collision.gameObject.layer == 24)
		{
			GoreZone componentInParent = collision.transform.GetComponentInParent<GoreZone>();
			if (componentInParent != null)
			{
				base.transform.SetParent(componentInParent.goreZone, worldPositionStays: true);
			}
			GetDeleted();
		}
	}

	public void GetDeleted()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Object.Instantiate(coinBreak, base.transform.position, Quaternion.identity);
		}
		GetComponent<MeshRenderer>().material = uselessMaterial;
		Object.Destroy(GetComponent<AudioSource>());
		Object.Destroy(base.transform.GetChild(0).GetComponent<AudioSource>());
		Object.Destroy(GetComponent<TrailRenderer>());
		Object.Destroy(GetComponent<SphereCollider>());
		base.gameObject.AddComponent<RemoveOnTime>().time = 5f;
		Object.Destroy(this);
	}

	private void StartCheckingSpeed()
	{
		Collider[] array = cols;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		checkingSpeed = true;
	}
}
