using System.Collections.Generic;
using UnityEngine;

public class GoreZone : MonoBehaviour
{
	public Transform goreZone;

	public Transform gibZone;

	public CheckPoint checkpoint;

	public float maxGore;

	public int goreAmount;

	public List<GameObject> outsideGore = new List<GameObject>();

	private void Start()
	{
		maxGore = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>().maxGore;
		if (goreZone == null)
		{
			GameObject gameObject = new GameObject();
			goreZone = gameObject.transform;
			goreZone.SetParent(base.transform, worldPositionStays: true);
		}
		if (gibZone == null)
		{
			GameObject gameObject2 = new GameObject();
			gibZone = gameObject2.transform;
			gibZone.SetParent(base.transform, worldPositionStays: true);
		}
		SlowUpdate();
	}

	private void SlowUpdate()
	{
		goreAmount = goreZone.childCount;
		if ((float)goreZone.childCount > maxGore)
		{
			int num = Mathf.RoundToInt((float)goreZone.childCount - maxGore);
			for (int i = 0; i < num; i++)
			{
				Object.Destroy(goreZone.GetChild(i).gameObject);
			}
		}
		if ((float)gibZone.childCount > maxGore / 10f)
		{
			int num2 = Mathf.RoundToInt((float)gibZone.childCount - maxGore / 10f);
			for (int j = 0; j < num2; j++)
			{
				Object.Destroy(gibZone.GetChild(j).gameObject);
			}
		}
		if ((float)outsideGore.Count > maxGore / 2f)
		{
			int num3 = Mathf.RoundToInt((float)outsideGore.Count - maxGore);
			for (int k = 0; k < num3; k++)
			{
				Object.Destroy(outsideGore[k].gameObject);
			}
		}
		Invoke("SlowUpdate", 1f);
	}

	public void Combine()
	{
		StaticBatchingUtility.Combine(goreZone.gameObject);
	}

	public void AddDeath()
	{
		checkpoint.restartDeaths++;
	}

	public void AddKillHitterTarget(int id)
	{
		if ((bool)checkpoint && !checkpoint.succesfulHitters.Contains(id))
		{
			checkpoint.succesfulHitters.Add(id);
		}
	}
}
