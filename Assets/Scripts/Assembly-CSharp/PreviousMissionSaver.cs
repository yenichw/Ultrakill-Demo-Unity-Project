using UnityEngine;

public class PreviousMissionSaver : MonoBehaviour
{
	public int previousMission;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
