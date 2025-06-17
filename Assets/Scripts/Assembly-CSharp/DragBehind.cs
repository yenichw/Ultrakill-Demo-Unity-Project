using UnityEngine;

public class DragBehind : MonoBehaviour
{
	private Vector3 previousPosition;

	private Quaternion currentRotation;

	private Quaternion nextRotation;

	private Quaternion previousRotation;

	public bool active;

	private void Awake()
	{
		previousPosition = base.transform.position;
		previousRotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		if (active)
		{
			currentRotation = base.transform.rotation;
			Quaternion rotation = Quaternion.LookRotation(previousPosition - base.transform.position, base.transform.right);
			base.transform.rotation = rotation;
			base.transform.up = base.transform.forward;
			nextRotation = Quaternion.Lerp(currentRotation, base.transform.rotation, Vector3.Distance(base.transform.position, previousPosition) / 5f);
			base.transform.rotation = Quaternion.RotateTowards(previousRotation, nextRotation, Time.deltaTime * 1000f);
		}
		previousPosition = Vector3.MoveTowards(previousPosition, base.transform.position, Time.deltaTime * (Vector3.Distance(previousPosition, base.transform.position) * 10f));
		previousRotation = base.transform.rotation;
	}
}
