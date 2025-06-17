using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleHUD : MonoBehaviour
{
	private GameObject styleHud;

	public Image styleRank;

	public Sprite[] ranks;

	public int currentRank;

	private float rankShaking;

	private Vector3 defaultPos;

	private float rankScale;

	private Slider styleMeter;

	public float currentMeter;

	public int maxMeter;

	public float drainSpeed = 1f;

	private Text styleInfo;

	private bool ascending;

	private int testNumber;

	public int maxReachedRank;

	private StatsManager sman;

	private List<string> styleNames = new List<string>();

	private float styleNameTime = 0.1f;

	private AudioSource aud;

	public bool freshWeapon;

	public GameObject freshWeaponObj;

	public Slider freshWeaponSlider;

	private void Start()
	{
		styleHud = base.transform.GetChild(0).gameObject;
		styleMeter = GetComponentInChildren<Slider>();
		styleInfo = GetComponentInChildren<Text>();
		sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		ComboOver();
		defaultPos = styleRank.transform.localPosition;
		aud = GetComponent<AudioSource>();
		if (freshWeaponObj != null)
		{
			freshWeaponSlider = freshWeaponObj.GetComponentInChildren<Slider>();
		}
	}

	private void Update()
	{
		if (styleNameTime > 0f)
		{
			styleNameTime = Mathf.MoveTowards(styleNameTime, 0f, Time.deltaTime * 2f);
		}
		else if (styleNames.Count > 0)
		{
			styleNameTime = 0.1f;
			styleInfo.text = "+ " + styleNames[0] + "\n" + styleInfo.text;
			styleNames.RemoveAt(0);
			Invoke("RemoveText", 3f);
			aud.Play();
		}
		if (currentMeter > 0f && !styleHud.activeSelf)
		{
			ComboStart();
		}
		if (currentMeter != 0f)
		{
			if (currentMeter < 0f)
			{
				if (currentRank == 0)
				{
					ComboOver();
				}
				else if (!ascending)
				{
					ascending = true;
					DescendRank();
				}
			}
			else
			{
				currentMeter -= Time.deltaTime * (drainSpeed * 20f);
			}
		}
		styleMeter.value = currentMeter;
		if (currentMeter > (float)maxMeter && !ascending)
		{
			if (currentRank < 7)
			{
				ascending = true;
				AscendRank();
			}
			else
			{
				currentMeter = maxMeter;
			}
		}
		if (rankShaking > 0f)
		{
			styleRank.transform.localPosition = new Vector3(defaultPos.x + rankShaking * (float)Random.Range(-3, 3), defaultPos.y + rankShaking * (float)Random.Range(-3, 3), defaultPos.z);
			rankShaking -= Time.deltaTime * 5f;
		}
		if (rankScale > 0f)
		{
			styleRank.transform.localScale = new Vector3(2f + rankScale, 1f + rankScale, 1f + rankScale);
			rankScale -= Time.deltaTime;
		}
	}

	public void ComboStart()
	{
		maxMeter = 175;
		if (currentMeter < (float)(maxMeter / 4))
		{
			currentMeter = maxMeter / 4;
		}
		styleHud.SetActive(value: true);
	}

	public void ComboOver()
	{
		currentMeter = 0f;
		currentRank = 0;
		styleHud.SetActive(value: false);
		maxMeter = 9999;
	}

	private void AscendRank()
	{
		float num = currentMeter - (float)maxMeter;
		currentRank++;
		float num2 = currentRank;
		if (currentRank < 4)
		{
			maxMeter = 175 + currentRank * 15;
		}
		else
		{
			maxMeter = 175 + currentRank * 30;
		}
		styleMeter.maxValue = maxMeter;
		if (currentRank < 4)
		{
			drainSpeed = 1f + num2 / 4f;
		}
		else
		{
			drainSpeed = 1f + num2 / 3f;
		}
		styleRank.sprite = ranks[currentRank];
		rankScale = 1f;
		if (num > (float)(maxMeter / 4))
		{
			currentMeter = num;
		}
		else
		{
			currentMeter = maxMeter / 4;
		}
		ascending = false;
		if (maxReachedRank < currentRank)
		{
			maxReachedRank = currentRank;
		}
		DiscordController.UpdateRank(currentRank);
	}

	public void DescendRank()
	{
		if (currentRank > 0)
		{
			currentMeter += maxMeter;
			currentRank--;
			float num = currentRank;
			if (currentRank < 4)
			{
				maxMeter = 175 + currentRank * 15;
			}
			else
			{
				maxMeter = 175 + currentRank * 30;
			}
			styleMeter.maxValue = maxMeter;
			if (currentRank < 4)
			{
				drainSpeed = 1f + num / 4f;
			}
			else
			{
				drainSpeed = 1f + num / 3f;
			}
			styleRank.sprite = ranks[currentRank];
			currentMeter = maxMeter - maxMeter / 4;
			ascending = false;
		}
		else
		{
			ComboOver();
		}
		DiscordController.UpdateRank(currentRank);
	}

	public void AddPoints(int points, string pointName)
	{
		float num = points;
		if (points > 0)
		{
			if (freshWeapon)
			{
				num *= 1.25f;
			}
			currentMeter += num;
			sman.stylePoints += Mathf.RoundToInt(num);
			rankScale = 0.2f;
		}
		if (pointName != "")
		{
			styleNames.Add(pointName);
		}
	}

	public void RemovePoints(int points)
	{
		rankShaking = 5f;
		currentMeter -= points;
	}

	private void RemoveText()
	{
		styleInfo.text = styleInfo.text.Substring(0, styleInfo.text.LastIndexOf("+"));
	}
}
