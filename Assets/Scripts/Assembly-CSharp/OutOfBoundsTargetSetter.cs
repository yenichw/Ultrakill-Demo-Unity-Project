using UnityEngine;

public class OutOfBoundsTargetSetter : MonoBehaviour
{
	private DeathZone[] dzs;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (dzs == null)
			{
				dzs = Object.FindObjectsOfType<DeathZone>();
			}
			DeathZone[] array = dzs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].respawnTarget = base.transform.position;
			}
		}
	}
}
