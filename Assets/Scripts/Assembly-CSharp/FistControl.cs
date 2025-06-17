using System.Collections.Generic;
using UnityEngine;

public class FistControl : MonoBehaviour
{
	private InputManager inman;

	public GameObject blueArm;

	private int currentOrderNum;

	private List<GameObject> spawnedArms = new List<GameObject>();

	private List<int> spawnedArmNums = new List<int>();

	private AudioSource aud;

	public bool shopping;

	private int shopRequests;

	public GameObject[] fistPanels;

	private WeaponHUD fistIcon;

	public GameObject heldObject;

	public float fistCooldown;

	public float weightCooldown;

	public bool activated = true;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		aud = GetComponent<AudioSource>();
		fistIcon = fistPanels[0].transform.GetChild(0).GetComponent<WeaponHUD>();
		ResetFists();
		fistCooldown = 0f;
	}

	private void Update()
	{
		if (fistCooldown > -1f)
		{
			fistCooldown = Mathf.MoveTowards(fistCooldown, 0f, Time.deltaTime * 2f);
		}
		if (weightCooldown > 0f)
		{
			weightCooldown = Mathf.MoveTowards(weightCooldown, 0f, Time.deltaTime * (weightCooldown / 2f + 0.1f));
		}
	}

	public void ForceArm(int varNum, bool animation = false)
	{
		if (spawnedArmNums.Contains(varNum))
		{
			ArmChange(spawnedArmNums.IndexOf(varNum));
		}
		else
		{
			ArmChange(0);
		}
		if (animation)
		{
			spawnedArms[currentOrderNum].GetComponent<Punch>().EquipAnimation();
			aud.Play();
		}
	}

	public void ArmChange(int orderNum)
	{
		if (orderNum < spawnedArms.Count)
		{
			if (currentOrderNum < spawnedArms.Count)
			{
				spawnedArms[currentOrderNum].SetActive(value: false);
			}
			spawnedArms[orderNum].SetActive(value: true);
			currentOrderNum = orderNum;
		}
	}

	public void NoFist()
	{
		if (spawnedArms.Count > 0)
		{
			spawnedArms[currentOrderNum].SetActive(value: false);
		}
	}

	public void YesFist()
	{
		if (spawnedArms.Count > 0)
		{
			spawnedArms[currentOrderNum].SetActive(value: true);
		}
	}

	public void ResetFists()
	{
		if (spawnedArms.Count > 0)
		{
			for (int i = 0; i < spawnedArms.Count; i++)
			{
				Object.Destroy(spawnedArms[i]);
			}
			spawnedArms.Clear();
			spawnedArmNums.Clear();
		}
		GameObject item = Object.Instantiate(blueArm, base.transform);
		spawnedArms.Add(item);
		spawnedArmNums.Add(0);
		GameObject[] array = fistPanels;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
		ForceArm(0);
	}

	public void ShopMode()
	{
		shopping = true;
		shopRequests++;
	}

	public void StopShop()
	{
		shopRequests--;
		if (shopRequests <= 0)
		{
			shopping = false;
		}
	}
}
