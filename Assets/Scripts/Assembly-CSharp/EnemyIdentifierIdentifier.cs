using UnityEngine;

public class EnemyIdentifierIdentifier : MonoBehaviour
{
	public EnemyIdentifier eid;

	private bool deactivated;

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
		SlowCheck();
	}

	private void SlowCheck()
	{
		if (base.gameObject.activeInHierarchy && (base.transform.position.y < -300f || base.transform.position.x < -1000f || base.transform.position.x > 1000f || base.transform.position.z < -1000f || base.transform.position.z > 1000f))
		{
			deactivated = true;
			CharacterJoint[] componentsInChildren = GetComponentsInChildren<CharacterJoint>();
			Rigidbody[] componentsInChildren2 = GetComponentsInChildren<Rigidbody>();
			CharacterJoint[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
			Rigidbody[] array2 = componentsInChildren2;
			foreach (Rigidbody rigidbody in array2)
			{
				if (rigidbody != null)
				{
					rigidbody.useGravity = false;
					rigidbody.isKinematic = true;
				}
			}
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
		}
		if (!deactivated)
		{
			Invoke("SlowCheck", 3f);
		}
	}
}
