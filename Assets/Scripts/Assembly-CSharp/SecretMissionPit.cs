using UnityEngine;

public class SecretMissionPit : MonoBehaviour
{
	public int missionNumber;

	public bool halfUnlock;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (halfUnlock)
			{
				GameProgressSaver.FoundSecretMission(missionNumber);
			}
			else
			{
				GameProgressSaver.SetSecretMission(missionNumber);
			}
		}
	}
}
