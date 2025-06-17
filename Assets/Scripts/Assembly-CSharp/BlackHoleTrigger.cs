using UnityEngine;

public class BlackHoleTrigger : MonoBehaviour
{
	private BlackHoleProjectile bhp;

	private void Start()
	{
		bhp = GetComponentInParent<BlackHoleProjectile>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 12 && other.GetComponent<EnemyIdentifier>() != null)
		{
			bhp.shootList.Add(other.GetComponent<EnemyIdentifier>());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 12 && other.GetComponent<EnemyIdentifier>() != null)
		{
			bhp.shootList.Remove(other.GetComponent<EnemyIdentifier>());
		}
	}
}
