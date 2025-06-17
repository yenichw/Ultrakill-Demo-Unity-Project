using UnityEngine;

public class OutdoorLightMaster : MonoBehaviour
{
	public bool inverse;

	private Light[] outdoorLights;

	[HideInInspector]
	public LayerMask normalMask;

	[HideInInspector]
	public LayerMask playerMask;

	private int requests;

	private void Start()
	{
		outdoorLights = GetComponentsInChildren<Light>();
		if (outdoorLights.Length != 0)
		{
			normalMask = outdoorLights[0].cullingMask;
			LayerMask layerMask = 8192;
			playerMask = (int)normalMask | (int)layerMask;
		}
	}

	private void Update()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time);
	}

	public void AddRequest()
	{
		requests++;
		if (requests != 1)
		{
			return;
		}
		if (inverse)
		{
			Light[] array = outdoorLights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].cullingMask = normalMask;
			}
		}
		else
		{
			Light[] array = outdoorLights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].cullingMask = playerMask;
			}
		}
	}

	public void RemoveRequest()
	{
		requests--;
		if (requests != 0)
		{
			return;
		}
		if (inverse)
		{
			Light[] array = outdoorLights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].cullingMask = playerMask;
			}
		}
		else
		{
			Light[] array = outdoorLights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].cullingMask = normalMask;
			}
		}
	}
}
