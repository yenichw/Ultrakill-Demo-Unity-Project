using UnityEngine;

public class MusicChanger : MonoBehaviour
{
	public bool match;

	public bool oneTime;

	public bool onEnable;

	public AudioClip clean;

	public AudioClip battle;

	public AudioClip boss;

	private MusicManager muman;

	private void OnEnable()
	{
		if (onEnable)
		{
			Change();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!onEnable && other.gameObject.tag == "Player")
		{
			Change();
		}
	}

	private void Change()
	{
		if (muman == null)
		{
			muman = Object.FindObjectOfType<MusicManager>();
		}
		if (oneTime || muman.cleanTheme.clip != clean)
		{
			float time = 0f;
			if (match)
			{
				time = muman.cleanTheme.time;
			}
			muman.StopMusic();
			muman.cleanTheme.clip = clean;
			muman.battleTheme.clip = battle;
			muman.bossTheme.clip = boss;
			muman.StartMusic();
			muman.cleanTheme.time = time;
			muman.battleTheme.time = time;
			muman.bossTheme.time = time;
			if (oneTime)
			{
				Object.Destroy(this);
			}
		}
	}
}
