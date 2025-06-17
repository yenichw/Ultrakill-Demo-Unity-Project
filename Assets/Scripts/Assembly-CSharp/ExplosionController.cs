using UnityEngine;

public class ExplosionController : MonoBehaviour
{
	public GameObject[] toActivate;

	public string playerPref;

	private void Start()
	{
		if (PlayerPrefs.GetInt(playerPref, 0) == 0)
		{
			GameObject[] array = toActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
	}
}
