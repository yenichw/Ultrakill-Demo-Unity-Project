using UnityEngine;

public class ActivateOnSoundEnd : MonoBehaviour
{
	private AudioSource aud;

	private bool hasStarted;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (aud.isPlaying)
		{
			hasStarted = true;
		}
		if (hasStarted && !aud.isPlaying && aud.time == 0f)
		{
			hasStarted = false;
			GameObject[] array = toActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = toDisactivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
