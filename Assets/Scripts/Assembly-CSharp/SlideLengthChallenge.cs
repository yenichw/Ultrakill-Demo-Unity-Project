using UnityEngine;

public class SlideLengthChallenge : MonoBehaviour
{
	public float slideLength;

	private void OnEnable()
	{
		if (GameObject.FindWithTag("Player").GetComponent<NewMovement>().longestSlide >= slideLength)
		{
			GetComponent<ChallengeManager>().ChallengeDone();
		}
	}
}
