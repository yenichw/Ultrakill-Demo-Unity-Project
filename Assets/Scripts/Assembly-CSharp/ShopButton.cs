using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public bool failure;

	public GameObject clickSound;

	public GameObject failSound;

	public GameObject[] toActivate;

	public GameObject[] toDeactivate;

	public VariationInfo variationInfo;

	public void OnPointerClick(PointerEventData eventData)
	{
		GameObject[] array = toActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		array = toDeactivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		if (!failure)
		{
			if (variationInfo != null)
			{
				variationInfo.WeaponBought();
			}
			if (clickSound != null)
			{
				Object.Instantiate(clickSound);
			}
		}
		else if (failure && failSound != null)
		{
			Object.Instantiate(failSound);
		}
	}
}
