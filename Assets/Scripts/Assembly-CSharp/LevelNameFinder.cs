using UnityEngine;
using UnityEngine.UI;

public class LevelNameFinder : MonoBehaviour
{
	public string textBeforeName;

	public bool breakLine;

	private int thisLevelNumber;

	public int otherLevelNumber;

	private Text targetText;

	public bool lookForPreviousMission;

	public bool lookForLatestMission;

	private void OnEnable()
	{
		if (targetText == null)
		{
			targetText = GetComponent<Text>();
		}
		if (lookForPreviousMission || lookForLatestMission)
		{
			bool flag = false;
			if (lookForPreviousMission)
			{
				PreviousMissionSaver previousMissionSaver = Object.FindObjectOfType<PreviousMissionSaver>();
				if (previousMissionSaver != null)
				{
					flag = true;
					otherLevelNumber = previousMissionSaver.previousMission;
				}
			}
			if (!flag && lookForLatestMission)
			{
				otherLevelNumber = GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff"));
			}
		}
		if (!lookForPreviousMission || !(targetText != null))
		{
			return;
		}
		string text = "";
		if (breakLine)
		{
			text = "\n";
		}
		if (otherLevelNumber != 0)
		{
			targetText.text = textBeforeName + text + GetMissionName.GetMission(otherLevelNumber);
			return;
		}
		if (thisLevelNumber == 0)
		{
			thisLevelNumber = Object.FindObjectOfType<StatsManager>().levelNumber;
		}
		targetText.text = textBeforeName + text + thisLevelNumber;
	}
}
