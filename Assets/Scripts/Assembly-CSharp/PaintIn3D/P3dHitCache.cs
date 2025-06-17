using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dHitCache
	{
		[NonSerialized]
		private bool cached;

		[NonSerialized]
		private List<IHitPoint> hitPoints = new List<IHitPoint>();

		[NonSerialized]
		private List<IHitLine> hitLines = new List<IHitLine>();

		[NonSerialized]
		private List<IHitRaycast> hitRaycasts = new List<IHitRaycast>();

		[NonSerialized]
		private static List<IHit> hits = new List<IHit>();

		public bool Cached => cached;

		public void InvokePoints(GameObject gameObject, bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (!cached)
			{
				Cache(gameObject);
			}
			for (int i = 0; i < hitPoints.Count; i++)
			{
				hitPoints[i].HandleHitPoint(preview, priority, collider, worldPosition, worldRotation, pressure);
			}
		}

		public void InvokeLines(GameObject gameObject, bool preview, int priority, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			if (!cached)
			{
				Cache(gameObject);
			}
			for (int i = 0; i < hitLines.Count; i++)
			{
				hitLines[i].HandleHitLine(preview, priority, worldPositionA, worldPositionB, pressureA, pressureB);
			}
		}

		public void InvokeRaycast(GameObject gameObject, bool preview, int priority, RaycastHit hit, float pressure)
		{
			if (!cached)
			{
				Cache(gameObject);
			}
			for (int i = 0; i < hitRaycasts.Count; i++)
			{
				hitRaycasts[i].HandleHitRaycast(preview, priority, hit, pressure);
			}
		}

		public void Clear()
		{
			cached = false;
			hitPoints.Clear();
			hitLines.Clear();
			hitRaycasts.Clear();
		}

		private void Cache(GameObject gameObject)
		{
			cached = true;
			gameObject.GetComponentsInChildren(hits);
			hitPoints.Clear();
			hitLines.Clear();
			hitRaycasts.Clear();
			for (int i = 0; i < hits.Count; i++)
			{
				IHit hit = hits[i];
				if (hit is IHitPoint item)
				{
					hitPoints.Add(item);
				}
				if (hit is IHitLine item2)
				{
					hitLines.Add(item2);
				}
				if (hit is IHitRaycast item3)
				{
					hitRaycasts.Add(item3);
				}
			}
		}
	}
}
