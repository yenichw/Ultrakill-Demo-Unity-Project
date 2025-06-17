using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
	private InputManager inman;

	public bool activated = true;

	private int rememberedSlot;

	public int currentVariation;

	public int currentSlot;

	public GameObject currentWeapon;

	public List<List<GameObject>> slots = new List<List<GameObject>>();

	public List<GameObject> slot1 = new List<GameObject>();

	public List<GameObject> slot2 = new List<GameObject>();

	public List<GameObject> slot3 = new List<GameObject>();

	public List<GameObject> slot4 = new List<GameObject>();

	public List<GameObject> slot5 = new List<GameObject>();

	public List<GameObject> allWeapons = new List<GameObject>();

	private AudioSource aud;

	public float killCharge;

	public Slider killMeter;

	public bool noWeapons = true;

	public int lastUsedSlot = 69;

	public int lastUsedVariation = 69;

	public float headShotComboTime;

	public int headshots;

	private StyleHUD shud;

	public GameObject[] gunPanel;

	private WeaponHUD whud;

	private List<float> weaponFreshnesses = new List<float>();

	private float scrollCooldown;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		currentVariation = PlayerPrefs.GetInt("CurVar", 0);
		currentSlot = PlayerPrefs.GetInt("CurSlo", 1);
		lastUsedVariation = PlayerPrefs.GetInt("LasVar", 69);
		lastUsedSlot = PlayerPrefs.GetInt("LasSlo", 69);
		if (currentSlot < 1)
		{
			currentSlot = 1;
		}
		aud = GetComponent<AudioSource>();
		slots.Add(slot1);
		slots.Add(slot2);
		slots.Add(slot3);
		slots.Add(slot4);
		slots.Add(slot5);
		foreach (List<GameObject> slot in slots)
		{
			if (slot.Count <= 0)
			{
				continue;
			}
			foreach (GameObject item in slot)
			{
				if (item != null)
				{
					allWeapons.Add(item);
					weaponFreshnesses.Add(10f);
				}
			}
		}
		foreach (List<GameObject> slot2 in slots)
		{
			if (slot2.Count != 0)
			{
				noWeapons = false;
			}
		}
		if (currentWeapon == null && slots[currentSlot - 1].Count > currentVariation)
		{
			currentWeapon = slots[currentSlot - 1][currentVariation];
		}
		else if (currentWeapon == null && slot1.Count != 0)
		{
			currentSlot = 1;
			currentVariation = 0;
			currentWeapon = slot1[0];
		}
		shud = Object.FindObjectOfType<StyleHUD>();
		whud = gunPanel[0].transform.GetChild(0).GetChild(0).GetComponent<WeaponHUD>();
		whud.UpdateImage(currentSlot, currentVariation);
		UpdateWeaponList(firstTime: true);
	}

	private void Update()
	{
		if (activated)
		{
			if (headShotComboTime > 0f)
			{
				headShotComboTime = Mathf.MoveTowards(headShotComboTime, 0f, Time.deltaTime);
			}
			else
			{
				headshots = 0;
			}
			if (lastUsedSlot == 0)
			{
				lastUsedSlot = 69;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				if (slot1.Count > 0 && slot1[0] != null && (slot1.Count > 1 || currentSlot != 1))
				{
					SwitchWeapon(1, slot1);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				if (slot2.Count > 0 && slot2[0] != null && (slot2.Count > 1 || currentSlot != 2))
				{
					SwitchWeapon(2, slot2);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3) && (slot3.Count > 1 || currentSlot != 3))
			{
				if (slot3.Count > 0 && slot3[0] != null)
				{
					SwitchWeapon(3, slot3);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4) && (slot4.Count > 1 || currentSlot != 4))
			{
				if (slot4.Count > 0 && slot4[0] != null)
				{
					SwitchWeapon(4, slot4);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5) && (slot5.Count > 1 || currentSlot != 5))
			{
				if (slot5.Count > 0 && slot5[0] != null)
				{
					SwitchWeapon(5, slot5);
				}
			}
			else if (Input.GetKeyDown(inman.Inputs["LastUsedWeapon"]) && lastUsedSlot != 69)
			{
				if (slots[lastUsedSlot - 1] != null)
				{
					SwitchWeapon(lastUsedSlot, slots[lastUsedSlot - 1], lastUsed: true);
				}
			}
			else if (Input.GetKeyDown(inman.Inputs["ChangeVariation"]) && slots[currentSlot - 1].Count > 1)
			{
				SwitchWeapon(currentSlot, slots[currentSlot - 1]);
			}
			else if (!noWeapons && inman.ScrOn)
			{
				if (((!inman.ScrRev && Input.GetAxis("Mouse ScrollWheel") > 0f) || (inman.ScrRev && Input.GetAxis("Mouse ScrollWheel") < 0f)) && scrollCooldown <= 0f)
				{
					int num = 0;
					if (inman.ScrWep && inman.ScrVar)
					{
						foreach (List<GameObject> slot in slots)
						{
							if (slot.Count > 0)
							{
								num++;
							}
						}
					}
					bool flag = false;
					if (inman.ScrVar)
					{
						if (slots[currentSlot - 1].Count > currentVariation + 1 || ((!inman.ScrWep || num <= 1) && slots[currentSlot - 1].Count > 1))
						{
							SwitchWeapon(currentSlot, slots[currentSlot - 1]);
							scrollCooldown = 0.5f;
							flag = true;
						}
						else if (!inman.ScrWep)
						{
							flag = true;
						}
					}
					if (!flag && inman.ScrWep)
					{
						if (!flag && currentSlot < slots.Count)
						{
							for (int i = currentSlot; i < slots.Count; i++)
							{
								if (slots[i].Count > 0)
								{
									flag = true;
									SwitchWeapon(i + 1, slots[i]);
									scrollCooldown = 0.5f;
									break;
								}
							}
						}
						if (!flag)
						{
							for (int j = 0; j < currentSlot; j++)
							{
								if (slots[j].Count > 0)
								{
									if (j != currentSlot - 1)
									{
										SwitchWeapon(j + 1, slots[j]);
										scrollCooldown = 0.5f;
									}
									break;
								}
							}
						}
					}
				}
				else if (((!inman.ScrRev && Input.GetAxis("Mouse ScrollWheel") < 0f) || (inman.ScrRev && Input.GetAxis("Mouse ScrollWheel") > 0f)) && scrollCooldown <= 0f)
				{
					int num2 = 0;
					if (inman.ScrWep && inman.ScrVar)
					{
						foreach (List<GameObject> slot2 in slots)
						{
							if (slot2.Count > 0)
							{
								num2++;
							}
						}
					}
					if ((inman.ScrWep && !inman.ScrVar) || (inman.ScrWep && num2 > 1))
					{
						if (inman.ScrVar)
						{
							if (currentVariation != 0)
							{
								GameObject weapon = slots[currentSlot - 1][currentVariation - 1];
								ForceWeapon(weapon);
								scrollCooldown = 0.5f;
							}
							else if (currentSlot == 1)
							{
								for (int num3 = slots.Count - 1; num3 >= 0; num3--)
								{
									if (slots[num3].Count > 0)
									{
										if (num3 != currentSlot - 1)
										{
											GameObject weapon2 = slots[num3][slots[num3].Count - 1];
											ForceWeapon(weapon2);
											scrollCooldown = 0.5f;
										}
										break;
									}
								}
							}
							else
							{
								bool flag2 = false;
								for (int num4 = currentSlot - 2; num4 >= 0; num4--)
								{
									if (slots[num4].Count > 0)
									{
										GameObject weapon3 = slots[num4][slots[num4].Count - 1];
										ForceWeapon(weapon3);
										scrollCooldown = 0.5f;
										flag2 = true;
									}
								}
								if (!flag2)
								{
									for (int num5 = slots.Count - 1; num5 >= 0; num5--)
									{
										if (slots[num5].Count > 0)
										{
											if (num5 != currentSlot - 1)
											{
												GameObject weapon4 = slots[num5][slots[num5].Count - 1];
												ForceWeapon(weapon4);
												scrollCooldown = 0.5f;
											}
											break;
										}
									}
								}
							}
						}
						else if (currentSlot == 1)
						{
							for (int num6 = slots.Count - 1; num6 >= 0; num6--)
							{
								if (slots[num6].Count > 0)
								{
									if (num6 != currentSlot - 1)
									{
										SwitchWeapon(num6 + 1, slots[num6]);
										scrollCooldown = 0.5f;
									}
									break;
								}
							}
						}
						else
						{
							bool flag3 = false;
							for (int num7 = currentSlot - 2; num7 >= 0; num7--)
							{
								if (slots[num7].Count > 0)
								{
									SwitchWeapon(num7 + 1, slots[num7]);
									scrollCooldown = 0.5f;
									flag3 = true;
									break;
								}
							}
							if (!flag3)
							{
								for (int num8 = slots.Count - 1; num8 >= 0; num8--)
								{
									if (slots[num8].Count > 0)
									{
										if (num8 != currentSlot - 1)
										{
											SwitchWeapon(num8 + 1, slots[num8]);
											scrollCooldown = 0.5f;
										}
										break;
									}
								}
							}
						}
					}
					else if (slots[currentSlot - 1].Count > 1)
					{
						if (currentVariation != 0)
						{
							GameObject weapon5 = slots[currentSlot - 1][currentVariation - 1];
							ForceWeapon(weapon5);
							scrollCooldown = 0.5f;
						}
						else
						{
							GameObject weapon6 = slots[currentSlot - 1][slots[currentSlot - 1].Count - 1];
							ForceWeapon(weapon6);
							scrollCooldown = 0.5f;
						}
					}
				}
			}
		}
		if (scrollCooldown > 0f)
		{
			scrollCooldown = Mathf.MoveTowards(scrollCooldown, 0f, Time.deltaTime * 5f);
		}
	}

	private void FixedUpdate()
	{
		if (!activated || allWeapons == null || allWeapons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < allWeapons.Count; i++)
		{
			if (!(allWeapons[i] != null))
			{
				continue;
			}
			if (!allWeapons[i].activeSelf && weaponFreshnesses[i] < 20f)
			{
				weaponFreshnesses[i] = Mathf.MoveTowards(weaponFreshnesses[i], 20f, 0.01f);
			}
			else if (allWeapons[i].activeSelf)
			{
				weaponFreshnesses[i] = Mathf.MoveTowards(weaponFreshnesses[i], 0f, 0.015f);
				if (weaponFreshnesses[i] < 10f)
				{
					shud.freshWeapon = false;
					shud.freshWeaponObj.SetActive(value: false);
				}
				else
				{
					shud.freshWeaponSlider.value = weaponFreshnesses[i];
				}
			}
		}
	}

	public void SwitchWeapon(int target, List<GameObject> slot, bool lastUsed = false)
	{
		if (currentWeapon != null)
		{
			currentWeapon.SetActive(value: false);
		}
		shud.freshWeapon = false;
		shud.freshWeaponObj.SetActive(value: false);
		int num = target;
		if (lastUsed)
		{
			if (slots[lastUsedSlot - 1].Count == 0)
			{
				lastUsedSlot = currentSlot;
				slot = slots[currentSlot - 1];
				num = currentSlot;
				if (currentVariation == 0)
				{
					if (slots[lastUsedSlot - 1].Count > 1)
					{
						lastUsedVariation = slots[lastUsedSlot - 1].Count - 1;
					}
					else
					{
						lastUsedVariation = 0;
					}
				}
				else
				{
					lastUsedVariation = currentVariation - 1;
				}
			}
			int num2 = currentVariation;
			currentVariation = lastUsedVariation;
			lastUsedVariation = num2;
			lastUsedSlot = currentSlot;
		}
		else if (currentSlot == target && currentVariation + 1 < slot.Count)
		{
			if (lastUsedSlot == 69)
			{
				lastUsedSlot = currentSlot;
				lastUsedVariation = currentVariation;
			}
			currentVariation++;
		}
		else
		{
			if (currentSlot != target)
			{
				lastUsedSlot = currentSlot;
				lastUsedVariation = currentVariation;
			}
			else
			{
				int num3 = 0;
				foreach (List<GameObject> slot2 in slots)
				{
					if (slot2.Count != 0)
					{
						num3++;
					}
				}
				if (num3 <= 1)
				{
					lastUsedSlot = currentSlot;
					lastUsedVariation = currentVariation;
				}
			}
			currentVariation = 0;
		}
		currentSlot = num;
		if (lastUsed && slots[currentSlot - 1].Count < currentVariation + 1)
		{
			currentVariation = 0;
		}
		if (!noWeapons)
		{
			currentWeapon = slots[currentSlot - 1][currentVariation];
			currentWeapon.SetActive(value: true);
			if (weaponFreshnesses[allWeapons.IndexOf(currentWeapon)] > 15f)
			{
				shud.freshWeapon = true;
				shud.freshWeaponObj.SetActive(value: true);
			}
			aud.Play();
			PlayerPrefs.SetInt("CurVar", currentVariation);
			PlayerPrefs.SetInt("CurSlo", currentSlot);
			PlayerPrefs.SetInt("LasVar", lastUsedVariation);
			PlayerPrefs.SetInt("LasSlo", lastUsedSlot);
		}
		whud.UpdateImage(currentSlot, currentVariation);
	}

	public void ForceWeapon(GameObject weapon)
	{
		new List<GameObject>();
		foreach (List<GameObject> slot in slots)
		{
			for (int i = 0; i < slot.Count; i++)
			{
				if (slot[i].name == weapon.name + "(Clone)" || slot[i].name == weapon.name)
				{
					if (currentWeapon != null)
					{
						currentWeapon.SetActive(value: false);
					}
					currentSlot = slots.IndexOf(slot) + 1;
					currentVariation = i;
					currentWeapon = slot[currentVariation];
					currentWeapon.SetActive(value: true);
					aud.Play();
					whud.UpdateImage(currentSlot, currentVariation);
					break;
				}
			}
		}
	}

	public void NoWeapon()
	{
		if (currentWeapon != null)
		{
			currentWeapon.SetActive(value: false);
			rememberedSlot = currentSlot;
			activated = false;
		}
	}

	public void YesWeapon()
	{
		if (slots[currentSlot - 1].Count > currentVariation && slots[currentSlot - 1][currentVariation] != null)
		{
			currentWeapon = slots[currentSlot - 1][currentVariation];
			currentWeapon.SetActive(value: true);
		}
		else if (slots[currentSlot - 1].Count > 0)
		{
			currentWeapon = slots[currentSlot - 1][0];
			currentVariation = 0;
			currentWeapon.SetActive(value: true);
		}
		else
		{
			int num = -1;
			for (int i = 0; i < currentSlot; i++)
			{
				if (slots[i].Count > 0)
				{
					num = i;
				}
			}
			if (num == -1)
			{
				num = 99;
				for (int j = currentSlot; j < slots.Count; j++)
				{
					if (slots[j].Count > 0 && j < num)
					{
						num = j;
					}
				}
			}
			if (num != 99)
			{
				currentWeapon = slots[num][0];
				currentSlot = num + 1;
				currentVariation = 0;
			}
			else
			{
				noWeapons = true;
			}
		}
		if (currentWeapon != null)
		{
			currentWeapon.SetActive(value: false);
			activated = true;
			currentWeapon.SetActive(value: true);
		}
		whud.UpdateImage(currentSlot, currentVariation);
	}

	public void AddKill()
	{
		if (killCharge < killMeter.maxValue)
		{
			killCharge += 1f;
			if (killCharge > killMeter.maxValue)
			{
				killCharge = killMeter.maxValue;
			}
			killMeter.value = killCharge;
		}
	}

	public void ClearKills()
	{
		killCharge = 0f;
		killMeter.value = killCharge;
	}

	public void UpdateWeaponList(bool firstTime = false)
	{
		allWeapons.Clear();
		noWeapons = true;
		foreach (List<GameObject> slot in slots)
		{
			foreach (GameObject item in slot)
			{
				if (item != null)
				{
					allWeapons.Add(item);
					weaponFreshnesses.Add(10f);
					if (noWeapons)
					{
						noWeapons = false;
					}
				}
			}
		}
		if (gunPanel == null || gunPanel.Length == 0)
		{
			return;
		}
		GameObject[] array;
		if (noWeapons || PlayerPrefs.GetInt("WeaIco", 1) == 0)
		{
			array = gunPanel;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			return;
		}
		if (whud == null && gunPanel != null && gunPanel.Length != 0)
		{
			whud = gunPanel[0].transform.GetChild(0).GetChild(0).GetComponent<WeaponHUD>();
		}
		if (whud != null)
		{
			whud.UpdateImage(currentSlot, currentVariation);
		}
		array = gunPanel;
		foreach (GameObject gameObject in array)
		{
			if (!firstTime || gameObject != gunPanel[0])
			{
				gameObject.SetActive(value: true);
			}
		}
	}
}
