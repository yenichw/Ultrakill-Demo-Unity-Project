using UnityEngine;

public class StyleComboRank : MonoBehaviour
{
	public int rankToReach;

	private StyleHUD shud;

	private ChallengeManager cman;

	private void OnEnable()
	{
		cman = GetComponent<ChallengeManager>();
		shud = GameObject.FindWithTag("StyleHUD").GetComponentInChildren<StyleHUD>();
		if (shud.maxReachedRank >= rankToReach)
		{
			cman.ChallengeDone();
		}
	}
}
