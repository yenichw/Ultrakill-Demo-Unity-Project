using UnityEngine;
using UnityEngine.UI;

public class StyleCalculator : MonoBehaviour
{
	public StyleHUD shud;

	private GameObject player;

	private NewMovement nmov;

	public Text airTimeText;

	public float airTime = 1f;

	private Vector3 airTimePos;

	private StatsManager sman;

	private GunControl gc;

	public bool enemiesShot;

	public float multikillTimer;

	public int multikillCount;

	private void Start()
	{
		shud = GetComponent<StyleHUD>();
		player = GameObject.FindWithTag("Player");
		nmov = player.GetComponent<NewMovement>();
		airTimePos = airTimeText.transform.localPosition;
		sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		gc = GameObject.FindWithTag("GunControl").GetComponent<GunControl>();
	}

	private void Update()
	{
		if (!nmov.gc.onGround || nmov.sliding)
		{
			airTime = Mathf.MoveTowards(airTime, 3f, Time.deltaTime * 2f);
			if (!airTimeText.gameObject.activeSelf)
			{
				airTimeText.gameObject.SetActive(value: true);
			}
		}
		else if (!nmov.boost)
		{
			airTime = Mathf.MoveTowards(airTime, 1f, Time.deltaTime * 10f);
			airTimeText.transform.localPosition = airTimePos;
		}
		if (airTime >= 2f && airTime < 3f)
		{
			airTimeText.text = "<color=orange><size=60>x" + airTime.ToString("F2") + "</size></color>";
			airTimeText.transform.localPosition = new Vector3(airTimePos.x + (float)Random.Range(-3, 3), airTimePos.y + (float)Random.Range(-3, 3), airTimePos.z);
		}
		else if (airTime == 3f)
		{
			airTimeText.text = "<color=red><size=72>x" + airTime.ToString("F2") + "</size></color>";
			airTimeText.transform.localPosition = new Vector3(airTimePos.x + (float)Random.Range(-6, 6), airTimePos.y + (float)Random.Range(-6, 6), airTimePos.z);
		}
		else if (airTime == 1f && airTimeText.gameObject.activeSelf)
		{
			airTimeText.gameObject.SetActive(value: false);
		}
		else
		{
			airTimeText.text = "x" + airTime.ToString("F2");
			airTimeText.transform.localPosition = airTimePos;
		}
		if (multikillTimer > 0f)
		{
			multikillTimer -= Time.deltaTime * 10f;
		}
		else if (multikillCount != 0)
		{
			multikillTimer = 0f;
			multikillCount = 0;
		}
	}

	public void HitCalculator(string hitter, string enemyType, string hitLimb, bool dead, GameObject sender)
	{
		switch (hitter)
		{
		case "punch":
			if (dead)
			{
				if (hitLimb == "head" || hitLimb == "limb")
				{
					AddPoints(60, "CRITICAL PUNCH");
				}
				else if (enemyType == "spider")
				{
					AddPoints(150, "BIG FISTKILL");
				}
				else
				{
					AddPoints(30, "KILL");
				}
				gc.AddKill();
			}
			else if (enemyType == "spider")
			{
				AddPoints(60, "DISRESPECT");
			}
			else
			{
				AddPoints(20, "");
			}
			break;
		case "ground slam":
			if (dead)
			{
				AddPoints(60, "GROUND SLAM");
				gc.AddKill();
			}
			else
			{
				AddPoints(20, "");
			}
			break;
		case "revolver":
			enemiesShot = true;
			if (dead)
			{
				if (hitLimb == "head" && enemyType == "spider")
				{
					AddPoints(150, "BIG HEADSHOT");
				}
				else if (hitLimb == "head")
				{
					AddPoints(80, "HEADSHOT");
				}
				else if (hitLimb == "limb")
				{
					AddPoints(60, "LIMB HIT");
				}
				else if (enemyType == "spider")
				{
					AddPoints(100, "BIG KILL");
				}
				else
				{
					AddPoints(30, "KILL");
				}
				gc.AddKill();
			}
			else if (hitLimb == "head")
			{
				AddPoints(25, "");
			}
			else if (hitLimb == "limb")
			{
				AddPoints(15, "");
			}
			else
			{
				AddPoints(10, "");
			}
			break;
		case "shotgun":
			enemiesShot = true;
			if (dead)
			{
				if (enemyType == "spider")
				{
					AddPoints(100, "BIG KILL");
				}
				else
				{
					AddPoints(45, "KILL");
				}
				gc.AddKill();
			}
			else
			{
				AddPoints(4, "");
			}
			break;
		case "shotgunzone":
			enemiesShot = true;
			if (dead)
			{
				if (enemyType == "spider")
				{
					AddPoints(125, "BIG KILL");
				}
				else
				{
					AddPoints(100, "OVERKILL");
				}
				gc.AddKill();
			}
			break;
		case "nail":
			enemiesShot = true;
			if (dead)
			{
				if (enemyType == "spider")
				{
					AddPoints(100, "BIG KILL");
				}
				else
				{
					AddPoints(45, "KILL");
				}
				gc.AddKill();
			}
			else
			{
				AddPoints(2, "");
			}
			break;
		case "railcannon":
			enemiesShot = true;
			if (dead)
			{
				if (enemyType == "spider")
				{
					AddPoints(100, "BIG KILL");
				}
				else
				{
					AddPoints(45, "KILL");
				}
				gc.AddKill();
			}
			else
			{
				AddPoints(15, "");
			}
			break;
		case "projectile":
			if (dead)
			{
				AddPoints(250, "FRIENDLY FIRE");
				gc.AddKill();
			}
			else
			{
				AddPoints(200, "FRIENDLY FIRE");
			}
			break;
		case "ffexplosion":
			if (dead)
			{
				AddPoints(250, "FRIENDLY FIRE");
				AddPoints(0, "EXPLODED");
				gc.AddKill();
			}
			else
			{
				AddPoints(200, "FRIENDLY FIRE");
			}
			break;
		case "explosion":
			if (dead)
			{
				AddPoints(45, "EXPLODED");
				gc.AddKill();
			}
			else
			{
				AddPoints(2, "");
			}
			break;
		case "fire":
			if (dead)
			{
				AddPoints(20, "FRIED");
				gc.AddKill();
			}
			else
			{
				AddPoints(2, "");
			}
			break;
		}
		if (dead && hitter != "secret")
		{
			multikillCount++;
			multikillTimer = 5f;
			switch (multikillCount)
			{
			case 2:
				shud.AddPoints(25, "<color=orange>DOUBLE KILL</color>");
				break;
			case 3:
				shud.AddPoints(50, "<color=orange>TRIPLE KILL</color>");
				break;
			default:
				shud.AddPoints(100, "<color=orange>MULTIKILL x" + multikillCount + "</color>");
				break;
			case 0:
			case 1:
				break;
			}
		}
	}

	private void AddPoints(int points, string pointName)
	{
		int num = Mathf.RoundToInt((float)points * airTime - (float)points);
		shud.AddPoints(points + num, pointName);
	}
}
