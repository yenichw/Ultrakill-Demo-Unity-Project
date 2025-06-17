using UnityEngine;

public class StatueFake : MonoBehaviour
{
	private Animator anim;

	private AudioSource aud;

	private ParticleSystem part;

	public GameObject[] toDeactivate;

	public GameObject enemyObject;

	public bool spawn;

	public GameObject[] toActivate;

	public bool quickSpawn;

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		aud = GetComponentInChildren<AudioSource>();
		part = GetComponentInChildren<ParticleSystem>();
		StatueIntroChecker statueIntroChecker = Object.FindObjectOfType<StatueIntroChecker>();
		if (statueIntroChecker != null)
		{
			if (statueIntroChecker.beenSeen)
			{
				quickSpawn = true;
			}
			else if (!quickSpawn)
			{
				statueIntroChecker.beenSeen = true;
			}
		}
		if (quickSpawn)
		{
			anim.speed = 1.5f;
		}
	}

	public void Activate()
	{
		if (anim == null)
		{
			anim = GetComponentInChildren<Animator>();
		}
		if (quickSpawn)
		{
			anim.Play("Awaken", -1, 0.33f);
		}
		else
		{
			Invoke("SlowStart", 3f);
		}
	}

	public void Crack()
	{
		aud.Play();
		part.Play();
	}

	public void Done()
	{
		GameObject[] array = toDeactivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = toActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		if (spawn)
		{
			Object.Instantiate(enemyObject, base.transform.position + base.transform.forward * 4f, base.transform.rotation);
		}
	}

	private void SlowStart()
	{
		anim.SetTrigger("Awaken");
	}
}
