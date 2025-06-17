using UnityEngine;

public class Follow : MonoBehaviour
{
	public float speed;

	public GameObject target;

	private GameObject player;

	private void Awake()
	{
		player = GameObject.FindWithTag("Player");
		if (target == null)
		{
			target = player;
		}
	}

	private void Update()
	{
		float maxDistanceDelta = speed * Time.deltaTime;
		base.transform.position = Vector3.MoveTowards(base.transform.position, target.transform.position, maxDistanceDelta);
	}
}
