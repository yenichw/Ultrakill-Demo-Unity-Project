using UnityEngine;

public class Flicker : MonoBehaviour
{
	private Light light;

	public float delay;

	private AudioSource aud;

	private float intensity;

	private float range;

	public bool onlyOnce;

	public bool quickFlicker;

	public float rangeRandomizer;

	public float intensityRandomizer;

	public float timeRandomizer;

	public bool stopAudio;

	private void Awake()
	{
		light = GetComponent<Light>();
		aud = GetComponent<AudioSource>();
		intensity = light.intensity;
		range = light.range;
		light.intensity = 0f;
		light.range = 0f;
		if (timeRandomizer != 0f)
		{
			Invoke("Flickering", delay + Random.Range(0f - timeRandomizer, timeRandomizer));
		}
		else
		{
			Invoke("Flickering", delay);
		}
	}

	private void Flickering()
	{
		if (light.intensity == 0f)
		{
			light.intensity = intensity + Random.Range(0f - intensityRandomizer, intensityRandomizer);
			light.range = range + Random.Range(0f - rangeRandomizer, rangeRandomizer);
			if (aud != null)
			{
				aud.Play();
			}
			if (quickFlicker)
			{
				Invoke("Off", 0.1f);
			}
		}
		else
		{
			light.intensity = 0f;
			if (aud != null && stopAudio)
			{
				aud.Stop();
			}
		}
		if (!onlyOnce)
		{
			if (timeRandomizer != 0f)
			{
				Invoke("Flickering", delay + Random.Range(0f - timeRandomizer, timeRandomizer));
			}
			else
			{
				Invoke("Flickering", delay);
			}
		}
	}

	private void Off()
	{
		light.intensity = 0f;
		if (aud != null && stopAudio)
		{
			aud.Stop();
		}
	}
}
