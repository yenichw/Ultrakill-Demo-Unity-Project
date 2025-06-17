using UnityEngine;

public class SlowMo : MonoBehaviour
{
	public float amount;

	private void OnEnable()
	{
		Object.FindObjectOfType<CameraController>().SlowDown(amount);
		Object.Destroy(this);
	}
}
