using UnityEngine;

public class Envirokills : MonoBehaviour
{
	private StatsManager sman;

	public enviroKillType ekt;

	public int killAmount;

	private ChallengeManager cman;

	private void OnEnable()
	{
		Debug.Log("Challenge is on");
		sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		cman = GetComponent<ChallengeManager>();
		if (ekt == enviroKillType.Glass && killAmount <= sman.maxGlassKills)
		{
			cman.ChallengeDone();
		}
	}
}
