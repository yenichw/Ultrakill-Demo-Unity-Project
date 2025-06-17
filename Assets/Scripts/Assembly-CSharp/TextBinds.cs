using UnityEngine;
using UnityEngine.UI;

public class TextBinds : MonoBehaviour
{
	public string text1;

	public string input;

	public string text2;

	private Text text;

	private void OnEnable()
	{
		if (!this.text)
		{
			this.text = GetComponent<Text>();
		}
		if (input == "")
		{
			this.text.text = text1;
		}
		else
		{
			string text = "";
			KeyCode keyCode = Object.FindObjectOfType<InputManager>().Inputs[input];
			switch (keyCode)
			{
			case KeyCode.Mouse0:
				text = "Left Mouse Button";
				break;
			case KeyCode.Mouse1:
				text = "Right Mouse Button";
				break;
			case KeyCode.Mouse2:
				text = "Middle Mouse Button";
				break;
			default:
				text = keyCode.ToString();
				break;
			}
			this.text.text = text1 + text + text2;
		}
		this.text.text = this.text.text.Replace('$', '\n');
	}
}
