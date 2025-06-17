using UnityEngine;

public class DoorOpener : MonoBehaviour
{
	public Door door;

	private void OnEnable()
	{
		door.Open(enemy: false, skull: true);
	}
}
