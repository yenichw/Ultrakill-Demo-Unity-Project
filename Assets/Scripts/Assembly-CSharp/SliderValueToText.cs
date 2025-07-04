using UnityEngine;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
	private Slider targetSlider;

	private Text targetText;

	public string suffix;

	public string ifMax;

	public string ifMin;

	public Color minColor;

	public Color maxColor;

	private Color origColor;

	private Color nullColor;

	private void Start()
	{
		targetSlider = GetComponentInParent<Slider>();
		targetText = GetComponent<Text>();
		origColor = targetText.color;
		nullColor = new Color(0f, 0f, 0f, 0f);
	}

	private void Update()
	{
		if (ifMax != "" && targetSlider.value == targetSlider.maxValue)
		{
			targetText.text = ifMax;
		}
		else if (ifMin != "" && targetSlider.value == targetSlider.minValue)
		{
			targetText.text = ifMin;
		}
		else
		{
			targetText.text = targetSlider.value + suffix;
		}
		if (maxColor != nullColor && targetSlider.value == targetSlider.maxValue)
		{
			targetText.color = maxColor;
		}
		else if (minColor != nullColor && targetSlider.value == targetSlider.minValue)
		{
			targetText.color = minColor;
		}
		else
		{
			targetText.color = origColor;
		}
	}
}
