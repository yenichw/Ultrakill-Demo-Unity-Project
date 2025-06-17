using UnityEngine;
using UnityEngine.SceneManagement;

public class AbruptLevelChanger : MonoBehaviour
{
	public bool saveMission;

	public void AbruptChangeLevel(string levelname)
	{
		if (saveMission)
		{
			PreviousMissionSaver previousMissionSaver = Object.FindObjectOfType<PreviousMissionSaver>();
			if (previousMissionSaver == null)
			{
				previousMissionSaver = new GameObject().AddComponent<PreviousMissionSaver>();
			}
			previousMissionSaver.previousMission = Object.FindObjectOfType<StatsManager>().levelNumber;
		}
		SceneManager.LoadScene(levelname);
	}

	public void NormalChangeLevel(string levelname)
	{
		Object.FindObjectOfType<OptionsManager>().ChangeLevel(levelname);
	}

	public void PositionChangeLevel(string levelname)
	{
		Object.FindObjectOfType<OptionsManager>().ChangeLevelWithPosition(levelname);
	}

	public void GoToLevel(int missionNumber)
	{
		SceneManager.LoadScene(GetMissionName.GetSceneName(missionNumber));
	}

	public void GoToSavedLevel()
	{
		PreviousMissionSaver previousMissionSaver = Object.FindObjectOfType<PreviousMissionSaver>();
		if (previousMissionSaver != null)
		{
			_ = previousMissionSaver.previousMission;
			Object.Destroy(previousMissionSaver.gameObject);
			GoToLevel(previousMissionSaver.previousMission);
		}
		else
		{
			GoToLevel(GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff")));
		}
	}
}
