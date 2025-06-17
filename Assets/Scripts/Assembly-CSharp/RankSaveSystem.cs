using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class RankSaveSystem
{
	public static void SaveRank(StatsManager sman)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/lvl", sman.levelNumber, "progress.bepis");
		RankData graph = new RankData(sman);
		FileStream fileStream = new FileStream(path, FileMode.Create);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	public static void ChallengeComplete()
	{
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/lvl", component.levelNumber, "progress.bepis");
		Debug.Log("RSS Step 1");
		if (File.Exists(path))
		{
			Debug.Log("RSS Step 2");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
			fileStream.Close();
			if (!rankData.challenge && rankData.levelNumber == component.levelNumber)
			{
				rankData.challenge = true;
				Debug.Log("RSS Step 3");
				fileStream = new FileStream(path, FileMode.Create);
				binaryFormatter.Serialize(fileStream, rankData);
				fileStream.Close();
			}
		}
		else
		{
			Debug.Log("RSS Step 2 File Not Found");
			BinaryFormatter binaryFormatter2 = new BinaryFormatter();
			RankData rankData = new RankData(component)
			{
				challenge = true
			};
			FileStream fileStream2 = new FileStream(path, FileMode.Create);
			binaryFormatter2.Serialize(fileStream2, rankData);
			fileStream2.Close();
		}
	}

	public static void SecretFound(int secretnum)
	{
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/lvl", component.levelNumber, "progress.bepis");
		if (File.Exists(path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
			fileStream.Close();
			if (rankData.levelNumber == component.levelNumber && !rankData.secretsFound[secretnum])
			{
				rankData.secretsFound[secretnum] = true;
				fileStream = new FileStream(path, FileMode.Create);
				binaryFormatter.Serialize(fileStream, rankData);
				fileStream.Close();
			}
		}
		else
		{
			BinaryFormatter binaryFormatter2 = new BinaryFormatter();
			RankData rankData = new RankData(component);
			bool[] array = new bool[component.maxSecrets];
			array[secretnum] = true;
			rankData.secretsFound = array;
			FileStream fileStream2 = new FileStream(path, FileMode.Create);
			binaryFormatter2.Serialize(fileStream2, rankData);
			fileStream2.Close();
		}
	}
}
