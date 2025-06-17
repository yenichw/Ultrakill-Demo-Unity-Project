using UnityEngine;

public class MouseEnable : MonoBehaviour
{
	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Object.Destroy(Object.FindObjectOfType<PlayerPosInfo>().gameObject);
	}
}
