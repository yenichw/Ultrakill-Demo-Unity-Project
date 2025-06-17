using UnityEngine;

public class WaterVolumeTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "CameraCollisionChecker")
		{
			Shader.SetGlobalInt("_IsUnderwater", 1);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name == "CameraCollisionChecker")
		{
			Shader.SetGlobalInt("_IsUnderwater", 0);
		}
	}
}
