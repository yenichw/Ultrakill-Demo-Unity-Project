using UnityEngine;

public class RemoveOnTime : MonoBehaviour
{
	public float time;

	private void Start()
	{
		Invoke("Remove", time);
	}

	private void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}
