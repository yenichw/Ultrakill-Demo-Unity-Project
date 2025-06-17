using UnityEngine;

public class DoorlessChallenge : MonoBehaviour
{
	public bool failed;

	private void OnEnable()
	{
		if (!failed)
		{
			GetComponent<ChallengeManager>().ChallengeDone();
		}
	}
}
