using UnityEngine;

public class Screenshaker : MonoBehaviour
{
	public float amount;

	public bool oneTime;

	private void OnEnable()
	{
		Object.FindObjectOfType<CameraController>().CameraShake(amount);
		if (oneTime)
		{
			Object.Destroy(this);
		}
	}
}
