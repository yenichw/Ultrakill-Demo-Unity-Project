using UnityEngine;

public class OneKillChallenge : MonoBehaviour
{
	public int kills;

	private void OnEnable()
	{
		ChallengeManager component = GetComponent<ChallengeManager>();
		if (GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().kills == kills)
		{
			component.ChallengeDone();
		}
	}
}
