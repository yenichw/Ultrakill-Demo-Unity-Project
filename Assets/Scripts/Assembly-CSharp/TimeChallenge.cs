using UnityEngine;

public class TimeChallenge : MonoBehaviour
{
	public float time;

	private void OnEnable()
	{
		ChallengeManager component = GetComponent<ChallengeManager>();
		if (GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().seconds < time)
		{
			component.ChallengeDone();
		}
	}
}
