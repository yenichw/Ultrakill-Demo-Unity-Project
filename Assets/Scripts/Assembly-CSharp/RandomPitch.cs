using UnityEngine;

public class RandomPitch : MonoBehaviour
{
	public float defaultPitch;

	public float pitchVariation;

	private void Start()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null)
		{
			if (pitchVariation == 0f)
			{
				component.pitch = Random.Range(0.8f, 1.2f);
			}
			else
			{
				component.pitch = Random.Range(defaultPitch - pitchVariation, defaultPitch + pitchVariation);
			}
			component.Play();
		}
	}
}
