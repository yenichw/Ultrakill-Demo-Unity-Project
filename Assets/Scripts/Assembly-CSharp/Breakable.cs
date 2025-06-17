using UnityEngine;

public class Breakable : MonoBehaviour
{
	public bool weak;

	public bool precisionOnly;

	public bool interrupt;

	public bool playerOnly;

	public GameObject breakParticle;

	public GameObject[] activateOnBreak;

	public GameObject[] destroyOnBreak;

	private bool broken;

	public void Break()
	{
		if (broken)
		{
			return;
		}
		broken = true;
		if (breakParticle != null)
		{
			Object.Instantiate(breakParticle, base.transform.position, base.transform.rotation);
		}
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		if (componentsInChildren.Length != 0)
		{
			Rigidbody[] array = componentsInChildren;
			foreach (Rigidbody obj in array)
			{
				obj.transform.SetParent(base.transform.parent, worldPositionStays: true);
				obj.isKinematic = false;
				obj.useGravity = true;
			}
		}
		if (activateOnBreak.Length != 0)
		{
			GameObject[] array2 = activateOnBreak;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(value: true);
				}
			}
		}
		if (destroyOnBreak.Length != 0)
		{
			GameObject[] array2 = destroyOnBreak;
			foreach (GameObject gameObject2 in array2)
			{
				if (gameObject2 != null)
				{
					Object.Destroy(gameObject2);
				}
			}
		}
		Object.Destroy(base.gameObject);
	}
}
