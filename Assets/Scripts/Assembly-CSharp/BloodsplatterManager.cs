using System.Collections.Generic;
using UnityEngine;

public class BloodsplatterManager : MonoBehaviour
{
	public GameObject head;

	public GameObject limb;

	public GameObject body;

	public GameObject small;

	private List<GameObject> heads = new List<GameObject>();

	private List<GameObject> limbs = new List<GameObject>();

	private List<GameObject> bodies = new List<GameObject>();

	private List<GameObject> smalls = new List<GameObject>();

	private int order;

	private Transform goreStore;

	private void Start()
	{
		goreStore = base.transform.GetChild(0);
		Invoke("Refresh", 0.2f);
	}

	private void Refresh()
	{
		switch (order)
		{
		case 0:
			CheckList(heads, head);
			break;
		case 1:
			CheckList(limbs, limb);
			break;
		case 2:
			CheckList(bodies, body);
			break;
		case 3:
			CheckList(smalls, small);
			break;
		}
		if (order < 3)
		{
			order++;
		}
		else
		{
			order = 0;
		}
		Invoke("Refresh", 0.2f);
	}

	private void CheckList(List<GameObject> list, GameObject bs)
	{
		if (list.Count <= 10)
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject item = Object.Instantiate(bs, goreStore);
				list.Add(item);
			}
		}
		else if (list.Count < 20)
		{
			for (int j = list.Count; j < 20; j++)
			{
				GameObject item2 = Object.Instantiate(bs, goreStore);
				list.Add(item2);
			}
		}
	}

	public GameObject GetGore(GoreType got)
	{
		switch (got)
		{
		case GoreType.Head:
			if (heads.Count > 0)
			{
				GameObject gameObject = heads[0];
				heads.Remove(gameObject);
				return gameObject;
			}
			break;
		case GoreType.Limb:
			if (limbs.Count > 0)
			{
				GameObject gameObject = limbs[0];
				limbs.Remove(gameObject);
				return gameObject;
			}
			break;
		case GoreType.Body:
			if (bodies.Count > 0)
			{
				GameObject gameObject = bodies[0];
				bodies.Remove(gameObject);
				return gameObject;
			}
			break;
		case GoreType.Small:
			if (smalls.Count > 0)
			{
				GameObject gameObject = smalls[0];
				smalls.Remove(gameObject);
				return gameObject;
			}
			break;
		}
		return null;
	}
}
