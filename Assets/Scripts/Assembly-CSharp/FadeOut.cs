using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
	public bool fadeIn;

	public bool distance;

	private List<float> origVol = new List<float>();

	public AudioSource[] auds;

	private bool fading;

	public float speed;

	public float maxDistance;

	public bool activateOnEnable;

	private GameObject player;

	private void Start()
	{
		if (auds.Length == 0)
		{
			auds = GetComponents<AudioSource>();
		}
		if (fadeIn)
		{
			AudioSource[] array = auds;
			foreach (AudioSource audioSource in array)
			{
				origVol.Add(audioSource.volume);
				audioSource.volume = 0f;
			}
		}
		player = GameObject.FindWithTag("Player");
		if (activateOnEnable)
		{
			BeginFade();
		}
	}

	private void Update()
	{
		if (fading)
		{
			if (fadeIn)
			{
				for (int i = 0; i < auds.Length; i++)
				{
					if (auds[i].isPlaying)
					{
						if (auds[i].volume == origVol[i])
						{
							auds[i].Stop();
						}
						else
						{
							auds[i].volume = Mathf.MoveTowards(auds[i].volume, origVol[i], Time.deltaTime * speed);
						}
					}
				}
				return;
			}
			AudioSource[] array = auds;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource.isPlaying)
				{
					if (audioSource.volume <= 0f)
					{
						audioSource.Stop();
					}
					else
					{
						audioSource.volume -= Time.deltaTime * speed;
					}
				}
			}
		}
		else
		{
			if (!distance)
			{
				return;
			}
			if (fadeIn)
			{
				for (int k = 0; k < auds.Length; k++)
				{
					if (Vector3.Distance(base.transform.position, player.transform.position) > maxDistance)
					{
						auds[k].volume = 0f;
					}
					else
					{
						auds[k].volume = Mathf.Pow((Mathf.Sqrt(maxDistance) - Mathf.Sqrt(Vector3.Distance(base.transform.position, player.transform.position))) / Mathf.Sqrt(maxDistance), 2f) * origVol[k];
					}
				}
			}
			else
			{
				for (int l = 0; l < auds.Length; l++)
				{
					auds[l].volume = Vector3.Distance(base.transform.position, player.transform.position) / maxDistance * origVol[l];
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			BeginFade();
		}
	}

	private void BeginFade()
	{
		fading = true;
		AudioSource[] array = auds;
		for (int i = 0; i < array.Length; i++)
		{
			GetMusicVolume component = array[i].GetComponent<GetMusicVolume>();
			if ((bool)component)
			{
				Object.Destroy(component);
			}
		}
	}
}
