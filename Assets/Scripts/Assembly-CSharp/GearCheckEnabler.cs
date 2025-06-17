using UnityEngine;

public class GearCheckEnabler : MonoBehaviour
{
	public string gear;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	private void Start()
	{
		if (GameProgressSaver.CheckGear(gear) > 0 && PlayerPrefs.GetInt("FullIntro", 0) == 0)
		{
			GameObject[] array = toActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = toDisactivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
