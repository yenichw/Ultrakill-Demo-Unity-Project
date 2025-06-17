using UnityEngine;

public class Spin : MonoBehaviour
{
	public Vector3 spinDirection;

	public float speed;

	public bool inLateUpdate;

	private Vector3 totalRotation;

	private void Start()
	{
	}

	private void Update()
	{
		if (!inLateUpdate)
		{
			base.transform.Rotate(spinDirection, speed * Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (inLateUpdate)
		{
			if (totalRotation == Vector3.zero)
			{
				totalRotation = base.transform.localRotation.eulerAngles;
			}
			base.transform.localRotation = Quaternion.Euler(totalRotation);
			base.transform.Rotate(spinDirection, speed * Time.deltaTime);
			totalRotation = base.transform.localRotation.eulerAngles;
		}
	}
}
