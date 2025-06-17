using UnityEngine;

public class FinalDoor : MonoBehaviour
{
	public Door[] doors;

	public GameObject doorLight;

	public bool startOpen;

	public Material[] offMaterials;

	public Material[] onMaterials;

	public bool levelNameOnOpen;

	private void Start()
	{
		if (doorLight == null)
		{
			doorLight = GetComponentInChildren<Light>().gameObject;
		}
		doorLight.SetActive(value: false);
		if (startOpen)
		{
			Open();
		}
	}

	public void Open()
	{
		GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().ArenaMusicEnd();
		Invoke("OpenDoors", 1f);
		GetComponent<AudioSource>().Play();
		doorLight.SetActive(value: true);
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			int material = GetMaterial(meshRenderer);
			meshRenderer.sharedMaterial = onMaterials[material];
		}
	}

	private int GetMaterial(MeshRenderer mr)
	{
		bool flag = false;
		int num = 0;
		while (!flag)
		{
			if (mr.sharedMaterial == offMaterials[num])
			{
				flag = true;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	private void OpenDoors()
	{
		Door[] array = doors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
		if (levelNameOnOpen)
		{
			Invoke("LevelNameGo", 1f);
		}
	}

	private void LevelNameGo()
	{
		Object.FindObjectOfType<LevelNamePopup>().NameAppear();
	}
}
