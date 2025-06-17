using UnityEngine;
using UnityEngine.UI;

public class LevelStats : MonoBehaviour
{
	public Text levelName;

	private StatsManager sman;

	private bool ready;

	public Text time;

	public Text timeRank;

	private float seconds;

	private float minutes;

	public Text kills;

	public Text killsRank;

	public Text style;

	public Text styleRank;

	public Text secrets;

	private int currentSecrets;

	public Text challenge;

	public Text majorAssists;

	private void Start()
	{
		sman = Object.FindObjectOfType<StatsManager>();
		if (sman.levelNumber != 0 && GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff")) > sman.levelNumber)
		{
			LevelActivity levelActivity = Object.FindObjectOfType<LevelActivity>();
			if (levelActivity != null)
			{
				levelName.text = levelActivity.Assets.LargeText;
			}
			else
			{
				levelName.text = "???";
			}
			ready = true;
			CheckStats();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (ready)
		{
			CheckStats();
		}
	}

	private void CheckStats()
	{
		seconds = sman.seconds;
		minutes = 0f;
		while (seconds >= 60f)
		{
			seconds -= 60f;
			minutes += 1f;
		}
		time.text = minutes + ":" + seconds.ToString("00.000");
		timeRank.text = sman.GetRanks(sman.timeRanks, sman.seconds, reverse: true);
		kills.text = sman.kills.ToString();
		killsRank.text = sman.GetRanks(sman.killRanks, sman.kills, reverse: false);
		style.text = sman.stylePoints.ToString();
		styleRank.text = sman.GetRanks(sman.styleRanks, sman.stylePoints, reverse: false);
		int num = sman.secrets + sman.prevSecrets.Count;
		secrets.text = num + "/" + sman.maxSecrets;
		if (sman.challengeComplete)
		{
			challenge.text = "<color=#FFAF00>YES</color>";
		}
		else
		{
			challenge.text = "NO";
		}
		if (sman.majorUsed)
		{
			majorAssists.text = "<color=#4C99E6>YES</color>";
		}
		else
		{
			majorAssists.text = "NO";
		}
	}
}
