using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public bool onGround;

	public bool canJump;

	public bool test;

	public bool heavyFall;

	public GameObject shockwave;

	public bool enemy;

	public float superJumpChance;

	public float extraJumpChance;

	private NewMovement nmov;

	private Collider col;

	private Collider currentCol;

	private Collider currentEnemyCol;

	private void Update()
	{
		if (superJumpChance > 0f)
		{
			superJumpChance = Mathf.MoveTowards(superJumpChance, 0f, Time.deltaTime);
			if (superJumpChance == 0f)
			{
				if (shockwave != null && nmov.stillHolding)
				{
					if (nmov.boostCharge >= 200f)
					{
						Object.Instantiate(shockwave, base.transform.position, Quaternion.identity).GetComponent<PhysicalShockwave>().force *= nmov.slamForce * 2.25f;
						if (!nmov.asscon.majorEnabled || !nmov.asscon.infiniteStamina)
						{
							nmov.boostCharge -= 200f;
						}
						nmov.cc.CameraShake(0.75f);
					}
					else
					{
						Object.Instantiate(nmov.staminaFailSound);
					}
				}
				extraJumpChance = 0.15f;
				nmov.stillHolding = false;
			}
		}
		if (extraJumpChance > 0f)
		{
			extraJumpChance = Mathf.MoveTowards(extraJumpChance, 0f, Time.deltaTime);
			if (extraJumpChance <= 0f)
			{
				nmov.slamForce = 0f;
			}
		}
		if (onGround && (currentCol == null || !currentCol.enabled || currentCol.gameObject.layer == 17))
		{
			onGround = false;
		}
		if (canJump && (currentEnemyCol == null || !currentEnemyCol.gameObject.activeInHierarchy || Vector3.Distance(base.transform.position, currentEnemyCol.transform.position) > 40f))
		{
			canJump = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 11 || other.gameObject.layer == 26) && (!enemy || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor"))
		{
			if (currentCol == null)
			{
				currentCol = other;
			}
			onGround = true;
			canJump = false;
		}
		else if (other.gameObject.tag != "Slippery" && other.gameObject.layer == 12)
		{
			currentEnemyCol = other;
			canJump = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 11 || other.gameObject.layer == 26) && (!enemy || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor"))
		{
			if (onGround)
			{
				onGround = false;
			}
		}
		else if (other.gameObject.tag != "Slippery" && other.gameObject.layer == 12)
		{
			canJump = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 11 || other.gameObject.layer == 26) && (!enemy || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor"))
		{
			currentCol = other;
		}
		if (!heavyFall)
		{
			return;
		}
		if (other.gameObject.layer == 10 || other.gameObject.layer == 11)
		{
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			if (eid != null)
			{
				eid.hitter = "ground slam";
				eid.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position) * 5000f, other.transform.position, 2f, tryForExplode: true, 0f);
				if (!eid.exploded)
				{
					heavyFall = false;
				}
			}
		}
		else if (other.gameObject.layer == 8 || other.gameObject.layer == 24)
		{
			Breakable component = other.gameObject.GetComponent<Breakable>();
			if (nmov == null)
			{
				nmov = GetComponentInParent<NewMovement>();
			}
			if (component != null && component.weak)
			{
				component.Break();
			}
			else
			{
				heavyFall = false;
			}
			superJumpChance = 0.075f;
		}
	}
}
