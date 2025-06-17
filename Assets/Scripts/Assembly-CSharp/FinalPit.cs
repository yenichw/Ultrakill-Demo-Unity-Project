using UnityEngine;

public class FinalPit : MonoBehaviour
{
	private NewMovement nmov;

	public float rotation;

	private StatsManager sm;

	private Rigidbody rb;

	private bool rotationReady;

	private GameObject player;

	private bool infoSent;

	public bool rankless;

	public bool secondPit;

	public string targetLevelName;

	private int levelNumber;

	public bool musicFadeOut;

	private void Start()
	{
		sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		player = GameObject.FindWithTag("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player)
		{
			if (musicFadeOut)
			{
				sm.GetComponentInChildren<MusicManager>().off = true;
			}
			nmov = other.gameObject.GetComponent<NewMovement>();
			rb = nmov.rb;
			nmov.activated = false;
			nmov.cc.activated = false;
			sm.HideShit();
			sm.StopTimer();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject == player))
		{
			return;
		}
		if (nmov == null)
		{
			nmov = other.gameObject.GetComponent<NewMovement>();
			rb = nmov.rb;
			if (nmov.dead)
			{
				return;
			}
		}
		else if (nmov.dead)
		{
			return;
		}
		if (other.transform.position.x != base.transform.position.x || other.transform.position.z != base.transform.position.z)
		{
			Vector3 vector = new Vector3(base.transform.position.x, other.transform.position.y, base.transform.position.z);
			float num = Vector3.Distance(other.transform.position, vector);
			other.transform.position = Vector3.MoveTowards(other.transform.position, vector, 1f + num * Time.deltaTime);
			rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
		}
		if (nmov.cc.rotationX != 0.001f)
		{
			if (nmov.cc.rotationX > 0.001f)
			{
				nmov.cc.rotationX = nmov.cc.rotationX - ((nmov.cc.rotationX + Mathf.Sign(nmov.cc.rotationX)) * Time.deltaTime + 1f);
			}
			else if (nmov.cc.rotationX < 0.001f)
			{
				nmov.cc.rotationX = nmov.cc.rotationX - ((nmov.cc.rotationX + Mathf.Sign(nmov.cc.rotationX)) * Time.deltaTime - 1f);
			}
			if (nmov.cc.rotationX < 1f && nmov.cc.rotationX > -1f)
			{
				nmov.cc.rotationX = 0.001f;
			}
		}
		if (nmov.cc.rotationY != rotation)
		{
			while (nmov.cc.rotationY < rotation - 180f)
			{
				nmov.cc.rotationY += 360f;
			}
			while (nmov.cc.rotationY > rotation + 180f)
			{
				nmov.cc.rotationY -= 360f;
			}
			if (nmov.cc.rotationY > rotation)
			{
				nmov.cc.rotationY = nmov.cc.rotationY - ((nmov.cc.rotationY - rotation) * Time.deltaTime + 1f);
				if (nmov.cc.rotationY - rotation < 0.2f)
				{
					nmov.cc.rotationY = rotation;
				}
			}
			else if (nmov.cc.rotationY < rotation)
			{
				nmov.cc.rotationY = nmov.cc.rotationY + ((rotation - nmov.cc.rotationY) * Time.deltaTime + 1f);
				if (rotation - nmov.cc.rotationY < 0.2f)
				{
					nmov.cc.rotationY = rotation;
				}
			}
		}
		if (nmov.cc.rotationX == 0.001f && nmov.cc.rotationY == rotation)
		{
			rotationReady = true;
		}
		if (!rotationReady || infoSent)
		{
			return;
		}
		infoSent = true;
		if (!rankless)
		{
			levelNumber = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().levelNumber;
			GameProgressSaver.SaveProgress(levelNumber + 1);
			if (!Application.CanStreamedLevelBeLoaded(targetLevelName))
			{
				targetLevelName = "Main Menu";
			}
			FinalRank fr = sm.fr;
			fr.targetLevelName = targetLevelName;
			fr.finalPitPos = base.transform.position;
			sm.SendInfo();
			if (secondPit)
			{
				fr.reachedSecondPit = true;
			}
		}
		else if (secondPit)
		{
			GameProgressSaver.SetTutorial(beat: true);
			FinalRank fr2 = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().fr;
			fr2.gameObject.SetActive(value: true);
			fr2.finalPitPos = base.transform.position;
			fr2.RanklessNextLevel(targetLevelName);
		}
	}
}
