using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
	public bool altHud;

	public bool colorless;

	private GameObject altHudObj;

	private GameObject gunCanvas;

	private HUDPos hudpos;

	public GameObject weaponIcon;

	public GameObject armIcon;

	public GameObject styleMeter;

	public GameObject styleInfo;

	public Image[] hudBackgrounds;

	private void Start()
	{
		CheckSituation();
		if (PlayerPrefs.GetInt("WeaIco", 1) == 0)
		{
			if (!altHud)
			{
				weaponIcon.transform.localPosition = new Vector3(weaponIcon.transform.localPosition.x, weaponIcon.transform.localPosition.y, 45f);
			}
			else
			{
				weaponIcon.SetActive(value: false);
			}
		}
		if (PlayerPrefs.GetInt("ArmIco", 1) == 0)
		{
			if (!altHud)
			{
				armIcon.transform.localPosition = new Vector3(armIcon.transform.localPosition.x, armIcon.transform.localPosition.y, 0f);
			}
			else
			{
				armIcon.SetActive(value: false);
			}
		}
		if (!altHud)
		{
			if (PlayerPrefs.GetInt("StyMet", 1) == 0)
			{
				styleMeter.transform.localPosition = new Vector3(styleMeter.transform.localPosition.x, styleMeter.transform.localPosition.y, -9999f);
			}
			if (PlayerPrefs.GetInt("StyInf", 1) == 0)
			{
				styleInfo.transform.localPosition = new Vector3(styleInfo.transform.localPosition.x, styleInfo.transform.localPosition.y, -9999f);
				base.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = false;
			}
		}
		float @float = PlayerPrefs.GetFloat("BgOpa", 50f);
		if (@float != 50f)
		{
			SetOpacity(@float);
		}
	}

	public void CheckSituation()
	{
		if (altHud)
		{
			if (altHudObj == null)
			{
				altHudObj = base.transform.GetChild(0).gameObject;
			}
			if ((bool)altHudObj)
			{
				if (PlayerPrefs.GetInt("AltHud", 1) == 2 && !colorless)
				{
					altHudObj.SetActive(value: true);
				}
				else if (PlayerPrefs.GetInt("AltHud", 1) == 3 && colorless)
				{
					altHudObj.SetActive(value: true);
				}
				else
				{
					altHudObj.SetActive(value: false);
				}
			}
			return;
		}
		if (PlayerPrefs.GetInt("AltHud", 1) != 1)
		{
			if (gunCanvas == null)
			{
				gunCanvas = base.transform.Find("GunCanvas").gameObject;
			}
			if (hudpos == null)
			{
				hudpos = gunCanvas.GetComponent<HUDPos>();
			}
			gunCanvas.transform.localPosition = new Vector3(gunCanvas.transform.localPosition.x, gunCanvas.transform.localPosition.y, -100f);
			if ((bool)hudpos)
			{
				hudpos.active = false;
			}
			return;
		}
		if (gunCanvas == null)
		{
			gunCanvas = base.transform.Find("GunCanvas").gameObject;
		}
		if (hudpos == null)
		{
			hudpos = gunCanvas.GetComponent<HUDPos>();
		}
		gunCanvas.transform.localPosition = new Vector3(gunCanvas.transform.localPosition.x, gunCanvas.transform.localPosition.y, 1f);
		if ((bool)hudpos)
		{
			hudpos.active = true;
			hudpos.CheckPos();
		}
	}

	public void SetOpacity(float amount)
	{
		Image[] array = hudBackgrounds;
		foreach (Image obj in array)
		{
			Color color = obj.color;
			color.a = amount / 100f;
			obj.color = color;
		}
	}
}
