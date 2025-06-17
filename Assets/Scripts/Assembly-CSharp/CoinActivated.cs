using UnityEngine;

public class CoinActivated : MonoBehaviour
{
	public GameObject[] toActivate;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Coin")
		{
			GameObject[] array = toActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			GetComponent<Collider>().enabled = false;
		}
	}
}
