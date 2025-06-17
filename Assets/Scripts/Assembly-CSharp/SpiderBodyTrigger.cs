using UnityEngine;

public class SpiderBodyTrigger : MonoBehaviour
{
	private SpiderBody spbody;

	private bool dead;

	private void Start()
	{
		spbody = base.transform.parent.GetComponentInChildren<SpiderBody>();
	}

	private void Update()
	{
		if (spbody != null)
		{
			base.transform.position = spbody.transform.position;
			base.transform.rotation = spbody.transform.rotation;
			if (spbody.dead && !dead)
			{
				dead = true;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 12)
		{
			if (!spbody)
			{
				spbody = base.transform.parent.GetComponentInChildren<SpiderBody>();
			}
			spbody.TriggerHit(other);
		}
	}
}
