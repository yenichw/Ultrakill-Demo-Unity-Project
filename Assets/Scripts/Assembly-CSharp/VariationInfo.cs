using UnityEngine;
using UnityEngine.UI;

public class VariationInfo : MonoBehaviour
{
	public GameObject varPage;

	private int money;

	public Text moneyText;

	public int cost;

	public Text costText;

	public ShopButton buyButton;

	private Text buttonText;

	public GameObject buySound;

	public Button equipButton;

	private GameObject equipImage;

	public bool alreadyOwned;

	public string weaponName;

	private GunSetter gs;

	private FistControl fc;

	private void Start()
	{
		buttonText = buyButton.GetComponentInChildren<Text>();
		buyButton.variationInfo = this;
		equipImage = equipButton.transform.GetChild(1).gameObject;
		if (GameProgressSaver.CheckGear(weaponName) > 0)
		{
			alreadyOwned = true;
		}
		UpdateMoney();
	}

	private void OnEnable()
	{
		UpdateMoney();
	}

	public void UpdateMoney()
	{
		money = GameProgressSaver.GetMoney();
		moneyText.text = DivideMoney(money) + "<color=orange>P</color>";
		if (!alreadyOwned)
		{
			if (cost > money)
			{
				costText.text = "<color=red>" + DivideMoney(cost) + "P</color>";
				if (buttonText == null)
				{
					buttonText = buyButton.GetComponentInChildren<Text>();
				}
				buttonText.text = costText.text;
				buyButton.failure = true;
				buyButton.GetComponent<Button>().interactable = false;
				buyButton.GetComponent<Image>().color = Color.red;
			}
			else
			{
				costText.text = "<color=white>" + DivideMoney(cost) + "</color><color=orange>P</color>";
				if (buttonText == null)
				{
					buttonText = buyButton.GetComponentInChildren<Text>();
				}
				buttonText.text = costText.text;
				buyButton.failure = false;
				buyButton.GetComponent<Button>().interactable = true;
				buyButton.GetComponent<Image>().color = Color.white;
			}
			equipButton.gameObject.SetActive(value: false);
		}
		else
		{
			costText.text = "ALREADY OWNED";
			if (buttonText == null)
			{
				buttonText = buyButton.GetComponentInChildren<Text>();
			}
			buttonText.text = costText.text;
			buyButton.failure = true;
			buyButton.GetComponent<Button>().interactable = false;
			buyButton.GetComponent<Image>().color = Color.white;
			equipButton.gameObject.SetActive(value: true);
			equipButton.interactable = true;
			if (equipImage == null)
			{
				equipImage = equipButton.transform.GetChild(1).gameObject;
			}
			if (PlayerPrefs.GetInt(weaponName, 0) > 0)
			{
				equipImage.SetActive(value: true);
			}
		}
	}

	public void WeaponBought()
	{
		alreadyOwned = true;
		Object.Instantiate(buySound);
		GameProgressSaver.AddMoney(cost * -1);
		GameProgressSaver.AddGear(weaponName);
		PlayerPrefs.SetInt(weaponName, 1);
		base.gameObject.GetComponentInParent<ShopZone>().UpdateMoney();
		if (PlayerPrefs.GetInt("FirVar", 1) == 1)
		{
			GetComponentInParent<ShopZone>().firstVariationBuy = true;
		}
		if (gs == null)
		{
			gs = GameObject.FindWithTag("Player").GetComponentInChildren<GunSetter>();
		}
		gs.ResetWeapons();
		gs.ForceWeapon(weaponName);
		gs.gunc.NoWeapon();
		if (fc == null)
		{
			fc = GameObject.FindWithTag("Player").GetComponentInChildren<FistControl>();
		}
		fc.ResetFists();
	}

	public void EquipWeapon()
	{
		if (PlayerPrefs.GetInt(weaponName, 0) > 0)
		{
			equipImage.SetActive(value: false);
			PlayerPrefs.SetInt(weaponName, 0);
		}
		else
		{
			equipImage.SetActive(value: true);
			PlayerPrefs.SetInt(weaponName, 1);
		}
		if (gs == null)
		{
			gs = GameObject.FindWithTag("Player").GetComponentInChildren<GunSetter>();
		}
		gs.ResetWeapons();
		if (fc == null)
		{
			fc = GameObject.FindWithTag("Player").GetComponentInChildren<FistControl>();
		}
		fc.ResetFists();
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
