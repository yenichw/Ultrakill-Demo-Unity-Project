using System.Collections.Generic;
using UnityEngine;

public class EnemyScanner : MonoBehaviour
{
	private EnemyIdentifier owner;

	private EnemyType ownerType;

	private SwordsMachine sman;

	private Zombie zom;

	private Collider col;

	private bool active = true;

	public float updateFrequency;

	public List<GameObject> spottedEnemies = new List<GameObject>();

	private List<Collider> checkedCols = new List<Collider>();

	private void Start()
	{
		owner = GetComponentInParent<EnemyIdentifier>();
		ownerType = owner.type;
		col = GetComponent<Collider>();
		if (ownerType == EnemyType.Machine)
		{
			sman = owner.GetComponent<SwordsMachine>();
		}
		else if (ownerType == EnemyType.Zombie)
		{
			zom = owner.GetComponent<Zombie>();
			if (!zom.friendly)
			{
				active = false;
			}
		}
		if (updateFrequency == 0f)
		{
			updateFrequency = Random.Range(0.4f, 0.6f);
		}
		col.enabled = false;
		CheckScanner();
	}

	private void Update()
	{
		if (ownerType == EnemyType.Zombie)
		{
			if (!zom.friendly)
			{
				active = false;
			}
			else
			{
				active = true;
			}
		}
	}

	private void CheckScanner()
	{
		if (active)
		{
			col.enabled = true;
		}
		Invoke("StopCheckScanner", 0.1f);
	}

	private void StopCheckScanner()
	{
		if (active)
		{
			col.enabled = false;
		}
		Invoke("CheckScanner", updateFrequency);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (owner == null)
		{
			owner = GetComponentInParent<EnemyIdentifier>();
		}
		if (!(other.gameObject.tag == "Enemy") || !(other.transform != owner.transform) || checkedCols.Contains(other))
		{
			return;
		}
		checkedCols.Add(other);
		EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
		if (component != null && component.type != ownerType && !component.dead && !component.ignoredByEnemies)
		{
			if (ownerType == EnemyType.Machine && sman != null && !sman.enemyTargets.Contains(component))
			{
				sman.enemyTargets.Add(component);
			}
			else if (ownerType == EnemyType.Zombie && zom != null && !zom.enemyTargets.Contains(component))
			{
				zom.enemyTargets.Add(component);
			}
		}
	}
}
