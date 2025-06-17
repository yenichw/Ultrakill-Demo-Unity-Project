using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsOptions : MonoBehaviour
{
	private InputManager inman;

	public OptionsManager opm;

	private List<Text> allTexts = new List<Text>();

	public Text wText;

	public Text sText;

	public Text aText;

	public Text dText;

	public Text jumpText;

	public Text dodgeText;

	public Text slideText;

	public Text fire1Text;

	public Text fire2Text;

	public Text punchText;

	public Text lastUsedWeaponText;

	public Text changeVariation;

	public Toggle scrollWheel;

	public GameObject scrollSettings;

	public Dropdown variationWheel;

	public Toggle reverseWheel;

	private GameObject currentKey;

	public Color normalColor;

	public Color pressedColor;

	private bool canUnpause;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		opm = Object.FindObjectOfType<OptionsManager>();
		wText.text = ((KeyCode)PlayerPrefs.GetInt("KeyW", 119)).ToString();
		sText.text = ((KeyCode)PlayerPrefs.GetInt("KeyS", 115)).ToString();
		aText.text = ((KeyCode)PlayerPrefs.GetInt("KeyA", 97)).ToString();
		dText.text = ((KeyCode)PlayerPrefs.GetInt("KeyD", 100)).ToString();
		jumpText.text = ((KeyCode)PlayerPrefs.GetInt("KeyJump", 32)).ToString();
		dodgeText.text = ((KeyCode)PlayerPrefs.GetInt("KeyDodge", 304)).ToString();
		slideText.text = ((KeyCode)PlayerPrefs.GetInt("KeySlide", 306)).ToString();
		fire1Text.text = ((KeyCode)PlayerPrefs.GetInt("KeyFire1", 323)).ToString();
		fire2Text.text = ((KeyCode)PlayerPrefs.GetInt("KeyFire2", 324)).ToString();
		punchText.text = ((KeyCode)PlayerPrefs.GetInt("KeyPunch", 102)).ToString();
		lastUsedWeaponText.text = ((KeyCode)PlayerPrefs.GetInt("KeyLastUsedWeapon", 113)).ToString();
		changeVariation.text = ((KeyCode)PlayerPrefs.GetInt("KeyChangeVariation", 101)).ToString();
		allTexts.Add(wText);
		allTexts.Add(sText);
		allTexts.Add(aText);
		allTexts.Add(dText);
		allTexts.Add(jumpText);
		allTexts.Add(dodgeText);
		allTexts.Add(slideText);
		allTexts.Add(fire1Text);
		allTexts.Add(fire2Text);
		allTexts.Add(punchText);
		allTexts.Add(lastUsedWeaponText);
		allTexts.Add(changeVariation);
		if (PlayerPrefs.GetInt("ScrOn", 1) == 0)
		{
			scrollWheel.isOn = false;
			scrollSettings.SetActive(value: false);
		}
		int @int = PlayerPrefs.GetInt("ScrVar", 0);
		int int2 = PlayerPrefs.GetInt("ScrWep", 1);
		if (@int == 1 && int2 == 1)
		{
			variationWheel.value = 2;
		}
		else if (@int == 1 && int2 == 0)
		{
			variationWheel.value = 1;
		}
		else
		{
			variationWheel.value = 0;
		}
		if (PlayerPrefs.GetInt("ScrRev", 0) == 1)
		{
			reverseWheel.isOn = true;
		}
	}

	private void OnDisable()
	{
		if (currentKey != null)
		{
			if (opm == null)
			{
				opm = Object.FindObjectOfType<OptionsManager>();
			}
			currentKey.GetComponent<Image>().color = normalColor;
			currentKey = null;
			opm.dontUnpause = false;
		}
	}

	private void LateUpdate()
	{
		if (canUnpause)
		{
			if (opm == null)
			{
				opm = Object.FindObjectOfType<OptionsManager>();
			}
			canUnpause = false;
			opm.dontUnpause = false;
		}
	}

	private void OnGUI()
	{
		if (!(currentKey != null))
		{
			return;
		}
		Event current = Event.current;
		if (current.keyCode == KeyCode.Escape)
		{
			currentKey.GetComponent<Image>().color = normalColor;
			currentKey = null;
			canUnpause = true;
		}
		if (current.isKey || current.isMouse || current.button > 2 || current.shift)
		{
			KeyCode keyCode = KeyCode.Alpha0;
			if (current.isKey)
			{
				keyCode = current.keyCode;
			}
			else if (Input.GetKey(KeyCode.LeftShift))
			{
				keyCode = KeyCode.LeftShift;
			}
			else if (Input.GetKey(KeyCode.RightShift))
			{
				keyCode = KeyCode.RightShift;
			}
			else
			{
				if (current.button > 6)
				{
					currentKey.GetComponent<Image>().color = normalColor;
					currentKey = null;
					canUnpause = true;
					return;
				}
				keyCode = (KeyCode)(323 + current.button);
			}
			if (inman == null)
			{
				inman = Object.FindObjectOfType<InputManager>();
			}
			inman.Inputs[currentKey.name] = keyCode;
			currentKey.GetComponentInChildren<Text>().text = keyCode.ToString();
			PlayerPrefs.SetInt("Key" + currentKey.name, (int)keyCode);
			currentKey.GetComponent<Image>().color = normalColor;
			currentKey = null;
			canUnpause = true;
		}
		else if (Input.GetKey(KeyCode.Mouse3) || Input.GetKey(KeyCode.Mouse4) || Input.GetKey(KeyCode.Mouse5) || Input.GetKey(KeyCode.Mouse6))
		{
			KeyCode value = KeyCode.Mouse3;
			if (Input.GetKey(KeyCode.Mouse4))
			{
				value = KeyCode.Mouse4;
			}
			else if (Input.GetKey(KeyCode.Mouse5))
			{
				value = KeyCode.Mouse5;
			}
			else if (Input.GetKey(KeyCode.Mouse6))
			{
				value = KeyCode.Mouse6;
			}
			if (inman == null)
			{
				inman = Object.FindObjectOfType<InputManager>();
			}
			inman.Inputs[currentKey.name] = value;
			currentKey.GetComponentInChildren<Text>().text = value.ToString();
			PlayerPrefs.SetInt("Key" + currentKey.name, (int)value);
			currentKey.GetComponent<Image>().color = normalColor;
			currentKey = null;
			canUnpause = true;
		}
		else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			KeyCode value2 = KeyCode.LeftShift;
			if (Input.GetKey(KeyCode.RightShift))
			{
				value2 = KeyCode.RightShift;
			}
			inman.Inputs[currentKey.name] = value2;
			currentKey.GetComponentInChildren<Text>().text = value2.ToString();
			PlayerPrefs.SetInt("Key" + currentKey.name, (int)value2);
			currentKey.GetComponent<Image>().color = normalColor;
			currentKey = null;
			canUnpause = true;
		}
	}

	public void ChangeKey(GameObject stuff)
	{
		if (opm == null)
		{
			opm = Object.FindObjectOfType<OptionsManager>();
		}
		opm.dontUnpause = true;
		currentKey = stuff;
		stuff.GetComponent<Image>().color = pressedColor;
	}

	public void ResetKeys()
	{
		if (inman == null)
		{
			inman = Object.FindObjectOfType<InputManager>();
		}
		for (int i = 0; i < allTexts.Count; i++)
		{
			switch (i)
			{
			case 0:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.W;
				break;
			case 1:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.S;
				break;
			case 2:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.A;
				break;
			case 3:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.D;
				break;
			case 4:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.Space;
				break;
			case 5:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.LeftShift;
				break;
			case 6:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.LeftControl;
				break;
			case 7:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.Mouse0;
				break;
			case 8:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.Mouse1;
				break;
			case 9:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.F;
				break;
			case 10:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.Q;
				break;
			case 11:
				inman.Inputs[allTexts[i].transform.parent.name] = KeyCode.E;
				break;
			}
			PlayerPrefs.SetInt("Key" + allTexts[i].transform.parent.name, (int)inman.Inputs[allTexts[i].transform.parent.name]);
			allTexts[i].text = ((KeyCode)PlayerPrefs.GetInt("Key" + allTexts[i].transform.parent.name)).ToString();
		}
	}

	public void ScrollOn(bool stuff)
	{
		if (inman == null)
		{
			inman = Object.FindObjectOfType<InputManager>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("ScrOn", 1);
			inman.ScrOn = true;
			scrollSettings.SetActive(value: true);
		}
		else
		{
			PlayerPrefs.SetInt("ScrOn", 0);
			inman.ScrOn = false;
			scrollSettings.SetActive(value: false);
		}
	}

	public void ScrollVariations(int stuff)
	{
		if (inman == null)
		{
			inman = Object.FindObjectOfType<InputManager>();
		}
		switch (stuff)
		{
		case 0:
			PlayerPrefs.SetInt("ScrWep", 1);
			PlayerPrefs.SetInt("ScrVar", 0);
			inman.ScrWep = true;
			inman.ScrVar = false;
			break;
		case 1:
			PlayerPrefs.SetInt("ScrWep", 0);
			PlayerPrefs.SetInt("ScrVar", 1);
			inman.ScrWep = false;
			inman.ScrVar = true;
			break;
		default:
			PlayerPrefs.SetInt("ScrWep", 1);
			PlayerPrefs.SetInt("ScrVar", 1);
			inman.ScrWep = true;
			inman.ScrVar = true;
			break;
		}
	}

	public void ScrollReverse(bool stuff)
	{
		if (inman == null)
		{
			inman = Object.FindObjectOfType<InputManager>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("ScrRev", 1);
			inman.ScrRev = true;
		}
		else
		{
			PlayerPrefs.SetInt("ScrRev", 0);
			inman.ScrRev = false;
		}
	}
}
