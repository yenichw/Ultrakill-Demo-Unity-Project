using UnityEngine;
using UnityEngine.UI;

public class CopyImage : MonoBehaviour
{
	private Image img;

	public Image imgToCopy;

	public CopyType copyType;

	public bool copyColor;

	private void Update()
	{
		if (img == null)
		{
			img = GetComponent<Image>();
		}
		if (!(img != null))
		{
			return;
		}
		if (imgToCopy == null && copyType != 0)
		{
			WeaponHUD[] array = Object.FindObjectsOfType<WeaponHUD>();
			WeaponHUD weaponHUD = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].mainHud)
				{
					weaponHUD = array[i];
					break;
				}
			}
			if (weaponHUD != null)
			{
				if (copyType == CopyType.WeaponIcon)
				{
					imgToCopy = weaponHUD.GetComponent<Image>();
				}
				else if (copyType == CopyType.WeaponShadow)
				{
					imgToCopy = weaponHUD.transform.GetChild(0).GetComponent<Image>();
				}
			}
		}
		if (imgToCopy != null)
		{
			img.sprite = imgToCopy.sprite;
			if (copyColor)
			{
				img.color = imgToCopy.color;
			}
		}
	}
}
