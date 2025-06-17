using UnityEngine;

public class BigDoorOpener : MonoBehaviour
{
	public BigDoor[] bigDoors;

	private void Start()
	{
		BigDoor[] array = bigDoors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
	}

	private void OnEnable()
	{
		BigDoor[] array = bigDoors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
	}

	private void OnDisable()
	{
		BigDoor[] array = bigDoors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Close();
		}
	}
}
