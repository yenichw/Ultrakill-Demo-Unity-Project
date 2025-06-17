using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
	public GameObject challengePanel;

	public FinalRank fr;

	private bool newChallengeDone;

	private void Awake()
	{
		if (fr == null)
		{
			fr = GetComponentInParent<FinalRank>();
		}
	}

	private void OnEnable()
	{
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		if (!newChallengeDone && component.challengeComplete)
		{
			challengePanel.GetComponent<Image>().color = new Color(1f, 0.696f, 0f, 0.5f);
			challengePanel.GetComponent<AudioSource>().volume = 0f;
			challengePanel.SetActive(value: true);
		}
	}

	public void ChallengeDone()
	{
		if (fr == null)
		{
			fr = GetComponentInParent<FinalRank>();
		}
		challengePanel.SetActive(value: true);
		RankSaveSystem.ChallengeComplete();
		newChallengeDone = true;
		if (fr != null)
		{
			fr.FlashPanel(challengePanel.transform.parent.GetChild(2).gameObject);
		}
	}
}
