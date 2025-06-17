using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public bool off;

	public bool dontMatch;

	public AudioSource battleTheme;

	public AudioSource cleanTheme;

	public AudioSource bossTheme;

	public AudioSource targetTheme;

	private AudioSource[] allThemes;

	public float volume;

	public float requestedThemes;

	private bool arenaMode;

	private float defaultVolume;

	public float fadeSpeed;

	private void Start()
	{
		if (fadeSpeed == 0f)
		{
			fadeSpeed = 1f;
		}
		allThemes = GetComponentsInChildren<AudioSource>();
		defaultVolume = volume;
		if (!off)
		{
			AudioSource[] array = allThemes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
			cleanTheme.volume = volume;
			targetTheme = cleanTheme;
		}
		else
		{
			targetTheme = GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		AudioSource[] array;
		if (!off && targetTheme.volume != volume)
		{
			array = allThemes;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource == targetTheme)
				{
					if (audioSource.volume > volume)
					{
						audioSource.volume = volume;
					}
					audioSource.volume = Mathf.MoveTowards(audioSource.volume, volume, fadeSpeed * Time.deltaTime);
				}
				else
				{
					audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);
				}
			}
			if (targetTheme.volume == volume)
			{
				array = allThemes;
				foreach (AudioSource audioSource2 in array)
				{
					if (audioSource2 != targetTheme)
					{
						audioSource2.volume = 0f;
					}
				}
			}
		}
		if (volume != 0f && (!off || !(targetTheme.volume > 0f)))
		{
			return;
		}
		array = allThemes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume -= Time.deltaTime / 5f * fadeSpeed;
		}
		if (targetTheme.volume <= 0f)
		{
			array = allThemes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = 0f;
			}
		}
	}

	public void StartMusic()
	{
		off = false;
		AudioSource[] array = allThemes;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip != null)
			{
				audioSource.Play();
			}
		}
		cleanTheme.volume = volume;
		targetTheme = cleanTheme;
	}

	public void PlayBattleMusic()
	{
		if (!dontMatch && targetTheme != battleTheme)
		{
			battleTheme.time = cleanTheme.time;
		}
		if (targetTheme != bossTheme)
		{
			targetTheme = battleTheme;
		}
		requestedThemes += 1f;
	}

	public void PlayCleanMusic()
	{
		requestedThemes -= 1f;
		if (requestedThemes <= 0f && !arenaMode)
		{
			requestedThemes = 0f;
			if (battleTheme.volume == volume)
			{
				cleanTheme.time = battleTheme.time;
			}
			targetTheme = cleanTheme;
		}
	}

	public void PlayBossMusic()
	{
		Debug.Log("PlayBossMusic");
		if (targetTheme != bossTheme)
		{
			bossTheme.time = cleanTheme.time;
		}
		targetTheme = bossTheme;
	}

	public void ArenaMusicStart()
	{
		if (off)
		{
			off = false;
		}
		if (!battleTheme.isPlaying)
		{
			AudioSource[] array = allThemes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
			battleTheme.volume = volume;
		}
		if (targetTheme != bossTheme)
		{
			targetTheme = battleTheme;
		}
		arenaMode = true;
	}

	public void ArenaMusicEnd()
	{
		requestedThemes = 0f;
		targetTheme = cleanTheme;
		arenaMode = false;
	}

	public void StopMusic()
	{
		off = true;
		AudioSource[] array = allThemes;
		foreach (AudioSource obj in array)
		{
			obj.volume = 0f;
			obj.Stop();
		}
	}
}
