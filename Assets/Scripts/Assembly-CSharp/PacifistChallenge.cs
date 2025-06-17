using UnityEngine;

public class PacifistChallenge : MonoBehaviour
{
	private void OnEnable()
	{
		ChallengeManager component = GetComponent<ChallengeManager>();
		if (!GameObject.FindWithTag("StyleHUD").GetComponentInChildren<StyleCalculator>().enemiesShot)
		{
			component.ChallengeDone();
		}
	}
}
