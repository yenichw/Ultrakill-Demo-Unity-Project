using UnityEngine;

public class DeathZone : MonoBehaviour
{
	private NewMovement pm;

	private AudioSource aud;

	public GameObject sawSound;

	private bool crusher;

	public string deathType;

	public bool dontExplode;

	public bool aliveOnly;

	public affectedSubjects affected;

	private bool playerAffected;

	private bool enemyAffected;

	public bool notInstakill;

	public Vector3 respawnTarget;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
		switch (affected)
		{
		case affectedSubjects.All:
			enemyAffected = true;
			playerAffected = true;
			break;
		case affectedSubjects.EnemiesOnly:
			enemyAffected = true;
			playerAffected = false;
			break;
		case affectedSubjects.PlayerOnly:
			enemyAffected = false;
			playerAffected = true;
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && playerAffected)
		{
			if (!notInstakill)
			{
				if (pm == null)
				{
					pm = other.GetComponent<NewMovement>();
				}
				pm.GetHurt(999999, invincible: false, 1f, explosion: false, instablack: true);
				if (sawSound != null)
				{
					Object.Instantiate(sawSound, other.transform.position, Quaternion.identity);
				}
				base.enabled = false;
				return;
			}
			if (pm == null)
			{
				pm = other.GetComponent<NewMovement>();
			}
			if (pm.hp > 0)
			{
				if (pm.hp > 50)
				{
					pm.GetHurt(50, invincible: true);
				}
				else
				{
					pm.GetHurt(pm.hp - 1, invincible: true);
				}
				if (sawSound != null)
				{
					Object.Instantiate(sawSound, other.transform.position, Quaternion.identity);
				}
				other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
				StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
				_ = Vector3.zero;
				if (respawnTarget != Vector3.zero)
				{
					other.transform.position = respawnTarget + Vector3.up * 1.25f;
				}
				else if (component.currentCheckPoint != null)
				{
					other.transform.position = component.currentCheckPoint.transform.position + Vector3.up * 1.25f;
				}
				else
				{
					other.transform.position = component.spawnPos;
				}
			}
		}
		else if ((other.gameObject.tag == "Enemy" || other.gameObject.layer == 10) && enemyAffected)
		{
			EnemyIdentifier enemyIdentifier = other.gameObject.GetComponentInParent<EnemyIdentifier>();
			if (enemyIdentifier == null)
			{
				EnemyIdentifierIdentifier component2 = other.gameObject.GetComponent<EnemyIdentifierIdentifier>();
				if (component2 != null && component2.eid != null)
				{
					enemyIdentifier = component2.eid;
				}
			}
			if (GetComponentInParent<Piston>() != null)
			{
				crusher = true;
			}
			if (enemyIdentifier != null && ((!dontExplode && !aliveOnly && !enemyIdentifier.exploded) || !enemyIdentifier.dead))
			{
				if (sawSound != null)
				{
					Object.Instantiate(sawSound, other.transform.position, Quaternion.identity);
				}
				enemyIdentifier.hitter = "deathzone";
				if (!enemyIdentifier.dead)
				{
					Object.FindObjectOfType<StyleHUD>().AddPoints(80, deathType);
				}
				else
				{
					Object.FindObjectOfType<StyleHUD>().AddPoints(20, "<color=grey>" + deathType + "</color>");
				}
				if (!dontExplode)
				{
					enemyIdentifier.Explode();
				}
				else
				{
					enemyIdentifier.InstaKill();
				}
			}
		}
		else if (other.gameObject.tag == "Coin")
		{
			other.GetComponent<Coin>().GetDeleted();
		}
	}
}
