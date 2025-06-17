using UnityEngine;
using UnityEngine.UI;

public class ScanningStuff : MonoBehaviour
{
	private GameObject scanningPanel;

	private GameObject readingPanel;

	public Image meter;

	private float loading;

	private bool active;

	private void OnEnable()
	{
		if (scanningPanel == null)
		{
			scanningPanel = base.transform.GetChild(0).gameObject;
			readingPanel = base.transform.GetChild(1).gameObject;
		}
		if (loading < 1f)
		{
			active = true;
			loading = 0f;
			meter.fillAmount = 0f;
		}
	}

	private void Update()
	{
		if (active)
		{
			loading = Mathf.MoveTowards(loading, 1f, Time.deltaTime / 2f);
			meter.fillAmount = loading;
			if (loading == 1f)
			{
				active = false;
				scanningPanel.SetActive(value: false);
				readingPanel.SetActive(value: true);
				Object.Destroy(this);
			}
		}
	}
}
