using System;
using UnityEngine;
using UnityEngine.UI;

namespace PaintIn3D.Examples
{
	[RequireComponent(typeof(Text))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dColorCounterText")]
	[AddComponentMenu("Paint in 3D/Examples/Color Counter Text")]
	public class P3dColorCounterText : MonoBehaviour
	{
		[SerializeField]
		private P3dColor color;

		[Multiline]
		[SerializeField]
		private string format = "{TEAM} = {TOTAL} : {RATIO}";

		[NonSerialized]
		private Text cachedText;

		public P3dColor Color
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
			if (color != null)
			{
				string text = format;
				text = text.Replace("{TEAM}", color.name);
				text = text.Replace("{TOTAL}", color.Solid.ToString());
				text = text.Replace("{RATIO}", color.Ratio.ToString());
				cachedText.text = text;
			}
		}
	}
}
