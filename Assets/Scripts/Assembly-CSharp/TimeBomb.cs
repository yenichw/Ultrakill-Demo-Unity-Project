using UnityEngine;

public class TimeBomb : MonoBehaviour
{
	public bool dontStartOnAwake;

	private bool activated;

	public float timer;

	private AudioSource aud;

	public GameObject beepLight;

	public float beeperSize;

	private GameObject beeper;

	private Vector3 origScale;

	public GameObject explosion;

	public bool lowBeeps;

	public bool dontExplode;

	private void Start()
	{
		if (!dontStartOnAwake)
		{
			StartCountdown();
		}
	}

	private void OnDestroy()
	{
		if (!dontExplode && explosion != null)
		{
			Object.Instantiate(explosion, base.transform.position, base.transform.rotation);
		}
	}

	private void Update()
	{
		if (activated)
		{
			timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime);
			if (timer != 0f)
			{
				beeper.transform.localScale = Vector3.Lerp(beeper.transform.localScale, Vector3.zero, Vector3.Distance(beeper.transform.localScale, Vector3.zero) * Time.deltaTime * 0.1f);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void StartCountdown()
	{
		if (!activated)
		{
			activated = true;
			Beep();
		}
	}

	private void Beep()
	{
		if (beeper == null)
		{
			beeper = Object.Instantiate(beepLight, base.transform.position, base.transform.rotation);
			beeper.transform.SetParent(base.transform, worldPositionStays: true);
			aud = beeper.GetComponent<AudioSource>();
			origScale = new Vector3(beeperSize, beeperSize, 1f);
			if (lowBeeps)
			{
				aud.pitch = 0.65f;
			}
		}
		aud.Play();
		beeper.transform.localScale = origScale;
		Invoke("Beep", timer / 6f);
	}
}
