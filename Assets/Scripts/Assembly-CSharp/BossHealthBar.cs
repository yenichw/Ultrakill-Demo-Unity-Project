using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	public GameObject bossBar;

	public Slider[] hpSlider;

	public Slider[] hpAfterImage;

	private EnemyIdentifier eid;

	private SpiderBody spb;

	private Machine mac;

	private Statue stat;

	public string bossName;

	private float introCharge;

	private GameObject filler;

	private float shakeTime;

	private Vector3 originalPosition;

	private bool done;

	public FinalDoor finalDoor;

	private int currentHpSlider;

	private int currentAfterImageSlider;

	private MusicManager mman;

	private float waitForDamage;

	private void Awake()
	{
		eid = GetComponent<EnemyIdentifier>();
		bossBar.GetComponentInChildren<Text>().text = bossName;
		if (eid.type == EnemyType.Spider)
		{
			spb = GetComponent<SpiderBody>();
		}
		if (eid.type == EnemyType.Machine)
		{
			mac = GetComponent<Machine>();
		}
		if (eid.type == EnemyType.Statue)
		{
			stat = GetComponent<Statue>();
		}
		filler = bossBar.transform.GetChild(0).gameObject;
		originalPosition = filler.transform.localPosition;
		currentHpSlider = hpSlider.Length - 1;
		currentAfterImageSlider = currentHpSlider;
		Slider[] array = hpSlider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].value = 0f;
		}
		array = hpAfterImage;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].value = 0f;
		}
		if (mman == null)
		{
			mman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		}
	}

	private void OnEnable()
	{
		if (!eid.dead)
		{
			if (mman == null)
			{
				mman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
			}
			mman.PlayBossMusic();
			bossBar.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		DisappearBar();
	}

	private void Update()
	{
		if (eid.type == EnemyType.Spider && hpSlider[currentHpSlider].value != spb.health)
		{
			if (introCharge < spb.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, spb.health, (spb.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array = hpSlider;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - spb.health);
				hpSlider[currentHpSlider].value = spb.health;
				waitForDamage = 0.15f;
				if (hpSlider[currentHpSlider].minValue > spb.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = spb.health;
				}
			}
		}
		else if (eid.type == EnemyType.Machine && hpSlider[currentHpSlider].value != mac.health)
		{
			if (introCharge < mac.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, mac.health, (mac.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array = hpSlider;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - mac.health);
				hpSlider[currentHpSlider].value = mac.health;
				waitForDamage = 0.15f;
				if (hpSlider[currentHpSlider].minValue > mac.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = mac.health;
				}
			}
		}
		else if (eid.type == EnemyType.Statue && hpSlider[currentHpSlider].value != stat.health)
		{
			if (introCharge < stat.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, stat.health, (stat.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array = hpSlider;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - stat.health);
				hpSlider[currentHpSlider].value = stat.health;
				waitForDamage = 0.15f;
				if (hpSlider[currentHpSlider].minValue > stat.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = stat.health;
				}
			}
		}
		if (hpAfterImage[currentAfterImageSlider].value != hpSlider[currentHpSlider].value)
		{
			if (waitForDamage > 0f && hpSlider[0].value > 0f)
			{
				waitForDamage = Mathf.MoveTowards(waitForDamage, 0f, Time.deltaTime);
			}
			else if (hpAfterImage[currentAfterImageSlider].value > hpSlider[currentHpSlider].value)
			{
				hpAfterImage[currentAfterImageSlider].value = Mathf.MoveTowards(hpAfterImage[currentAfterImageSlider].value, hpSlider[currentHpSlider].value, Time.deltaTime * (Mathf.Abs((hpAfterImage[currentAfterImageSlider].value - hpSlider[currentHpSlider].value) * 5f) + 0.5f));
			}
			else
			{
				hpAfterImage[currentAfterImageSlider].value = hpSlider[currentHpSlider].value;
			}
			if (hpAfterImage[currentAfterImageSlider].value <= hpAfterImage[currentAfterImageSlider].minValue && currentAfterImageSlider > 0)
			{
				currentAfterImageSlider--;
			}
		}
		if (shakeTime != 0f)
		{
			if (shakeTime > 10f)
			{
				shakeTime = 10f;
			}
			shakeTime -= Time.deltaTime * 10f;
			filler.transform.localPosition = new Vector3(originalPosition.x + Random.Range(0f - shakeTime, shakeTime), originalPosition.y + Random.Range(0f - shakeTime, shakeTime), originalPosition.z);
			if (shakeTime < 0f)
			{
				shakeTime = 0f;
				filler.transform.localPosition = originalPosition;
			}
		}
		if (!done && hpSlider[0].value <= 0f && (mac == null || mac.limp))
		{
			done = true;
			if (finalDoor != null)
			{
				Invoke("OpenDoors", 1f);
			}
			Invoke("DisappearBar", 3f);
			Debug.Log("Invoking");
		}
	}

	private void OpenDoors()
	{
		finalDoor.Open();
	}

	public void DisappearBar()
	{
		if (bossBar != null)
		{
			bossBar.SetActive(value: false);
		}
	}
}
