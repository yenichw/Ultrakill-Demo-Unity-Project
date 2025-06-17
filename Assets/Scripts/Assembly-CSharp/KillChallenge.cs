using UnityEngine;

public class KillChallenge : MonoBehaviour
{
	public void Done()
	{
		GetComponent<ChallengeManager>().ChallengeDone();
	}
}
