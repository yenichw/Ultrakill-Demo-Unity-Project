using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
	public int levelNumber;

	public int levelNumberInLayer;

	private Text panelTitle;

	private bool allSecrets;

	public Sprite lockedSprite;

	public Sprite unlockedSprite;

	private Sprite origSprite;

	public Image[] secretIcons;

	public Image challengeIcon;

	private int tempInt;

	private string origName;

	private LayerSelect ls;

	private GameObject challengeChecker;

	private bool beenChecked;

	private void OnEnable()
	{
		if (!beenChecked)
		{
			CheckScore();
		}
	}

	private void OnDisable()
	{
		beenChecked = false;
	}

	public void CheckScore()
	{
		beenChecked = true;
		if (ls == null)
		{
			ls = base.transform.parent.GetComponent<LayerSelect>();
		}
		if (origSprite == null)
		{
			origSprite = base.transform.Find("Image").GetComponent<Image>().sprite;
		}
		if (origName == null)
		{
			origName = base.transform.Find("Name").GetComponent<Text>().text;
		}
		tempInt = GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff", 2));
		Debug.Log("Getting Progress for: " + PlayerPrefs.GetInt("Diff", 2) + ". Progress is: " + tempInt + ". Level Number is: " + levelNumber);
		if (tempInt < levelNumber)
		{
			base.transform.Find("Name").GetComponent<Text>().text = ls.layerNumber + "-" + levelNumberInLayer + ": ???";
			base.transform.Find("Image").GetComponent<Image>().sprite = lockedSprite;
			GetComponent<Button>().enabled = false;
		}
		else
		{
			if (tempInt == levelNumber)
			{
				base.transform.Find("Image").GetComponent<Image>().sprite = unlockedSprite;
			}
			else
			{
				base.transform.Find("Image").GetComponent<Image>().sprite = origSprite;
			}
			base.transform.Find("Name").GetComponent<Text>().text = GetMissionName.GetMission(levelNumber);
			GetComponent<Button>().enabled = true;
			if (challengeChecker == null)
			{
				challengeChecker = challengeIcon.transform.Find("EventTrigger").gameObject;
			}
			if (tempInt > levelNumber)
			{
				challengeChecker.SetActive(value: true);
			}
		}
		string path = string.Concat(Directory.GetParent(Application.dataPath), "/Saves/lvl", levelNumber, "progress.bepis");
		if (File.Exists(path))
		{
			Debug.Log("Found Level " + levelNumber + " Data");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
			fileStream.Close();
			int @int = PlayerPrefs.GetInt("Diff", 2);
			if (rankData.levelNumber == levelNumber)
			{
				Text componentInChildren = base.transform.Find("Stats").Find("Rank").GetComponentInChildren<Text>();
				if (rankData.ranks[@int] == 12 && (rankData.majorAssists == null || !rankData.majorAssists[@int]))
				{
					componentInChildren.text = "<color=#FFFFFF>P</color>";
					Image component = componentInChildren.transform.parent.GetComponent<Image>();
					component.color = new Color(1f, 0.686f, 0f, 1f);
					component.fillCenter = true;
					ls.AddScore(4, perfect: true);
				}
				else if (rankData.majorAssists != null && rankData.majorAssists[@int])
				{
					switch (rankData.ranks[@int])
					{
					case 1:
						componentInChildren.text = "C";
						ls.AddScore(1);
						break;
					case 2:
						componentInChildren.text = "B";
						ls.AddScore(2);
						break;
					case 3:
						componentInChildren.text = "A";
						ls.AddScore(3);
						break;
					case 4:
					case 5:
					case 6:
						ls.AddScore(4);
						componentInChildren.text = "S";
						break;
					default:
						ls.AddScore(0);
						componentInChildren.text = "D";
						break;
					}
					Image component2 = componentInChildren.transform.parent.GetComponent<Image>();
					component2.color = new Color(0.3f, 0.6f, 0.9f, 1f);
					component2.fillCenter = true;
				}
				else if (rankData.ranks[@int] < 0)
				{
					componentInChildren.text = "";
					Image component3 = componentInChildren.transform.parent.GetComponent<Image>();
					component3.color = Color.white;
					component3.fillCenter = false;
				}
				else
				{
					switch (rankData.ranks[@int])
					{
					case 1:
						componentInChildren.text = "<color=#4CFF00>C</color>";
						ls.AddScore(1);
						break;
					case 2:
						componentInChildren.text = "<color=#FFD800>B</color>";
						ls.AddScore(2);
						break;
					case 3:
						componentInChildren.text = "<color=#FF6A00>A</color>";
						ls.AddScore(3);
						break;
					case 4:
					case 5:
					case 6:
						ls.AddScore(4);
						componentInChildren.text = "<color=#FF0000>S</color>";
						break;
					default:
						ls.AddScore(0);
						componentInChildren.text = "<color=#0094FF>D</color>";
						break;
					}
					Image component4 = componentInChildren.transform.parent.GetComponent<Image>();
					component4.color = Color.white;
					component4.fillCenter = false;
				}
				if (rankData.secretsAmount > 0)
				{
					allSecrets = true;
					for (int i = 0; i < 5; i++)
					{
						if (i < rankData.secretsAmount && rankData.secretsFound[i])
						{
							secretIcons[i].fillCenter = true;
						}
						else if (i < rankData.secretsAmount)
						{
							allSecrets = false;
							secretIcons[i].fillCenter = false;
						}
						else if (i >= rankData.secretsAmount)
						{
							secretIcons[i].enabled = false;
						}
					}
				}
				else
				{
					Image[] array = secretIcons;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].enabled = false;
					}
				}
				if (rankData.challenge)
				{
					Debug.Log("Challenge Complete " + rankData.levelNumber);
					challengeIcon.fillCenter = true;
					Text componentInChildren2 = challengeIcon.GetComponentInChildren<Text>();
					componentInChildren2.text = "C O M P L E T E";
					if (rankData.ranks[@int] == 12 && (allSecrets || rankData.secretsAmount == 0))
					{
						componentInChildren2.color = new Color(0.6f, 0.4f, 0f, 1f);
					}
					else
					{
						componentInChildren2.color = Color.black;
					}
				}
				else
				{
					challengeIcon.fillCenter = false;
					Text componentInChildren3 = challengeIcon.GetComponentInChildren<Text>();
					componentInChildren3.text = "C H A L L E N G E";
					componentInChildren3.color = Color.white;
				}
			}
			else
			{
				Debug.Log("Error in finding " + levelNumber + " Data");
				Image component5 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
				component5.color = Color.white;
				component5.fillCenter = false;
				component5.GetComponentInChildren<Text>().text = "";
				allSecrets = false;
				Image[] array = secretIcons;
				foreach (Image obj in array)
				{
					obj.enabled = true;
					obj.fillCenter = false;
				}
			}
			if (rankData.challenge && rankData.ranks[@int] == 12 && (allSecrets || rankData.secretsAmount == 0))
			{
				ls.Gold();
				GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 0.75f);
			}
			else
			{
				GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
			}
		}
		else
		{
			Debug.Log("Didn't Find Level " + levelNumber + " Data");
			Image component6 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
			component6.color = Color.white;
			component6.fillCenter = false;
			component6.GetComponentInChildren<Text>().text = "";
			allSecrets = false;
			Image[] array = secretIcons;
			foreach (Image obj2 in array)
			{
				obj2.enabled = true;
				obj2.fillCenter = false;
			}
		}
	}
}
