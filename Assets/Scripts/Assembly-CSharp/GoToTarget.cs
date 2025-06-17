using UnityEngine;

public class GoToTarget : MonoBehaviour
{
	public ToDo onTargetReach;

	public float speed;

	public Transform target;

	private Rigidbody rb;

	private bool stopped;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (stopped)
		{
			return;
		}
		rb.velocity = (target.position - base.transform.position).normalized * speed * Time.deltaTime;
		if (Vector3.Distance(base.transform.position, target.position) < 0.5f)
		{
			switch (onTargetReach)
			{
			case ToDo.Disable:
				base.gameObject.SetActive(value: false);
				break;
			case ToDo.Destroy:
				Object.Destroy(base.gameObject);
				break;
			default:
				stopped = true;
				rb.velocity = Vector3.zero;
				break;
			}
		}
	}
}
