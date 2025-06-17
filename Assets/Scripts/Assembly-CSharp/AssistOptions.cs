using UnityEngine;
using UnityEngine.UI;

public class AssistOptions : MonoBehaviour
{
	public Toggle majorEnable;

	public Slider gameSpeed;

	public Slider damageTaken;

	public Toggle infiniteStamina;

	public Toggle disableHardDamage;

	public GameObject majorPopup;

	public GameObject majorBlocker;

	private AssistController asscon;

	private void Start()
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		if (PlayerPrefs.GetInt("MajAss", 0) == 1)
		{
			majorEnable.isOn = true;
			majorBlocker.SetActive(value: false);
		}
		else
		{
			majorEnable.isOn = false;
			majorBlocker.SetActive(value: true);
		}
		gameSpeed.value = PlayerPrefs.GetFloat("GamSpe", 100f);
		damageTaken.value = PlayerPrefs.GetFloat("DamTak", 100f);
		if (PlayerPrefs.GetInt("InfSta", 0) == 1)
		{
			infiniteStamina.isOn = true;
		}
		else
		{
			infiniteStamina.isOn = false;
		}
		if (PlayerPrefs.GetInt("DisHarDam", 0) == 1)
		{
			disableHardDamage.isOn = true;
		}
		else
		{
			disableHardDamage.isOn = false;
		}
	}

	public void MajorCheck()
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		if (PlayerPrefs.GetInt("MajAss", 0) == 0)
		{
			majorPopup.SetActive(value: true);
			return;
		}
		PlayerPrefs.SetInt("MajAss", 0);
		asscon.majorEnabled = false;
		majorEnable.isOn = false;
		majorBlocker.SetActive(value: true);
	}

	public void MajorEnable()
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		PlayerPrefs.SetInt("MajAss", 1);
		asscon.MajorEnabled();
		majorEnable.isOn = true;
		majorBlocker.SetActive(value: false);
	}

	public void GameSpeed(float stuff)
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		PlayerPrefs.SetFloat("GamSpe", stuff);
		asscon.gameSpeed = stuff / 100f;
	}

	public void DamageTaken(float stuff)
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		PlayerPrefs.SetFloat("DamTak", stuff);
		asscon.damageTaken = stuff / 100f;
	}

	public void InfiniteStamina(bool stuff)
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("InfSta", 1);
		}
		else
		{
			PlayerPrefs.SetInt("InfSta", 0);
		}
		asscon.infiniteStamina = stuff;
	}

	public void DisableHardDamage(bool stuff)
	{
		if (asscon == null)
		{
			asscon = Object.FindObjectOfType<AssistController>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("DisHarDam", 1);
		}
		else
		{
			PlayerPrefs.SetInt("DisHarDam", 0);
		}
		asscon.disableHardDamage = stuff;
	}
}
