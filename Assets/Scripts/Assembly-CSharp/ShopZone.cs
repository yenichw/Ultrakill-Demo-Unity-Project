using UnityEngine;
using UnityEngine.UI;

public class ShopZone : MonoBehaviour
{
	private GunControl gc;

	private bool inUse;

	private Canvas shopCanvas;

	private FistControl pun;

	private AudioSource music;

	public Text moneyText;

	public bool firstVariationBuy;

	public GameObject variationSwitchTip;

	private ShopMother shom;

	private ShopCategory[] shopcats;

	private void Start()
	{
		shopCanvas = GetComponentInChildren<Canvas>();
		if (shopCanvas != null)
		{
			shopCanvas.gameObject.SetActive(value: false);
		}
		music = base.transform.Find("Music").GetComponent<AudioSource>();
		if (moneyText != null)
		{
			moneyText.text = DivideMoney(GameProgressSaver.GetMoney()) + "<color=orange>P</color>";
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.NoWeapon();
			if (pun == null)
			{
				pun = other.GetComponentInChildren<FistControl>();
			}
			pun.ShopMode();
			inUse = true;
			shopCanvas.gameObject.SetActive(value: true);
			if (shopcats == null)
			{
				shopcats = GetComponentsInChildren<ShopCategory>();
			}
			ShopCategory[] array = shopcats;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CheckGear();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.YesWeapon();
			inUse = false;
			pun.StopShop();
			shopCanvas.gameObject.SetActive(value: false);
			if (firstVariationBuy)
			{
				HudMessager.SendHudMessage("Cycle through <color=orange>EQUIPPED</color> variations with '<color=orange>", "ChangeVariation", "</color>'.");
				firstVariationBuy = false;
				PlayerPrefs.SetInt("FirVar", 0);
			}
		}
	}

	private void Update()
	{
		if (!(music != null))
		{
			return;
		}
		if (inUse)
		{
			if (music.pitch < 1f)
			{
				music.pitch = Mathf.MoveTowards(music.pitch, 1f, Time.deltaTime);
			}
		}
		else if (music.pitch > 0f)
		{
			music.pitch = Mathf.MoveTowards(music.pitch, 0f, Time.deltaTime);
		}
	}

	public void ForceOff()
	{
		if (inUse)
		{
			shopCanvas.gameObject.SetActive(value: false);
		}
	}

	public void UpdateMoney()
	{
		moneyText.text = DivideMoney(GameProgressSaver.GetMoney()) + "<color=orange>P</color>";
		VariationInfo[] componentsInChildren = GetComponentsInChildren<VariationInfo>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].UpdateMoney();
		}
	}

	public string DivideMoney(int dosh)
	{
		int num = dosh;
		int num2 = 0;
		int num3 = 0;
		while (num >= 1000)
		{
			num2++;
			num -= 1000;
		}
		while (num2 >= 1000)
		{
			num3++;
			num2 -= 1000;
		}
		if (num3 > 0)
		{
			string text = num3 + ",";
			text = ((num2 < 10) ? (text + "00" + num2 + ",") : ((num2 >= 100) ? (text + num2 + ",") : (text + "0" + num2 + ",")));
			if (num < 10)
			{
				return text + "00" + num;
			}
			if (num < 100)
			{
				return text + "0" + num;
			}
			return text + num;
		}
		if (num2 > 0)
		{
			string text = num2 + ",";
			if (num < 10)
			{
				return text + "00" + num;
			}
			if (num < 100)
			{
				return text + "0" + num;
			}
			return text + num;
		}
		return string.Concat(num);
	}
}
