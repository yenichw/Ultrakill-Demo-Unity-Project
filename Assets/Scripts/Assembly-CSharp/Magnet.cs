using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
	private List<Rigidbody> affectedRbs = new List<Rigidbody>();

	private List<Rigidbody> removeRbs = new List<Rigidbody>();

	private List<EnemyIdentifier> eids = new List<EnemyIdentifier>();

	private List<Rigidbody> eidRbs = new List<Rigidbody>();

	public List<EnemyIdentifier> ignoredEids = new List<EnemyIdentifier>();

	public bool onEnemy;

	private SphereCollider col;

	public float strength;

	private LayerMask lmask;

	private RaycastHit rhit;

	private void Start()
	{
		col = GetComponent<SphereCollider>();
		lmask = (int)lmask | 0x400;
		lmask = (int)lmask | 0x800;
	}

	private void OnDestroy()
	{
		Launch();
	}

	public void Launch()
	{
		foreach (Rigidbody affectedRb in affectedRbs)
		{
			if (affectedRb != null)
			{
				affectedRb.velocity = Vector3.zero;
				if (Physics.SphereCast(new Ray(affectedRb.transform.position, affectedRb.transform.position - base.transform.position), 20f, out rhit, 100f, lmask))
				{
					affectedRb.AddForce((rhit.point - affectedRb.transform.position).normalized * strength * 10f);
				}
				else
				{
					affectedRb.AddForce((base.transform.position - affectedRb.transform.position).normalized * strength * -10f);
				}
				Nail component = affectedRb.GetComponent<Nail>();
				if (component != null)
				{
					component.MagnetRelease(this);
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 14 && other.gameObject.tag == "Metal")
		{
			Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
			if (component != null && !affectedRbs.Contains(component))
			{
				affectedRbs.Add(component);
				Nail component2 = other.GetComponent<Nail>();
				if (component2 != null)
				{
					component2.MagnetCaught(this);
				}
			}
		}
		else
		{
			if (other.gameObject.layer != 12)
			{
				return;
			}
			EnemyIdentifier component3 = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component3 != null && !component3.bigEnemy && !eids.Contains(component3) && !ignoredEids.Contains(component3))
			{
				Rigidbody component4 = component3.GetComponent<Rigidbody>();
				if (component4 != null)
				{
					eids.Add(component3);
					eidRbs.Add(component4);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 14 && other.gameObject.tag == "Metal")
		{
			Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
			if (component != null && affectedRbs.Contains(component))
			{
				affectedRbs.Remove(component);
				Nail component2 = component.GetComponent<Nail>();
				if (component2 != null)
				{
					component2.MagnetRelease(this);
				}
			}
		}
		else if (other.gameObject.layer == 12)
		{
			EnemyIdentifier component3 = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component3 != null && eids.Contains(component3))
			{
				int index = eids.IndexOf(component3);
				eids.RemoveAt(index);
				eidRbs.RemoveAt(index);
			}
		}
	}

	private void Update()
	{
		if (affectedRbs.Count > 0)
		{
			foreach (Rigidbody affectedRb in affectedRbs)
			{
				if (affectedRb != null)
				{
					affectedRb.AddForce((base.transform.position - affectedRb.transform.position) * ((col.radius - Vector3.Distance(affectedRb.transform.position, base.transform.position)) / col.radius * strength * 50f * Time.deltaTime));
				}
				else
				{
					removeRbs.Add(affectedRb);
				}
			}
			if (removeRbs.Count > 0)
			{
				foreach (Rigidbody removeRb in removeRbs)
				{
					affectedRbs.Remove(removeRb);
				}
				removeRbs.Clear();
			}
		}
		if (eids.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < eids.Count; i++)
		{
			if (eids[i] != null && eidRbs[i] != null && !ignoredEids.Contains(eids[i]))
			{
				if (eids[i].nailsAmount > 0 && !eidRbs[i].isKinematic)
				{
					eids[i].useBrakes = false;
					eids[i].magneted = true;
					eidRbs[i].AddForce((base.transform.position - eidRbs[i].transform.position).normalized * ((col.radius - Vector3.Distance(eidRbs[i].transform.position, base.transform.position)) / col.radius * strength * (float)eids[i].nailsAmount * 5f * Time.deltaTime));
				}
			}
			else
			{
				eids.RemoveAt(i);
				eidRbs.RemoveAt(i);
			}
		}
	}
}
