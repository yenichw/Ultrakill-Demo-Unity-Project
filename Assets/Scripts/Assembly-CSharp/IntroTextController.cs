using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class IntroTextController : MonoBehaviour
{
	public bool firstTime;

	public GameObject page2Screen;

	public GameObject[] deactivateOnIntroEnd;

	public Slider soundSlider;

	public Slider musicSlider;

	public AudioMixer[] audmix;

	private Image img;

	private Text page2Text;

	private float fadeValue;

	private bool inMenu;

	public bool introOver;

	private float introOverWait = 1f;

	private Rigidbody rb;

	private void Awake()
	{
		if (firstTime)
		{
			soundSlider.value = 0f;
			musicSlider.value = 0f;
			PlayerPrefs.SetFloat("MaVo", 0f);
			PlayerPrefs.SetFloat("MuVo", 0f);
		}
		else
		{
			soundSlider.value = PlayerPrefs.GetFloat("MaVo", 1f) * 100f;
			musicSlider.value = PlayerPrefs.GetFloat("MuVo", 0.6f) * 100f;
		}
	}

	private void Start()
	{
		float value = 0f;
		audmix[0].GetFloat("allVolume", out value);
		Debug.Log("Mixer Volume " + value);
		AudioMixer[] array = audmix;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat("allVolume", -80f);
		}
		audmix[0].GetFloat("allVolume", out value);
		Debug.Log("Mixer Volume " + value);
		Invoke("SlowDown", 0.1f);
		Object.FindObjectOfType<OptionsManager>().inIntro = true;
		rb = Object.FindObjectOfType<NewMovement>().GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
	}

	private void SlowDown()
	{
		inMenu = true;
	}

	private void Update()
	{
		if (inMenu)
		{
			rb.velocity = Vector3.zero;
			rb.useGravity = false;
			if (page2Screen.activeSelf)
			{
				Object.FindObjectOfType<NewMovement>().GetComponent<Rigidbody>().useGravity = true;
				inMenu = false;
			}
		}
		else
		{
			if (!introOver)
			{
				return;
			}
			if (!img)
			{
				img = GetComponent<Image>();
				page2Text = page2Screen.GetComponent<Text>();
				fadeValue = 1f;
			}
			if (fadeValue > 0f)
			{
				fadeValue = Mathf.MoveTowards(fadeValue, 0f, Time.deltaTime * 0.375f);
				Color color = img.color;
				color.a = fadeValue;
				img.color = color;
				AudioMixer[] array = audmix;
				foreach (AudioMixer obj in array)
				{
					float value = 0f;
					obj.GetFloat("allVolume", out value);
					obj.SetFloat("allVolume", Mathf.MoveTowards(value, 0f, Time.deltaTime * Mathf.Abs(value)));
				}
				color = page2Text.color;
				color.a = fadeValue;
				page2Text.color = color;
			}
			else if (introOverWait > 0f)
			{
				introOverWait = Mathf.MoveTowards(introOverWait, 0f, Time.deltaTime);
				AudioMixer[] array = audmix;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFloat("allVolume", introOverWait * -5.47f);
				}
			}
			else
			{
				Object.FindObjectOfType<OptionsManager>().inIntro = false;
				GameObject[] array2 = deactivateOnIntroEnd;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].SetActive(value: false);
				}
			}
		}
	}
}
