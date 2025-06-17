using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
	public bool oneTime;

	public bool skippable;

	public bool disableOnExit;

	private bool activated;

	public float delay;

	public GameObject[] toActivate;

	public GameObject[] toDisActivate;

	public bool forEnemies;

	private bool canSkip;

	private void Start()
	{
		if (GetComponent<Collider>() == null)
		{
			Invoke("Activate", delay);
		}
	}

	private void LateUpdate()
	{
		if (canSkip && Input.GetKeyDown(KeyCode.Escape))
		{
			Activate();
			GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>().UnPause();
			Object.Destroy(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((!forEnemies && !activated && other.gameObject.tag == "Player") || (forEnemies && !activated && other.gameObject.tag == "Enemy"))
		{
			if (oneTime)
			{
				activated = true;
			}
			Invoke("Activate", delay);
			if (skippable)
			{
				canSkip = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!disableOnExit || ((forEnemies || activated || !(other.gameObject.tag == "Player")) && (!forEnemies || activated || !(other.gameObject.tag == "Enemy"))))
		{
			return;
		}
		GameObject[] array = toDisActivate;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: true);
			}
		}
		array = toActivate;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(value: false);
			}
		}
		CancelInvoke("Activate");
	}

	private void Activate()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		GameObject[] array = toDisActivate;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		array = toActivate;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(value: true);
			}
		}
		if (skippable)
		{
			canSkip = false;
		}
	}

	private void OnDisable()
	{
		CancelInvoke("Activate");
		if (canSkip)
		{
			canSkip = false;
		}
	}
}
