using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dColor")]
	[AddComponentMenu("Paint in 3D/Examples/Color")]
	public class P3dColor : P3dLinkedBehaviour<P3dColor>
	{
		[SerializeField]
		private class Contribution
		{
			public P3dColorCounter Counter;

			public int Solid;
		}

		[SerializeField]
		private Color color;

		[SerializeField]
		private List<Contribution> contributions;

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public int Total
		{
			get
			{
				int num = 0;
				foreach (P3dColorCounter instance in P3dColorCounter.Instances)
				{
					num += instance.Total;
				}
				return num;
			}
		}

		public int Solid
		{
			get
			{
				int num = 0;
				if (contributions != null)
				{
					for (int num2 = contributions.Count - 1; num2 >= 0; num2--)
					{
						Contribution contribution = contributions[num2];
						if (contribution.Counter != null && contribution.Counter.isActiveAndEnabled)
						{
							num += contribution.Solid;
						}
						else
						{
							contributions.RemoveAt(num2);
						}
					}
				}
				return num;
			}
		}

		public float Ratio
		{
			get
			{
				int total = Total;
				if (total > 0)
				{
					return (float)Solid / (float)total;
				}
				return 0f;
			}
		}

		public void Contribute(P3dColorCounter counter, int solid)
		{
			Contribution contribution = null;
			if (!TryGetContribution(counter, ref contribution))
			{
				if (solid <= 0)
				{
					return;
				}
				contribution = new Contribution();
				contributions.Add(contribution);
				contribution.Counter = counter;
			}
			contribution.Solid = solid;
		}

		private bool TryGetContribution(P3dColorCounter counter, ref Contribution contribution)
		{
			if (contributions == null)
			{
				contributions = new List<Contribution>();
			}
			for (int num = contributions.Count - 1; num >= 0; num--)
			{
				contribution = contributions[num];
				if (contribution.Counter == counter)
				{
					return true;
				}
			}
			return false;
		}
	}
}
