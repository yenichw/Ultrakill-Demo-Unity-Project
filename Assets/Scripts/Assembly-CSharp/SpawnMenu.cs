using UnityEngine;
using UnityEngine.UI;

public class SpawnMenu : MonoBehaviour
{
	[SerializeField]
	private SpawnMenuSectionReference sectionReference;

	[SerializeField]
	private SpawnableObjectsDatabase objects;

	[HideInInspector]
	public DebugArm arm;

	private void Awake()
	{
		CreateButtons(objects.enemies, "ENEMIES");
		CreateButtons(objects.objects, "ITEMS");
		sectionReference.gameObject.SetActive(value: false);
	}

	private void CreateButtons(SpawnableObject[] list, string sectionName)
	{
		SpawnMenuSectionReference spawnMenuSectionReference = Object.Instantiate(sectionReference, sectionReference.transform.parent);
		spawnMenuSectionReference.sectionName.text = sectionName;
		foreach (SpawnableObject spawnableObject in list)
		{
			spawnMenuSectionReference.buttonBackgroundImage.color = spawnableObject.backgroundColor;
			spawnMenuSectionReference.buttonForegroundImage.sprite = spawnableObject.gridIcon;
			Button button = Object.Instantiate(spawnMenuSectionReference.button, spawnMenuSectionReference.grid.transform, worldPositionStays: false);
			SpawnableObject spawnableObj = spawnableObject;
			button.onClick.AddListener(delegate
			{
				SelectObject(spawnableObj);
			});
		}
		spawnMenuSectionReference.button.gameObject.SetActive(value: false);
	}

	private void SelectObject(SpawnableObject obj)
	{
		arm.PreviewObject(obj);
	}
}
