using UnityEngine;

public class MenuEsc : MonoBehaviour
{
	public GameObject previousPage;

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			previousPage.SetActive(value: true);
			base.gameObject.SetActive(value: false);
		}
	}
}
