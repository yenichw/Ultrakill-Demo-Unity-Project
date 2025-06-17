using UnityEngine;
using UnityEngine.UI;

public class HUDOptions : MonoBehaviour
{
	public Dropdown hudType;

	private HudController[] hudCons;

	private HudController mainCon;

	public Slider bgOpacity;

	public Toggle weaponIcon;

	public Toggle armIcon;

	public Toggle styleMeter;

	public Toggle styleInfo;

	private Crosshair crosshair;

	public Dropdown crossHairType;

	public Dropdown crossHairColor;

	public Dropdown crossHairHud;

	public Toggle crossHairHudFade;

	public Toggle toodeeScreens;

	private void Start()
	{
		crosshair = GetComponentInChildren<Crosshair>();
		crossHairType.value = PlayerPrefs.GetInt("CroHai", 1);
		crossHairType.RefreshShownValue();
		crossHairColor.value = PlayerPrefs.GetInt("CroCol", 1);
		crossHairColor.RefreshShownValue();
		crossHairHud.value = PlayerPrefs.GetInt("Chud", 2);
		crossHairHud.RefreshShownValue();
		hudType.value = PlayerPrefs.GetInt("AltHud", 1);
		hudType.RefreshShownValue();
		bgOpacity.value = PlayerPrefs.GetFloat("BgOpa", 50f);
		if (PlayerPrefs.GetInt("WeaIco", 1) == 0)
		{
			weaponIcon.isOn = false;
		}
		if (PlayerPrefs.GetInt("ArmIco", 1) == 0)
		{
			armIcon.isOn = false;
		}
		if (PlayerPrefs.GetInt("StyMet", 1) == 0)
		{
			styleMeter.isOn = false;
		}
		if (PlayerPrefs.GetInt("StyInf", 1) == 0)
		{
			styleInfo.isOn = false;
		}
		if (PlayerPrefs.GetInt("BarFad", 1) == 0)
		{
			crossHairHudFade.isOn = false;
		}
		if (PlayerPrefs.GetInt("TooDee", 0) == 1)
		{
			toodeeScreens.isOn = true;
		}
		else
		{
			toodeeScreens.isOn = false;
		}
	}

	public void CrossHairType(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("CroHai", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void CrossHairColor(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("CroCol", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void CrossHairHud(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("Chud", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void HudType(int stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		PlayerPrefs.SetInt("AltHud", stuff);
		HudController[] array = hudCons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CheckSituation();
		}
		GetComponent<OptionsMenuToManager>().CheckEasterEgg();
	}

	public void HudFade(bool stuff)
	{
		if (stuff)
		{
			PlayerPrefs.SetInt("BarFad", 1);
		}
		else
		{
			PlayerPrefs.SetInt("BarFad", 0);
		}
		FadeOutBars[] array = Object.FindObjectsOfType<FadeOutBars>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CheckState();
		}
	}

	public void BgOpacity(float stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		PlayerPrefs.SetFloat("BgOpa", stuff);
		HudController[] array = hudCons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOpacity(stuff);
		}
	}

	public void WeaponIcon(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		HudController[] array;
		if (stuff)
		{
			PlayerPrefs.SetInt("WeaIco", 1);
			array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.weaponIcon.transform.localPosition = new Vector3(hudController.weaponIcon.transform.localPosition.x, hudController.weaponIcon.transform.localPosition.y, 45f);
				}
				else
				{
					hudController.weaponIcon.SetActive(value: true);
				}
			}
			return;
		}
		PlayerPrefs.SetInt("WeaIco", 0);
		array = hudCons;
		foreach (HudController hudController2 in array)
		{
			if (!hudController2.altHud)
			{
				hudController2.weaponIcon.transform.localPosition = new Vector3(hudController2.weaponIcon.transform.localPosition.x, hudController2.weaponIcon.transform.localPosition.y, -9999f);
			}
			else
			{
				hudController2.weaponIcon.SetActive(value: false);
			}
		}
	}

	public void ArmIcon(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		HudController[] array;
		if (stuff)
		{
			PlayerPrefs.SetInt("ArmIco", 1);
			array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.armIcon.transform.localPosition = new Vector3(hudController.armIcon.transform.localPosition.x, hudController.armIcon.transform.localPosition.y, 0f);
				}
				else
				{
					hudController.armIcon.SetActive(value: true);
				}
			}
			return;
		}
		PlayerPrefs.SetInt("ArmIco", 0);
		array = hudCons;
		foreach (HudController hudController2 in array)
		{
			if (!hudController2.altHud)
			{
				hudController2.armIcon.transform.localPosition = new Vector3(hudController2.armIcon.transform.localPosition.x, hudController2.armIcon.transform.localPosition.y, -9999f);
			}
			else
			{
				hudController2.armIcon.SetActive(value: false);
			}
		}
	}

	public void StyleMeter(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		HudController[] array;
		if (stuff)
		{
			PlayerPrefs.SetInt("StyMet", 1);
			array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.styleMeter.transform.localPosition = new Vector3(hudController.styleMeter.transform.localPosition.x, hudController.styleMeter.transform.localPosition.y, 0f);
				}
			}
			return;
		}
		PlayerPrefs.SetInt("StyMet", 0);
		array = hudCons;
		foreach (HudController hudController2 in array)
		{
			if (!hudController2.altHud)
			{
				hudController2.styleMeter.transform.localPosition = new Vector3(hudController2.styleMeter.transform.localPosition.x, hudController2.styleMeter.transform.localPosition.y, -9999f);
			}
		}
	}

	public void StyleInfo(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 4)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		HudController[] array;
		if (stuff)
		{
			PlayerPrefs.SetInt("StyInf", 1);
			array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.styleInfo.transform.localPosition = new Vector3(hudController.styleInfo.transform.localPosition.x, hudController.styleInfo.transform.localPosition.y, 0f);
					hudController.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = true;
				}
			}
			return;
		}
		PlayerPrefs.SetInt("StyInf", 0);
		array = hudCons;
		foreach (HudController hudController2 in array)
		{
			if (!hudController2.altHud)
			{
				hudController2.styleInfo.transform.localPosition = new Vector3(hudController2.styleInfo.transform.localPosition.x, hudController2.styleInfo.transform.localPosition.y, -9999f);
				hudController2.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = false;
			}
		}
	}

	public void TooDeeScreens(bool stuff)
	{
	}
}
