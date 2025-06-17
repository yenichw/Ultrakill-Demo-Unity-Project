using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dHitConnector : MonoBehaviour
	{
		private class Link
		{
			public object Owner;

			public Vector3 WorldPosition;

			public float Pressure;

			public float Age;

			public bool Preview;
		}

		[SerializeField]
		protected bool connectHits;

		[SerializeField]
		private float hitSpacing;

		[SerializeField]
		private int hitLimit = 30;

		[NonSerialized]
		private List<Link> links = new List<Link>();

		[NonSerialized]
		private static Stack<Link> linkPool = new Stack<Link>();

		[NonSerialized]
		protected P3dHitCache hitCache = new P3dHitCache();

		public bool ConnectHits
		{
			get
			{
				return connectHits;
			}
			set
			{
				connectHits = value;
			}
		}

		public float HitSpacing
		{
			get
			{
				return hitSpacing;
			}
			set
			{
				hitSpacing = value;
			}
		}

		public int HitLimit
		{
			get
			{
				return hitLimit;
			}
			set
			{
				hitLimit = value;
			}
		}

		public P3dHitCache HitCache => hitCache;

		[ContextMenu("Clear Hit Cache")]
		public void ClearHitCache()
		{
			hitCache.Clear();
		}

		[ContextMenu("Reset Hit Positions")]
		public void ResetPosition()
		{
			Vector3 position = base.transform.position;
			for (int num = links.Count - 1; num >= 0; num--)
			{
				links[num].WorldPosition = position;
			}
		}

		protected void BreakHits(object owner)
		{
			for (int num = links.Count - 1; num >= 0; num--)
			{
				Link link = links[num];
				if (link.Owner == owner)
				{
					links.RemoveAt(num);
					linkPool.Push(link);
					break;
				}
			}
		}

		protected void DispatchHits(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure, object owner)
		{
			if (connectHits && owner != null)
			{
				Link link = null;
				Vector3 vector = worldPosition;
				if (TryGetLink(owner, ref link))
				{
					if (link.Preview == preview)
					{
						if (hitSpacing > 0f)
						{
							vector = link.WorldPosition;
							int num = Mathf.FloorToInt(Vector3.Distance(link.WorldPosition, worldPosition) / hitSpacing);
							if (num > hitLimit)
							{
								num = hitLimit;
							}
							for (int i = 0; i < num; i++)
							{
								vector = Vector3.MoveTowards(vector, worldPosition, hitSpacing);
								hitCache.InvokeLines(base.gameObject, preview, priority, link.WorldPosition, vector, link.Pressure, pressure);
								link.WorldPosition = vector;
							}
						}
						else
						{
							hitCache.InvokeLines(base.gameObject, preview, priority, link.WorldPosition, worldPosition, link.Pressure, pressure);
						}
					}
					else
					{
						hitCache.InvokePoints(base.gameObject, preview, priority, collider, worldPosition, worldRotation, pressure);
					}
				}
				else
				{
					link = ((linkPool.Count > 0) ? linkPool.Pop() : new Link());
					link.Owner = owner;
					links.Add(link);
					hitCache.InvokePoints(base.gameObject, preview, priority, collider, worldPosition, worldRotation, pressure);
				}
				link.WorldPosition = vector;
				link.Pressure = pressure;
				link.Preview = preview;
			}
			else
			{
				hitCache.InvokePoints(base.gameObject, preview, priority, collider, worldPosition, worldRotation, pressure);
			}
		}

		protected virtual void Update()
		{
			for (int num = links.Count - 1; num >= 0; num--)
			{
				Link link = links[num];
				link.Age += Time.deltaTime;
				if (link.Age > 1f)
				{
					link.Age = 0f;
					links.RemoveAt(num);
					linkPool.Push(link);
				}
			}
		}

		private bool TryGetLink(object owner, ref Link link)
		{
			for (int num = links.Count - 1; num >= 0; num--)
			{
				link = links[num];
				link.Age = 0f;
				if (link.Owner == owner)
				{
					return true;
				}
			}
			return false;
		}
	}
}
