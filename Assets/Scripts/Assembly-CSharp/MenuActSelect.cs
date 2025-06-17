using UnityEngine;
using UnityEngine.UI;

public class MenuActSelect : MonoBehaviour
{
	public int requiredLevels;

	public bool forceOff;

	private void OnEnable()
	{
		if (forceOff || GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff", 2)) < requiredLevels)
		{
			GetComponent<Button>().interactable = false;
			base.transform.GetChild(0).GetComponent<Text>().color = new Color(0.3f, 0.3f, 0.3f);
			base.transform.GetChild(1).gameObject.SetActive(value: false);
		}
		else
		{
			GetComponent<Button>().interactable = true;
			base.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f);
			base.transform.GetChild(1).gameObject.SetActive(value: true);
		}
	}
}
