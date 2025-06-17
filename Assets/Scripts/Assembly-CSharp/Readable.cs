using UnityEngine;

public class Readable : MonoBehaviour
{
	private GameObject scanPanel;

	private bool pickedUp;

	public void PickUp()
	{
		pickedUp = true;
		Invoke("StartScan", 0.5f);
	}

	public void PutDown()
	{
		if (scanPanel == null)
		{
			scanPanel = Object.FindObjectOfType<GraphicsOptions>().transform.Find("ScanningStuff").gameObject;
		}
		pickedUp = false;
		CancelInvoke("StartScan");
		scanPanel.SetActive(value: false);
	}

	private void StartScan()
	{
		if (scanPanel == null)
		{
			scanPanel = Object.FindObjectOfType<GraphicsOptions>().transform.Find("ScanningStuff").gameObject;
		}
		scanPanel.SetActive(value: true);
	}
}
