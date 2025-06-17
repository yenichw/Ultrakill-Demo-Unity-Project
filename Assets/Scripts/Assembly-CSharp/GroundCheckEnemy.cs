using System.Collections.Generic;
using UnityEngine;

public class GroundCheckEnemy : MonoBehaviour
{
	public bool onGround;

	public List<Collider> cols = new List<Collider>();

	private List<Collider> toRemove = new List<Collider>();

	public bool dontCheckTags;

	private void Start()
	{
		cols.Clear();
		toRemove.Clear();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.enabled = false;
		component.enabled = true;
		CheckCols();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 16) && ((other.gameObject.layer != 16 && dontCheckTags) || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor" || other.gameObject.tag == "Moving"))
		{
			onGround = true;
			cols.Add(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 16) && ((other.gameObject.layer != 16 && dontCheckTags) || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor" || other.gameObject.tag == "Moving") && cols.Contains(other))
		{
			cols.Remove(other);
		}
	}

	private void CheckCols()
	{
		CheckColsOnce();
		Invoke("CheckCols", 0.1f);
	}

	private void CheckColsOnce()
	{
		if (cols.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < cols.Count; i++)
			{
				if (cols[i] == null || !cols[i].gameObject.activeInHierarchy)
				{
					toRemove.Add(cols[i]);
				}
				else if (cols[i].gameObject.tag != "Slippery" && (cols[i].gameObject.layer == 8 || cols[i].gameObject.layer == 24 || cols[i].gameObject.layer == 16) && ((cols[i].gameObject.layer != 16 && dontCheckTags) || cols[i].gameObject.tag == "Floor" || cols[i].gameObject.tag == "Wall" || cols[i].gameObject.tag == "GlassFloor" || cols[i].gameObject.tag == "Moving"))
				{
					flag = true;
				}
			}
			if (flag)
			{
				onGround = true;
			}
			else
			{
				onGround = false;
			}
		}
		else
		{
			onGround = false;
		}
		if (toRemove.Count <= 0)
		{
			return;
		}
		foreach (Collider item in toRemove)
		{
			cols.Remove(item);
		}
		toRemove.Clear();
	}

	public Vector3 ClosestPoint()
	{
		CheckColsOnce();
		if (cols.Count > 0)
		{
			Vector3 result = base.transform.position;
			float num = 999f;
			for (int i = 0; i < cols.Count; i++)
			{
				Vector3 vector = cols[i].ClosestPoint(base.transform.position);
				if (Vector3.Distance(vector, base.transform.position) < num)
				{
					result = vector;
					num = Vector3.Distance(vector, base.transform.position);
				}
			}
			return result;
		}
		return base.transform.position;
	}
}
