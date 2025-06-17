using System;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dHitNearby")]
	[AddComponentMenu("Paint in 3D/Hit/Hit Nearby")]
	public class P3dHitNearby : P3dHitConnector
	{
		public enum PaintType
		{
			Update = 0,
			FixedUpdate = 1
		}

		[SerializeField]
		private PaintType paintIn;

		[SerializeField]
		private float delay = 0.05f;

		[SerializeField]
		private bool preview;

		[SerializeField]
		private int priority;

		[Range(0f, 1f)]
		[SerializeField]
		private float pressure = 1f;

		[NonSerialized]
		private float current;

		[SerializeField]
		private Vector3 lastPosition;

		public PaintType PaintIn
		{
			get
			{
				return paintIn;
			}
			set
			{
				paintIn = value;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			set
			{
				delay = value;
			}
		}

		public bool Preview
		{
			get
			{
				return preview;
			}
			set
			{
				preview = value;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
			}
		}

		public float Pressure
		{
			get
			{
				return pressure;
			}
			set
			{
				pressure = value;
			}
		}

		protected virtual void OnEnable()
		{
			ResetPosition();
		}

		protected virtual void Start()
		{
			ResetPosition();
		}

		protected override void Update()
		{
			base.Update();
			if (preview)
			{
				DispatchHits(preview: true, priority, null, base.transform.position, base.transform.rotation, pressure, this);
			}
			else if (paintIn == PaintType.Update)
			{
				UpdatePaint();
			}
		}

		protected virtual void FixedUpdate()
		{
			if (!preview && paintIn == PaintType.FixedUpdate)
			{
				UpdatePaint();
			}
		}

		private void UpdatePaint()
		{
			current += Time.deltaTime;
			if (delay > 0f)
			{
				if (current >= delay)
				{
					current %= delay;
					DispatchHits(preview: false, priority, null, base.transform.position, base.transform.rotation, pressure, this);
				}
			}
			else
			{
				DispatchHits(preview: false, priority, null, base.transform.position, base.transform.rotation, pressure, this);
			}
		}
	}
}
