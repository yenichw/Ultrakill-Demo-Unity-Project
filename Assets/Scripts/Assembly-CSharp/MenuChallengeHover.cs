using UnityEngine;
using UnityEngine.EventSystems;

public class MenuChallengeHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		base.transform.parent.GetChild(1).gameObject.SetActive(value: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		base.transform.parent.GetChild(1).gameObject.SetActive(value: false);
	}
}
