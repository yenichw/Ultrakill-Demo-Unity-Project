using System.Collections.Generic;
using UnityEngine;

public class KillHitterCache : MonoBehaviour
{
	public int neededScore;

	public int currentScore;

	private List<int> eids = new List<int>();

	public void OneDone(int enemyId)
	{
		if (eids.Count == 0 || !eids.Contains(enemyId))
		{
			currentScore++;
			eids.Add(enemyId);
		}
	}

	public void RemoveId(int enemyId)
	{
		if (eids.Contains(enemyId))
		{
			currentScore--;
			eids.Remove(enemyId);
		}
	}
}
