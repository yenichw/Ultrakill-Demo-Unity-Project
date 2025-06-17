using System.Collections.Generic;
using UnityEngine;

public class TimeOfDayChanger : MonoBehaviour
{
	private bool allOff;

	private bool allDone = true;

	public Light[] oldLights;

	public Light[] newLights;

	private List<float> origIntensities = new List<float>();

	public Material oldWalls;

	public Material oldSky;

	public Material newWalls;

	public Material newSky;

	public bool toBattleMusic;

	public bool toBossMusic;

	public bool musicWaitsUntilChange;

	private void Awake()
	{
		if (newLights.Length != 0)
		{
			for (int i = 0; i < newLights.Length; i++)
			{
				origIntensities.Add(newLights[i].intensity);
				newLights[i].intensity = 0f;
				newLights[i].enabled = true;
			}
		}
	}

	private void OnEnable()
	{
		allDone = false;
		if (!musicWaitsUntilChange)
		{
			if (toBattleMusic)
			{
				Object.FindObjectOfType<MusicManager>().ArenaMusicStart();
			}
			else if (toBossMusic)
			{
				Object.FindObjectOfType<MusicManager>().PlayBossMusic();
			}
		}
	}

	private void ChangeMaterials()
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.sharedMaterial == oldWalls)
			{
				meshRenderer.material = newWalls;
			}
			else if (meshRenderer.sharedMaterial == oldSky)
			{
				meshRenderer.material = newSky;
			}
		}
		if (musicWaitsUntilChange)
		{
			if (toBattleMusic)
			{
				Object.FindObjectOfType<MusicManager>().ArenaMusicStart();
			}
			else if (toBossMusic)
			{
				Object.FindObjectOfType<MusicManager>().PlayBossMusic();
			}
		}
	}

	private void Update()
	{
		if (allDone)
		{
			return;
		}
		if (!allOff)
		{
			bool flag = true;
			Light[] array = oldLights;
			foreach (Light light in array)
			{
				if (light.intensity != 0f)
				{
					light.intensity = Mathf.MoveTowards(light.intensity, 0f, Time.deltaTime);
					if (light.intensity != 0f)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				allOff = true;
				ChangeMaterials();
			}
		}
		else if (newLights.Length != 0)
		{
			bool flag2 = true;
			for (int j = 0; j < newLights.Length; j++)
			{
				if (newLights[j].intensity != origIntensities[j])
				{
					newLights[j].intensity = Mathf.MoveTowards(newLights[j].intensity, origIntensities[j], Time.deltaTime);
					if (newLights[j].intensity != origIntensities[j])
					{
						flag2 = false;
					}
				}
			}
			if (flag2)
			{
				allDone = true;
			}
		}
		else
		{
			allDone = true;
		}
	}
}
