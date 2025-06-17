using UnityEngine;

public class SpiderLegPos : MonoBehaviour
{
	public GameObject childLeg;

	public SpiderLeg sl;

	private bool movingLeg;

	private RaycastHit hit;

	public bool backLeg;

	public LayerMask lmask;

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == childLeg)
		{
			MoveLeg();
			movingLeg = true;
		}
	}

	private void Update()
	{
		if (movingLeg)
		{
			childLeg.transform.position = Vector3.MoveTowards(childLeg.transform.position, base.transform.position, Time.deltaTime * 50f);
			if (childLeg.transform.position == base.transform.position)
			{
				movingLeg = false;
			}
		}
	}

	private void MoveLeg()
	{
		if (!backLeg)
		{
			Physics.Raycast(base.transform.position, base.transform.up * -1f + (base.transform.forward + base.transform.right * -1f) * Random.Range(0f, 0.5f), out hit, 35f);
		}
		else
		{
			Physics.Raycast(base.transform.position, base.transform.up * -1f + (base.transform.forward + base.transform.right) * -1f * Random.Range(0f, 0.5f), out hit, 35f);
		}
		if (hit.transform != null && (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Floor" || hit.transform.gameObject.tag == "Moving"))
		{
			sl.target = hit.point;
		}
		else if (hit.transform == null)
		{
			Physics.Raycast(base.transform.position, base.transform.up * -1f, out hit, 35f);
			if (hit.transform != null)
			{
				MoveLeg();
			}
		}
	}
}
