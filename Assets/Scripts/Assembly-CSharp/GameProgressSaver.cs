using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameProgressSaver
{
	public static void SaveProgress(int levelnum)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/difficulty", PlayerPrefs.GetInt("Diff", 2), "progress.bepis");
		if (File.Exists(path))
		{
			try
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressData gameProgressData = binaryFormatter.Deserialize(fileStream) as GameProgressData;
				fileStream.Close();
				if (gameProgressData.levelNum < levelnum || gameProgressData.difficulty != PlayerPrefs.GetInt("Diff", 2))
				{
					GameProgressData graph = new GameProgressData(levelnum);
					fileStream = new FileStream(path, FileMode.Create);
					binaryFormatter.Serialize(fileStream, graph);
					fileStream.Close();
				}
				return;
			}
			catch
			{
				Debug.Log("Save Failed");
				GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
				return;
			}
		}
		try
		{
			GameProgressData graph2 = new GameProgressData(levelnum);
			FileStream fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, graph2);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static int GetProgress(int difficulty)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/difficulty", difficulty, "progress.bepis");
		if (File.Exists(path))
		{
			try
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressData gameProgressData = binaryFormatter.Deserialize(fileStream) as GameProgressData;
				fileStream.Close();
				if (gameProgressData.difficulty == difficulty)
				{
					return gameProgressData.levelNum;
				}
				return 1;
			}
			catch
			{
				Debug.Log("Load Failed");
				GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
				return 1;
			}
		}
		return 1;
	}

	public static void AddGear(string gear)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			switch (gear)
			{
			case "rev0":
				gameProgressMoneyAndGear.rev0 = 1;
				break;
			case "rev1":
				gameProgressMoneyAndGear.rev1 = 1;
				break;
			case "rev2":
				gameProgressMoneyAndGear.rev2 = 1;
				break;
			case "rev3":
				gameProgressMoneyAndGear.rev3 = 1;
				break;
			case "sho0":
				gameProgressMoneyAndGear.sho0 = 1;
				break;
			case "sho1":
				gameProgressMoneyAndGear.sho1 = 1;
				break;
			case "sho2":
				gameProgressMoneyAndGear.sho2 = 1;
				break;
			case "sho3":
				gameProgressMoneyAndGear.sho3 = 1;
				break;
			case "nai0":
				gameProgressMoneyAndGear.nai0 = 1;
				break;
			case "nai1":
				gameProgressMoneyAndGear.nai1 = 1;
				break;
			case "nai2":
				gameProgressMoneyAndGear.nai2 = 1;
				break;
			case "nai3":
				gameProgressMoneyAndGear.nai3 = 1;
				break;
			case "rai0":
				gameProgressMoneyAndGear.rai0 = 1;
				break;
			case "rai1":
				gameProgressMoneyAndGear.rai1 = 1;
				break;
			case "rai2":
				gameProgressMoneyAndGear.rai2 = 1;
				break;
			case "rai3":
				gameProgressMoneyAndGear.rai3 = 1;
				break;
			case "arm1":
				gameProgressMoneyAndGear.arm1 = 1;
				break;
			case "arm2":
				gameProgressMoneyAndGear.arm2 = 1;
				break;
			case "arm3":
				gameProgressMoneyAndGear.arm3 = 1;
				break;
			}
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save/Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static int CheckGear(string gear)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				switch (gear)
				{
				case "rev0":
					return gameProgressMoneyAndGear.rev0;
				case "rev1":
					return gameProgressMoneyAndGear.rev1;
				case "rev2":
					return gameProgressMoneyAndGear.rev2;
				case "rev3":
					return gameProgressMoneyAndGear.rev3;
				case "sho0":
					return gameProgressMoneyAndGear.sho0;
				case "sho1":
					return gameProgressMoneyAndGear.sho1;
				case "sho2":
					return gameProgressMoneyAndGear.sho2;
				case "sho3":
					return gameProgressMoneyAndGear.sho3;
				case "nai0":
					return gameProgressMoneyAndGear.nai0;
				case "nai1":
					return gameProgressMoneyAndGear.nai1;
				case "nai2":
					return gameProgressMoneyAndGear.nai2;
				case "nai3":
					return gameProgressMoneyAndGear.nai3;
				case "rai0":
					return gameProgressMoneyAndGear.rai0;
				case "rai1":
					return gameProgressMoneyAndGear.rai1;
				case "rai2":
					return gameProgressMoneyAndGear.rai2;
				case "rai3":
					return gameProgressMoneyAndGear.rai3;
				case "arm1":
					return gameProgressMoneyAndGear.arm1;
				case "arm2":
					return gameProgressMoneyAndGear.arm2;
				case "arm3":
					return gameProgressMoneyAndGear.arm3;
				default:
					return 0;
				}
			}
			return 0;
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
			return 0;
		}
	}

	public static void LoadGear()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear obj = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				if (obj.rev0 > 0)
				{
					PlayerPrefs.SetInt("rev0", 1);
				}
				if (obj.rev1 > 0)
				{
					PlayerPrefs.SetInt("rev1", 1);
				}
				if (obj.rev2 > 0)
				{
					PlayerPrefs.SetInt("rev2", 1);
				}
				if (obj.rev3 > 0)
				{
					PlayerPrefs.SetInt("rev3", 1);
				}
				if (obj.sho0 > 0)
				{
					PlayerPrefs.SetInt("sho0", 1);
				}
				if (obj.sho1 > 0)
				{
					PlayerPrefs.SetInt("sho1", 1);
				}
				if (obj.sho2 > 0)
				{
					PlayerPrefs.SetInt("sho2", 1);
				}
				if (obj.sho3 > 0)
				{
					PlayerPrefs.SetInt("sho3", 1);
				}
				if (obj.nai0 > 0)
				{
					PlayerPrefs.SetInt("nai0", 1);
				}
				if (obj.nai1 > 0)
				{
					PlayerPrefs.SetInt("nai1", 1);
				}
				if (obj.nai2 > 0)
				{
					PlayerPrefs.SetInt("nai2", 1);
				}
				if (obj.nai3 > 0)
				{
					PlayerPrefs.SetInt("nai3", 1);
				}
				if (obj.rai0 > 0)
				{
					PlayerPrefs.SetInt("rai0", 1);
				}
				if (obj.rai1 > 0)
				{
					PlayerPrefs.SetInt("rai1", 1);
				}
				if (obj.rai2 > 0)
				{
					PlayerPrefs.SetInt("rai2", 1);
				}
				if (obj.rai3 > 0)
				{
					PlayerPrefs.SetInt("rai3", 1);
				}
			}
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static void AddMoney(int money)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			gameProgressMoneyAndGear.money += money;
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static int GetMoney()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear obj = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				return obj.money;
			}
			return 0;
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
			return 0;
		}
	}

	public static bool GetIntro()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear obj = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				return obj.introSeen;
			}
			return false;
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
			return false;
		}
	}

	public static void SetIntro(bool beat)
	{
		Debug.Log("Setting Intro");
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		try
		{
			Directory.CreateDirectory(string.Concat(Directory.GetParent(Application.dataPath), "/Saves"));
			string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			gameProgressMoneyAndGear.introSeen = beat;
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static bool GetTutorial()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear obj = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				return obj.tutorialBeat;
			}
			return false;
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
			return false;
		}
	}

	public static void SetTutorial(bool beat)
	{
		Debug.Log("Setting Tutorial");
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		try
		{
			Directory.CreateDirectory(string.Concat(Directory.GetParent(Application.dataPath), "/Saves"));
			string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			gameProgressMoneyAndGear.tutorialBeat = beat;
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static int GetSecretMission(int missionNumber)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			if (File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
				if (gameProgressMoneyAndGear.secretMissions.Length == 0)
				{
					gameProgressMoneyAndGear.secretMissions = new int[10];
				}
				return gameProgressMoneyAndGear.secretMissions[missionNumber];
			}
			return 0;
		}
		catch
		{
			Debug.Log("Load Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
			return 0;
		}
	}

	public static void FoundSecretMission(int missionNumber)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			if (gameProgressMoneyAndGear.secretMissions[missionNumber] != 2)
			{
				gameProgressMoneyAndGear.secretMissions[missionNumber] = 1;
			}
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}

	public static void SetSecretMission(int missionNumber)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/generalprogress.bepis");
		try
		{
			GameProgressMoneyAndGear gameProgressMoneyAndGear;
			FileStream fileStream;
			if (File.Exists(path))
			{
				fileStream = new FileStream(path, FileMode.Open);
				gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
				fileStream.Close();
			}
			else
			{
				gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
				gameProgressMoneyAndGear.secretMissions = new int[10];
			}
			gameProgressMoneyAndGear.secretMissions[missionNumber] = 2;
			fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save Failed");
			GameObject.FindWithTag("OptionsManager").GetComponent<ProgressChecker>().SaveLoadError();
		}
	}
}
