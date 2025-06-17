using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
	private Text txt;

	private string fullString;

	private string tempString;

	private StringBuilder sb;

	private bool doneWithDots;

	private bool readyToContinue;

	private bool waitingForInput;

	private AudioSource aud;

	private int dotsAmount = 3;

	private int calibrated;

	public GameObject[] calibrationWindows;

	public GameObject[] activateOnEnd;

	public GameObject[] deactivateOnEnd;

	public GameObject[] activateOnTextTrigger;

	private string colorString;

	private List<int> colorsPositions = new List<int>();

	private List<int> colorsClosePositions = new List<int>();

	private void Start()
	{
		txt = GetComponent<Text>();
		fullString = txt.text;
		aud = GetComponent<AudioSource>();
		StartCoroutine(TextAppear());
	}

	private void Update()
	{
		if (waitingForInput)
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				waitingForInput = false;
			}
			else if (Input.GetKeyDown(KeyCode.N))
			{
				Over();
			}
		}
	}

	public void DoneWithSetting()
	{
		calibrationWindows[calibrated].SetActive(value: false);
		calibrated++;
		readyToContinue = true;
	}

	private IEnumerator TextAppear()
	{
		int j = fullString.Length;
		for (int i = 0; i < j; i++)
		{
			char c = fullString[i];
			float waitTime = 0.035f;
			bool playSound = true;
			switch (c)
			{
			case '§':
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i);
				tempString = txt.text;
				doneWithDots = false;
				dotsAmount = 2;
				StartCoroutine(DotsAppear());
				yield return new WaitUntil(() => doneWithDots);
				break;
			case '#':
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i);
				tempString = txt.text;
				doneWithDots = false;
				dotsAmount = 1;
				StartCoroutine(DotsAppear());
				yield return new WaitUntil(() => doneWithDots);
				break;
			case '½':
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				playSound = false;
				waitTime = 0.75f;
				txt.text = fullString.Substring(0, i);
				break;
			case ' ':
				waitTime = 0f;
				txt.text = fullString.Substring(0, i);
				break;
			case '~':
				waitTime = 0f;
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i);
				waitingForInput = true;
				yield return new WaitUntil(() => !waitingForInput);
				break;
			case '@':
			{
				waitTime = 0f;
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i);
				GameObject[] array = activateOnTextTrigger;
				for (int k = 0; k < array.Length; k++)
				{
					array[k].SetActive(value: true);
				}
				break;
			}
			case '&':
				GetComponentInParent<IntroTextController>().introOver = true;
				GameProgressSaver.SetIntro(beat: true);
				waitTime = 0f;
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i);
				break;
			case 'Ä':
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				txt.text = fullString.Substring(0, i) + "<color=red>ERROR</color>";
				yield return new WaitForSecondsRealtime(1f);
				calibrationWindows[calibrated].SetActive(value: true);
				readyToContinue = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				yield return new WaitUntil(() => readyToContinue);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				tempString = "<color=lime>OK</color>";
				fullString = fullString.Insert(i, tempString);
				i += tempString.Length;
				_ = j + tempString.Length;
				txt.text = fullString.Substring(0, i);
				break;
			case '*':
				colorString = "<color=red>";
				PlaceColor(i);
				i += colorString.Length;
				_ = j + colorString.Length;
				txt.text = fullString.Substring(0, i);
				break;
			case '+':
				colorString = "<color=lime>";
				PlaceColor(i);
				i += colorString.Length;
				_ = j + colorString.Length;
				txt.text = fullString.Substring(0, i);
				break;
			case '^':
				colorString = "<color=grey>";
				PlaceColor(i);
				i += colorString.Length;
				_ = j + colorString.Length;
				txt.text = fullString.Substring(0, i);
				break;
			case '_':
			{
				colorsClosePositions.Add(i);
				sb = new StringBuilder(fullString);
				sb[i] = ' ';
				fullString = sb.ToString();
				string text = "</color>";
				fullString = fullString.Insert(i, text);
				i += text.Length;
				_ = j + text.Length;
				txt.text = fullString.Substring(0, i);
				break;
			}
			default:
				txt.text = fullString.Substring(0, i);
				break;
			}
			j = fullString.Length;
			if (waitTime != 0f && playSound)
			{
				aud.Play();
			}
			if (colorsPositions.Count > colorsClosePositions.Count)
			{
				txt.text += "</color>";
			}
			yield return new WaitForSecondsRealtime(waitTime);
		}
		Over();
	}

	private void PlaceColor(int i)
	{
		colorsPositions.Add(i);
		sb = new StringBuilder(fullString);
		sb[i] = ' ';
		fullString = sb.ToString();
		fullString = fullString.Insert(i, colorString);
	}

	private void Over()
	{
		GameObject[] array = activateOnEnd;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		array = deactivateOnEnd;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}

	private IEnumerator DotsAppear()
	{
		for (int i = 0; i < dotsAmount; i++)
		{
			txt.text = tempString;
			aud.Play();
			yield return new WaitForSecondsRealtime(0.25f);
			txt.text = tempString + ".";
			aud.Play();
			yield return new WaitForSecondsRealtime(0.25f);
			txt.text = tempString + "..";
			aud.Play();
			yield return new WaitForSecondsRealtime(0.25f);
			txt.text = tempString + "...";
			aud.Play();
			yield return new WaitForSecondsRealtime(0.25f);
		}
		doneWithDots = true;
	}
}
