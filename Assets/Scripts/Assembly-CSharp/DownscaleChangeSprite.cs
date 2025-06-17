using UnityEngine;
using UnityEngine.UI;

public class DownscaleChangeSprite : MonoBehaviour
{
	public Sprite normal;

	public Sprite downscaled;

	private Image img;

	private void Start()
	{
		CheckScale();
	}

	private void OnEnable()
	{
		CheckScale();
	}

	public void CheckScale()
	{
		if (img == null)
		{
			img = GetComponent<Image>();
		}
		if (PlayerPrefs.GetInt("Pix", 1) == 1)
		{
			img.sprite = downscaled;
		}
		else
		{
			img.sprite = normal;
		}
	}
}
