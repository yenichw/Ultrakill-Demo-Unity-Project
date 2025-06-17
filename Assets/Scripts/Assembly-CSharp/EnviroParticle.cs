using UnityEngine;

public class EnviroParticle : MonoBehaviour
{
	private ParticleSystem part;

	private void Start()
	{
		part = GetComponent<ParticleSystem>();
		if (PlayerPrefs.GetInt("EnvPar", 0) == 1)
		{
			part.Stop();
			part.Clear();
		}
	}

	private void OnEnable()
	{
		CheckEnviroParticles();
	}

	public void CheckEnviroParticles()
	{
		if (part == null)
		{
			part = GetComponent<ParticleSystem>();
		}
		if (PlayerPrefs.GetInt("EnvPar", 0) == 1)
		{
			part.Stop();
			part.Clear();
		}
		else
		{
			part.Play();
		}
	}
}
