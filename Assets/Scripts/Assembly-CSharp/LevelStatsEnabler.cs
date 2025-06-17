using UnityEngine;

public class LevelStatsEnabler : MonoBehaviour
{
	private GameObject levelStats;

	private bool keepOpen;

	private float doubleTap;

	private void Start()
	{
		StatsManager statsManager = Object.FindObjectOfType<StatsManager>();
		if (statsManager.levelNumber == 0 || GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff")) <= statsManager.levelNumber)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (PlayerPrefs.GetInt("LevStaTut", 0) == 0)
		{
			Invoke("LevelStatsTutorial", 1.5f);
		}
		levelStats = base.transform.GetChild(0).gameObject;
		levelStats.SetActive(value: false);
	}

	private void Update()
	{
		if (!keepOpen)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (!keepOpen)
				{
					if (doubleTap > 0f)
					{
						keepOpen = true;
					}
					else
					{
						doubleTap = 0.5f;
					}
				}
				levelStats.SetActive(value: true);
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
			{
				levelStats.SetActive(value: false);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Tab))
		{
			keepOpen = false;
			levelStats.SetActive(value: false);
		}
		if (doubleTap > 0f)
		{
			doubleTap = Mathf.MoveTowards(doubleTap, 0f, Time.deltaTime);
		}
	}

	private void LevelStatsTutorial()
	{
		PlayerPrefs.SetInt("LevStaTut", 1);
		HudMessager.SendHudMessage("Hold <color=orange>TAB</color> to see current stats when <color=orange>REPLAYING</color> a level.\n<color=orange>DOUBLE TAP</color> to keep open.");
	}
}
