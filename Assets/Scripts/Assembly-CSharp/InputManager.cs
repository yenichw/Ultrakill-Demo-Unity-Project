using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public Dictionary<string, KeyCode> inputsDictionary = new Dictionary<string, KeyCode>();

	public bool ScrOn;

	public bool ScrWep;

	public bool ScrVar;

	public bool ScrRev;

	public Dictionary<string, KeyCode> Inputs
	{
		get
		{
			CheckInputs();
			return inputsDictionary;
		}
	}

	private void Awake()
	{
		if (PlayerPrefs.GetInt("ScrOn", 1) == 1)
		{
			ScrOn = true;
		}
		else
		{
			ScrOn = false;
		}
		if (PlayerPrefs.GetInt("ScrWep", 1) == 1)
		{
			ScrWep = true;
		}
		else
		{
			ScrWep = false;
		}
		if (PlayerPrefs.GetInt("ScrVar", 0) == 1)
		{
			ScrVar = true;
		}
		else
		{
			ScrVar = false;
		}
		if (PlayerPrefs.GetInt("ScrRev", 0) == 1)
		{
			ScrRev = true;
		}
		else
		{
			ScrRev = false;
		}
		CheckInputs();
	}

	public void CheckInputs()
	{
		if (inputsDictionary == null || inputsDictionary.Count == 0)
		{
			inputsDictionary.Add("W", (KeyCode)PlayerPrefs.GetInt("KeyW", 119));
			inputsDictionary.Add("S", (KeyCode)PlayerPrefs.GetInt("KeyS", 115));
			inputsDictionary.Add("A", (KeyCode)PlayerPrefs.GetInt("KeyA", 97));
			inputsDictionary.Add("D", (KeyCode)PlayerPrefs.GetInt("KeyD", 100));
			inputsDictionary.Add("Jump", (KeyCode)PlayerPrefs.GetInt("KeyJump", 32));
			inputsDictionary.Add("Dodge", (KeyCode)PlayerPrefs.GetInt("KeyDodge", 304));
			inputsDictionary.Add("Slide", (KeyCode)PlayerPrefs.GetInt("KeySlide", 306));
			inputsDictionary.Add("Fire1", (KeyCode)PlayerPrefs.GetInt("KeyFire1", 323));
			inputsDictionary.Add("Fire2", (KeyCode)PlayerPrefs.GetInt("KeyFire2", 324));
			inputsDictionary.Add("Punch", (KeyCode)PlayerPrefs.GetInt("KeyPunch", 102));
			inputsDictionary.Add("LastUsedWeapon", (KeyCode)PlayerPrefs.GetInt("KeyLastUsedWeapon", 113));
			inputsDictionary.Add("ChangeVariation", (KeyCode)PlayerPrefs.GetInt("KeyChangeVariation", 101));
			inputsDictionary.Add("ChangeFist", (KeyCode)PlayerPrefs.GetInt("KeyChangeFist", 103));
		}
	}
}
