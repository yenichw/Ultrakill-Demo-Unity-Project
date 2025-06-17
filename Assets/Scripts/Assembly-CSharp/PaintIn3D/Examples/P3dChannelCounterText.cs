using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PaintIn3D.Examples
{
	[RequireComponent(typeof(Text))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dChannelCounterText")]
	[AddComponentMenu("Paint in 3D/Examples/Channel Counter Text")]
	public class P3dChannelCounterText : MonoBehaviour
	{
		public enum ChannelType
		{
			Red = 0,
			Green = 1,
			Blue = 2,
			Alpha = 3,
			InverseAlpha = 4
		}

		public enum OutputType
		{
			Percentage = 0,
			Pixels = 1
		}

		[SerializeField]
		private List<P3dChannelCounter> counters;

		[SerializeField]
		private ChannelType channel;

		[SerializeField]
		private OutputType output;

		[SerializeField]
		private int round = 1;

		[Multiline]
		[SerializeField]
		private string format = "{0}";

		[NonSerialized]
		private Text cachedText;

		public List<P3dChannelCounter> Counters
		{
			get
			{
				if (counters == null)
				{
					counters = new List<P3dChannelCounter>();
				}
				return counters;
			}
		}

		public ChannelType Channel
		{
			get
			{
				return channel;
			}
			set
			{
				channel = value;
			}
		}

		public OutputType Output
		{
			get
			{
				return output;
			}
			set
			{
				output = value;
			}
		}

		public int Round
		{
			get
			{
				return round;
			}
			set
			{
				round = value;
			}
		}

		public string Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		protected virtual void OnEnable()
		{
			cachedText = GetComponent<Text>();
		}

		protected virtual void Update()
		{
			List<P3dChannelCounter> list = ((counters.Count > 0) ? counters : null);
			switch (output)
			{
			case OutputType.Percentage:
			{
				Vector4 ratioRGBA = P3dChannelCounter.GetRatioRGBA(list);
				switch (channel)
				{
				case ChannelType.Red:
					OutputRatio(ratioRGBA.x);
					break;
				case ChannelType.Green:
					OutputRatio(ratioRGBA.y);
					break;
				case ChannelType.Blue:
					OutputRatio(ratioRGBA.z);
					break;
				case ChannelType.Alpha:
					OutputRatio(ratioRGBA.w);
					break;
				case ChannelType.InverseAlpha:
					OutputRatio(1f - ratioRGBA.w);
					break;
				}
				break;
			}
			case OutputType.Pixels:
				switch (channel)
				{
				case ChannelType.Red:
					OutputSolid(P3dChannelCounter.GetSolidR(list));
					break;
				case ChannelType.Green:
					OutputSolid(P3dChannelCounter.GetSolidG(list));
					break;
				case ChannelType.Blue:
					OutputSolid(P3dChannelCounter.GetSolidB(list));
					break;
				case ChannelType.Alpha:
					OutputSolid(P3dChannelCounter.GetSolidA(list));
					break;
				case ChannelType.InverseAlpha:
					OutputSolid(P3dChannelCounter.GetTotal(list) - P3dChannelCounter.GetSolidA(list));
					break;
				}
				break;
			}
		}

		private void OutputRatio(float ratio)
		{
			float num = Mathf.Clamp01(ratio) * 100f;
			if (round >= 0)
			{
				double num2 = Math.Pow(10.0, round);
				num = (float)(Math.Truncate((double)num * num2) / num2);
			}
			cachedText.text = string.Format(format, num);
		}

		private void OutputSolid(long solid)
		{
			cachedText.text = string.Format(format, solid);
		}
	}
}
