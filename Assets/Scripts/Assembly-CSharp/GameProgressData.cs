using System;
using UnityEngine;

[Serializable]
public class GameProgressData
{
	public int levelNum;

	public int difficulty;

	public GameProgressData(int lvl)
	{
		levelNum = lvl;
		difficulty = PlayerPrefs.GetInt("Diff", 2);
	}
}
