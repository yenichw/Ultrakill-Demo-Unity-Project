using UnityEngine;
using UnityEngine.UI;

public class DifficultyTitle : MonoBehaviour
{
	public bool lines;

	private Text txt;

	private void Start()
	{
		Check();
	}

	private void OnEnable()
	{
		Check();
	}

	private void Check()
	{
		int @int = PlayerPrefs.GetInt("Diff", 2);
		if (txt == null)
		{
			txt = GetComponent<Text>();
		}
		txt.text = "";
		if (lines)
		{
			txt.text += "-- ";
		}
		switch (@int)
		{
		case 0:
			txt.text += "CASUAL EASY";
			break;
		case 1:
			txt.text += "CASUAL HARD";
			break;
		case 2:
			txt.text += "STANDARD";
			break;
		case 3:
			txt.text += "VIOLENT";
			break;
		case 4:
			txt.text += "BRUTAL";
			break;
		case 5:
			txt.text += "ULTRAKILL MUST DIE";
			break;
		}
		if (lines)
		{
			txt.text += " --";
		}
	}
}
