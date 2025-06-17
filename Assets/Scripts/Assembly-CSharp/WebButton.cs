using UnityEngine;

public class WebButton : MonoBehaviour
{
	public string url;

	public void OpenURL()
	{
		Application.OpenURL(url);
	}
}
