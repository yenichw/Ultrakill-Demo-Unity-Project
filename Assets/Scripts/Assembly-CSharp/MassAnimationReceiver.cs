using UnityEngine;

public class MassAnimationReceiver : MonoBehaviour
{
	public GameObject groundBreakEffect;

	public GameObject smallGroundBreakEffect;

	public bool fakeMass;

	public bool otherBossIntro;

	public GameObject realMass;

	public GameObject footstep;

	private Animator anim;

	private StatueIntroChecker sic;

	private bool turnTowards;

	private Transform player;

	private int breaks;

	public void Start()
	{
		anim = GetComponent<Animator>();
		if (fakeMass)
		{
			sic = Object.FindObjectOfType<StatueIntroChecker>();
			if (!otherBossIntro)
			{
				anim.speed = 0f;
			}
		}
	}

	private void Update()
	{
		if (turnTowards)
		{
			Quaternion b = Quaternion.LookRotation(new Vector3(player.position.x, base.transform.position.y, player.position.z) - base.transform.position, Vector3.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * (Quaternion.Angle(base.transform.rotation, b) / 2f + 1f));
		}
	}

	public void GroundBreak()
	{
		Object.Instantiate(groundBreakEffect, base.transform.position, Quaternion.identity);
		breaks++;
		if (breaks == 3)
		{
			player = GameObject.FindWithTag("Player").transform;
			turnTowards = true;
		}
	}

	public void SmallGroundBreak()
	{
		Object.Instantiate(smallGroundBreakEffect, base.transform.position, Quaternion.identity);
	}

	public void SpawnMass()
	{
		if ((bool)sic && !sic.beenSeen)
		{
			sic.beenSeen = true;
		}
		realMass.SetActive(value: true);
		if (otherBossIntro)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Footstep()
	{
		if (anim.GetLayerWeight(1) > 0.5f)
		{
			Object.Instantiate(footstep, base.transform.position, Quaternion.identity);
		}
	}

	public void SkipOnReplay()
	{
		if ((bool)sic && sic.beenSeen)
		{
			SpawnMass();
		}
	}
}
