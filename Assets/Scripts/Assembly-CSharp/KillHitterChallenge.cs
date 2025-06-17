using UnityEngine;

public class KillHitterChallenge : MonoBehaviour
{
	private void OnEnable()
	{
		KillHitterCache killHitterCache = Object.FindObjectOfType<KillHitterCache>();
		if ((bool)killHitterCache && killHitterCache.currentScore >= killHitterCache.neededScore)
		{
			GetComponent<ChallengeManager>().ChallengeDone();
		}
	}
}
