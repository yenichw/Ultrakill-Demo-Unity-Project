using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressChecker : MonoBehaviour
{
	public bool continueWithoutSaving;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!GameProgressSaver.GetTutorial() || !GameProgressSaver.GetIntro())
		{
			SceneManager.LoadScene("Tutorial");
			continueWithoutSaving = false;
		}
	}

	public void SaveLoadError()
	{
		if (!continueWithoutSaving)
		{
			continueWithoutSaving = true;
			Object.FindObjectOfType<SaveLoadFailMessage>().ShowError();
		}
	}
}
