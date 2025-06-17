using UnityEngine;

public class SaveLoadFailMessage : MonoBehaviour
{
	private bool beenActivated;

	public void ShowError()
	{
		base.transform.GetChild(0).gameObject.SetActive(value: true);
		beenActivated = true;
	}

	private void Update()
	{
		if (beenActivated)
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				base.transform.GetChild(0).gameObject.SetActive(value: false);
				beenActivated = false;
			}
			if (Input.GetKeyDown(KeyCode.N))
			{
				Application.Quit();
			}
		}
	}
}
