using UnityEngine;
using UnityEngine.UI;

public class HudMessageReceiver : MonoBehaviour
{
	private Image img;

	private Text text;

	private AudioSource aud;

	private HudOpenEffect hoe;

	private string message;

	private string input;

	private string message2;

	private bool noSound;

	private void Start()
	{
		img = GetComponent<Image>();
		text = GetComponentInChildren<Text>();
		aud = GetComponent<AudioSource>();
		hoe = GetComponent<HudOpenEffect>();
	}

	private void Done()
	{
		img.enabled = false;
		text.enabled = false;
	}

	public void SendHudMessage(string newmessage, string newinput = "", string newmessage2 = "", int delay = 0, bool silent = false)
	{
		message = newmessage;
		input = newinput;
		message2 = newmessage2;
		noSound = silent;
		Invoke("ShowHudMessage", delay);
	}

	private void ShowHudMessage()
	{
		if (input == "")
		{
			this.text.text = message;
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
			this.text.text = message + text + message2;
		}
		this.text.text = this.text.text.Replace('$', '\n');
		this.text.enabled = true;
		img.enabled = true;
		hoe.Force();
		if (!noSound)
		{
			aud.Play();
		}
		CancelInvoke("Done");
		Invoke("Done", 5f);
	}
}
