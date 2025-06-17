using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
	private int currentStep;

	public bool onGround;

	public AudioClip[] footsteps;

	private AudioSource aud;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	public void Footstep()
	{
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		else if (onGround)
		{
			currentStep = Random.Range(0, footsteps.Length);
			aud.clip = footsteps[currentStep];
			aud.Play();
		}
	}
}
