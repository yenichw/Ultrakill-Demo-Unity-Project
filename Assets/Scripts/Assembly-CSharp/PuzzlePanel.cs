using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler
{
	[HideInInspector]
	public TileType tileType;

	[HideInInspector]
	public TileColor tileColor;

	[HideInInspector]
	public GameObject currentPanel;

	[HideInInspector]
	public GameObject whiteSquare;

	[HideInInspector]
	public GameObject blackSquare;

	[HideInInspector]
	public GameObject fillSquare;

	public GameObject pitSquare;

	private Image img;

	private bool activated;

	private PuzzleController pc;

	[HideInInspector]
	public PuzzleLine pl;

	private void Start()
	{
		img = GetComponent<Image>();
		pc = GetComponentInParent<PuzzleController>();
		pl = base.transform.GetChild(0).GetComponent<PuzzleLine>();
		if (pl != null)
		{
			pl.transform.SetParent(base.transform.parent, worldPositionStays: true);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pc.Clicked(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pc.Unclicked();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pc.Hovered(this);
	}

	public void Activate(TileColor color)
	{
		if (tileType == TileType.WhiteEnd)
		{
			base.transform.GetChild(0).GetComponent<Image>().fillCenter = true;
		}
		activated = true;
		Color color2 = pl.TranslateColor(color);
		img.color = new Color(color2.r, color2.g, color2.b, 0.85f);
	}

	public void DeActivate()
	{
		if (tileType == TileType.WhiteEnd)
		{
			base.transform.GetChild(0).GetComponent<Image>().fillCenter = false;
		}
		activated = false;
		img.color = new Color(1f, 1f, 1f, 0.5f);
	}
}
