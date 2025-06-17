using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PaintIn3D.Examples
{
	[RequireComponent(typeof(Image))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dChannelCounterFill")]
	[AddComponentMenu("Paint in 3D/Examples/Channel Counter Fill")]
	public class P3dChannelCounterFill : MonoBehaviour
	{
		public enum ChannelType
		{
			Red = 0,
			Green = 1,
			Blue = 2,
			Alpha = 3,
			InverseAlpha = 4
		}

		[SerializeField]
		private List<P3dChannelCounter> counters;

		[SerializeField]
		private ChannelType channel;

		[NonSerialized]
		private Image cachedImage;

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

		protected virtual void OnEnable()
		{
			cachedImage = GetComponent<Image>();
		}

		protected virtual void Update()
		{
			Vector4 ratioRGBA = P3dChannelCounter.GetRatioRGBA((counters.Count > 0) ? counters : null);
			float value = 0f;
			switch (channel)
			{
			case ChannelType.Red:
				value = ratioRGBA.x;
				break;
			case ChannelType.Green:
				value = ratioRGBA.y;
				break;
			case ChannelType.Blue:
				value = ratioRGBA.z;
				break;
			case ChannelType.Alpha:
				value = ratioRGBA.w;
				break;
			case ChannelType.InverseAlpha:
				value = 1f - ratioRGBA.w;
				break;
			}
			cachedImage.fillAmount = Mathf.Clamp01(value);
		}
	}
}
