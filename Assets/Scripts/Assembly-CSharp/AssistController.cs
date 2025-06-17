using UnityEngine;

public class AssistController : MonoBehaviour
{
	public bool majorEnabled;

	public float gameSpeed;

	public float damageTaken;

	public bool infiniteStamina;

	public bool disableHardDamage;

	private StatsManager sman;

	private void Start()
	{
		if (PlayerPrefs.GetInt("MajAss", 0) == 1)
		{
			majorEnabled = true;
		}
		else
		{
			majorEnabled = false;
		}
		gameSpeed = PlayerPrefs.GetFloat("GamSpe", 100f) / 100f;
		damageTaken = PlayerPrefs.GetFloat("DamTak", 100f) / 100f;
		if (PlayerPrefs.GetInt("InfSta", 0) == 1)
		{
			infiniteStamina = true;
		}
		else
		{
			infiniteStamina = false;
		}
		if (PlayerPrefs.GetInt("DisHarDam", 0) == 1)
		{
			disableHardDamage = true;
		}
		else
		{
			disableHardDamage = false;
		}
	}

	public void MajorEnabled()
	{
		majorEnabled = true;
		if (sman == null)
		{
			sman = GetComponent<StatsManager>();
		}
		sman.MajorUsed();
	}
}
