using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public float amount;

	private void Start()
	{
		Object.FindObjectOfType<CameraController>().CameraShake(amount);
		Object.Destroy(this);
	}
}
