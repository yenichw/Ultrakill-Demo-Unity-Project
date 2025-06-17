using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
	public EnemyType type;

	[HideInInspector]
	public Zombie zombie;

	[HideInInspector]
	public SpiderBody spider;

	[HideInInspector]
	public Machine machine;

	[HideInInspector]
	public Statue statue;

	[HideInInspector]
	public Wicked wicked;

	[HideInInspector]
	public Drone drone;

	public RaycastHit rhit;

	public string hitter;

	public List<string> hitterWeapons = new List<string>();

	public string[] weaknesses;

	public float weaknessMultiplier;

	public GameObject weakPoint;

	public bool exploded;

	public bool dead;

	public DoorController usingDoor;

	public bool ignoredByEnemies;

	public GameObject ineffectiveSound;

	private EnemyIdentifierIdentifier[] limbs;

	public GameObject splatterBlood;

	public int nailsAmount;

	public List<Nail> nails = new List<Nail>();

	public bool useBrakes;

	public bool bigEnemy;

	private bool beingZapped;

	public bool magneted;

	public GameObject[] activateOnDeath;

	public GameObject[] deactivateOnDeath;

	private void Start()
	{
		if (type == EnemyType.Zombie)
		{
			zombie = GetComponent<Zombie>();
		}
		else if (type == EnemyType.Spider)
		{
			spider = GetComponent<SpiderBody>();
		}
		else if (type == EnemyType.Machine)
		{
			machine = GetComponent<Machine>();
		}
		else if (type == EnemyType.Drone)
		{
			drone = GetComponent<Drone>();
		}
	}

	public void DeliverDamage(GameObject target, Vector3 force, Vector3 hitPoint, float multiplier, bool tryForExplode, float critMultiplier)
	{
		if (weaknesses.Length != 0)
		{
			string[] array = weaknesses;
			foreach (string text in array)
			{
				if (hitter == text)
				{
					multiplier *= weaknessMultiplier;
				}
			}
		}
		if (nails.Count > 10)
		{
			for (int j = 0; j < nails.Count - 10; j++)
			{
				if (nails[j] != null)
				{
					Object.Destroy(nails[j].gameObject);
				}
				nails.RemoveAt(j);
			}
		}
		if (!beingZapped && hitter == "railcannon" && nailsAmount > 0)
		{
			beingZapped = true;
			foreach (Nail nail in nails)
			{
				nail.Zap();
			}
			Invoke("AfterShock", 1f);
		}
		if (beingZapped)
		{
			Debug.Log("Damage: " + multiplier);
		}
		if (magneted && hitter != "deathzone")
		{
			magneted = false;
		}
		switch (type)
		{
		case EnemyType.Zombie:
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GetHurt(target, force, multiplier, critMultiplier);
			if (tryForExplode && zombie.health <= 0f && !exploded)
			{
				Explode();
			}
			if (zombie.health <= 0f)
			{
				Death();
			}
			break;
		case EnemyType.Spider:
			if (spider == null)
			{
				spider = GetComponent<SpiderBody>();
			}
			if (hitter != "explosion" && hitter != "ffexplosion")
			{
				spider.GetHurt(target, force, hitPoint, multiplier);
			}
			if (spider.health <= 0f)
			{
				Death();
			}
			break;
		case EnemyType.Machine:
			if (machine == null)
			{
				machine = GetComponent<Machine>();
			}
			machine.GetHurt(target, force, multiplier, critMultiplier);
			if (tryForExplode && machine.health <= 0f && (machine.symbiote == null || machine.symbiote.health <= 0f) && !machine.dontDie && !exploded)
			{
				Explode();
			}
			if (machine.health <= 0f)
			{
				Death();
			}
			break;
		case EnemyType.Statue:
			if (statue == null)
			{
				statue = GetComponent<Statue>();
			}
			statue.GetHurt(target, force, multiplier, critMultiplier, hitPoint);
			if (tryForExplode && statue.health <= 0f && !exploded)
			{
				Explode();
			}
			if (statue.health <= 0f)
			{
				Death();
			}
			break;
		case EnemyType.Wicked:
			if (wicked == null)
			{
				wicked = GetComponent<Wicked>();
			}
			wicked.GetHit();
			break;
		case EnemyType.Drone:
			if (drone == null)
			{
				drone = GetComponent<Drone>();
			}
			drone.GetHurt(force, multiplier);
			break;
		}
	}

	private void AfterShock()
	{
		float num = nailsAmount / 15;
		if (num > 6f)
		{
			num = 6f;
		}
		GoreZone componentInParent = GetComponentInParent<GoreZone>();
		foreach (Nail nail in nails)
		{
			GameObject gore = Object.FindObjectOfType<BloodsplatterManager>().GetGore(GoreType.Small);
			gore.transform.position = nail.transform.position;
			gore.SetActive(value: true);
			Bloodsplatter component = gore.GetComponent<Bloodsplatter>();
			gore.transform.SetParent(componentInParent.goreZone, worldPositionStays: true);
			if (component != null && !dead)
			{
				component.GetReady();
			}
			Object.Destroy(nail.gameObject);
		}
		nails.Clear();
		nailsAmount = 0;
		hitter = "electricity";
		EnemyIdentifierIdentifier[] componentsInChildren = GetComponentsInChildren<EnemyIdentifierIdentifier>();
		if (!dead)
		{
			Object.FindObjectOfType<StyleHUD>().AddPoints(Mathf.RoundToInt(num * 15f), "<color=cyan>CONDUCTOR</color>");
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject != base.gameObject)
			{
				DeliverDamage(componentsInChildren[i].gameObject, Vector3.zero, base.transform.position, num, tryForExplode: true, 0f);
				break;
			}
		}
		beingZapped = false;
		Object.FindObjectOfType<CameraController>().CameraShake(1f);
	}

	private void Death()
	{
		if (!dead)
		{
			if (hitterWeapons.Count > 1)
			{
				Object.FindObjectOfType<StyleHUD>().AddPoints(50, "<color=cyan>ARSENAL</color>");
			}
			GameObject[] array = activateOnDeath;
			foreach (GameObject gameObject in array)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(value: true);
				}
			}
			array = deactivateOnDeath;
			foreach (GameObject gameObject2 in array)
			{
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(value: false);
				}
			}
		}
		dead = true;
		Magnet[] componentsInChildren = GetComponentsInChildren<Magnet>();
		if (componentsInChildren != null)
		{
			Magnet[] array2 = componentsInChildren;
			for (int i = 0; i < array2.Length; i++)
			{
				Object.Destroy(array2[i].gameObject);
			}
		}
		if (usingDoor != null)
		{
			usingDoor.Close();
			usingDoor = null;
		}
	}

	public void InstaKill()
	{
		Debug.Log("Instakill Phase 2");
		if (dead)
		{
			return;
		}
		if (magneted)
		{
			Object.FindObjectOfType<StyleHUD>().AddPoints(240, "<color=cyan>CATAPULTED</color>");
		}
		dead = true;
		switch (type)
		{
		case EnemyType.Zombie:
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GoLimp();
			break;
		case EnemyType.Machine:
			if (machine == null)
			{
				machine = GetComponent<Machine>();
			}
			machine.GoLimp();
			break;
		case EnemyType.Statue:
			if (statue == null)
			{
				statue = GetComponent<Statue>();
			}
			statue.GoLimp();
			break;
		case EnemyType.Drone:
			if (drone == null)
			{
				drone = GetComponent<Drone>();
			}
			drone.GetHurt(Vector3.zero, 999f);
			drone.Explode();
			break;
		case EnemyType.Spider:
			if (spider == null)
			{
				spider = GetComponent<SpiderBody>();
			}
			if (!spider.dead)
			{
				spider.Die();
			}
			break;
		}
		if (usingDoor != null)
		{
			usingDoor.Close();
			usingDoor = null;
		}
	}

	public void Explode()
	{
		if (type == EnemyType.Zombie)
		{
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			if (exploded || zombie.chestExploding)
			{
				return;
			}
			exploded = true;
			if (zombie.chestExploding)
			{
				zombie.ChestExplodeEnd();
				Debug.Log("Premature Chest Explosion End #2");
			}
			if (!dead && magneted)
			{
				Object.FindObjectOfType<StyleHUD>().AddPoints(240, "<color=cyan>CATAPULTED</color>");
			}
			Transform[] componentsInChildren = zombie.GetComponentsInChildren<Transform>();
			GoreZone componentInParent = GetComponentInParent<GoreZone>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.tag == "Limb")
				{
					Object.Destroy(transform.GetComponent<CharacterJoint>());
					transform.transform.SetParent(componentInParent.gibZone, worldPositionStays: true);
				}
				else if (transform.gameObject.tag == "Head" || transform.gameObject.tag == "EndLimb")
				{
					zombie.GetHurt(transform.gameObject, (base.transform.position - transform.position).normalized * 1000f, 1E+09f, 1f);
				}
			}
			dead = true;
			if (usingDoor != null)
			{
				usingDoor.Close();
				usingDoor = null;
			}
		}
		else if (type == EnemyType.Machine)
		{
			if (machine == null)
			{
				machine = GetComponent<Machine>();
			}
			if (exploded)
			{
				return;
			}
			exploded = true;
			EnemyIdentifierIdentifier[] componentsInChildren2 = machine.GetComponentsInChildren<EnemyIdentifierIdentifier>();
			GoreZone componentInParent2 = GetComponentInParent<GoreZone>();
			EnemyIdentifierIdentifier[] array2 = componentsInChildren2;
			foreach (EnemyIdentifierIdentifier enemyIdentifierIdentifier in array2)
			{
				if (enemyIdentifierIdentifier.gameObject.tag == "Limb")
				{
					CharacterJoint component = enemyIdentifierIdentifier.GetComponent<CharacterJoint>();
					if (component != null)
					{
						Object.Destroy(component);
					}
					enemyIdentifierIdentifier.transform.SetParent(componentInParent2.gibZone, worldPositionStays: true);
				}
				else if (enemyIdentifierIdentifier.gameObject.tag == "Head" || enemyIdentifierIdentifier.gameObject.tag == "EndLimb")
				{
					machine.GetHurt(enemyIdentifierIdentifier.gameObject, (base.transform.position - enemyIdentifierIdentifier.transform.position).normalized * 1000f, 999f, 1f);
				}
			}
			dead = true;
			if (usingDoor != null)
			{
				usingDoor.Close();
				usingDoor = null;
			}
		}
		else if (type == EnemyType.Statue)
		{
			if (statue == null)
			{
				statue = GetComponent<Statue>();
			}
			if (!exploded)
			{
				exploded = true;
				statue.GoLimp();
				dead = true;
			}
		}
		else if (type == EnemyType.Spider)
		{
			if (spider == null)
			{
				spider = GetComponent<SpiderBody>();
			}
			if (!spider.dead)
			{
				hitter = "breaker";
				spider.Die();
			}
			else
			{
				spider.BreakCorpse();
			}
		}
	}

	public void Splatter()
	{
		switch (type)
		{
		case EnemyType.Zombie:
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			break;
		case EnemyType.Statue:
			if (statue == null)
			{
				statue = GetComponent<Statue>();
			}
			break;
		case EnemyType.Machine:
			if (machine == null)
			{
				machine = GetComponent<Machine>();
			}
			break;
		}
		if (exploded || (type == EnemyType.Zombie && zombie.chestExploding))
		{
			return;
		}
		exploded = true;
		limbs = GetComponentsInChildren<EnemyIdentifierIdentifier>();
		if (!dead)
		{
			SendMessage("GoLimp", SendMessageOptions.DontRequireReceiver);
			StyleHUD styleHUD = Object.FindObjectOfType<StyleHUD>();
			if (magneted)
			{
				styleHUD.AddPoints(120, "<color=cyan>CATAPULTED</color>");
			}
			styleHUD.AddPoints(100, "SPLATTERED");
			base.transform.Rotate(new Vector3(90f, 0f, 0f));
		}
		Object.Instantiate(splatterBlood, base.transform.position + Vector3.up, Quaternion.identity).GetComponent<Bloodsplatter>().GetReady();
		EnemyIdentifierIdentifier[] array = limbs;
		foreach (EnemyIdentifierIdentifier enemyIdentifierIdentifier in array)
		{
			if (enemyIdentifierIdentifier.gameObject.tag == "Body" || enemyIdentifierIdentifier.gameObject.tag == "Limb" || enemyIdentifierIdentifier.gameObject.tag == "Head" || enemyIdentifierIdentifier.gameObject.tag == "EndLimb")
			{
				Object.Destroy(enemyIdentifierIdentifier.GetComponent<CharacterJoint>());
				enemyIdentifierIdentifier.transform.SetParent(enemyIdentifierIdentifier.GetComponentInParent<GoreZone>().gibZone, worldPositionStays: true);
				Rigidbody component = enemyIdentifierIdentifier.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = Vector3.zero;
					enemyIdentifierIdentifier.transform.position = new Vector3(enemyIdentifierIdentifier.transform.position.x, base.transform.position.y + 0.1f, enemyIdentifierIdentifier.transform.position.z);
					Vector3 vector = new Vector3(base.transform.position.x - enemyIdentifierIdentifier.transform.position.x, 0f, base.transform.position.z - enemyIdentifierIdentifier.transform.position.z);
					component.AddForce(vector * 15f, ForceMode.VelocityChange);
					component.constraints = RigidbodyConstraints.FreezePositionY;
				}
			}
		}
		Invoke("StopSplatter", 1f);
		dead = true;
		if (usingDoor != null)
		{
			usingDoor.Close();
			usingDoor = null;
		}
	}

	public void StopSplatter()
	{
		EnemyIdentifierIdentifier[] array = limbs;
		foreach (EnemyIdentifierIdentifier enemyIdentifierIdentifier in array)
		{
			if (enemyIdentifierIdentifier != null)
			{
				Rigidbody component = enemyIdentifierIdentifier.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.constraints = RigidbodyConstraints.None;
				}
			}
		}
	}
}
