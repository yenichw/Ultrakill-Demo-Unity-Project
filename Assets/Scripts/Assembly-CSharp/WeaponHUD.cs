using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
	public bool mainHud;

	public Color[] colors;

	private Image img;

	public int currentWeapon;

	public int currentVariation;

	public Sprite armSprite;

	public Sprite[] revolverSprites;

	public Sprite[] shotgunSprites;

	public Sprite[] nailgunSprites;

	public Sprite[] railcannonSprites;

	public Sprite blackHoleSprite;

	private void Awake()
	{
		if (img == null)
		{
			img = GetComponent<Image>();
		}
	}

	public void UpdateImage(int weapon, int variation)
	{
		if (img == null)
		{
			img = GetComponent<Image>();
		}
		switch (weapon)
		{
		case 0:
			img.sprite = armSprite;
			break;
		case 1:
			img.sprite = revolverSprites[variation];
			break;
		case 2:
			img.sprite = shotgunSprites[variation];
			break;
		case 3:
			img.sprite = nailgunSprites[variation];
			break;
		case 4:
			img.sprite = railcannonSprites[variation];
			break;
		}
		img.color = colors[variation];
		currentWeapon = weapon;
		currentVariation = variation;
		if (mainHud)
		{
			base.transform.GetChild(0).GetComponent<WeaponHUD>().UpdateImage(currentWeapon, currentVariation);
		}
	}
}
