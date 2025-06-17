using System;
using UnityEngine;

namespace PaintIn3D.Examples
{
	public class P3dDocumentation : ScriptableObject
	{
		public string Title;

		public string YouTube;

		public string Forum;

		public string Link;

		public string IconData;

		public string HTML;

		[NonSerialized]
		private Texture2D icon;

		public Texture2D Icon
		{
			get
			{
				if (icon == null)
				{
					icon = new Texture2D(1, 1);
					icon.LoadImage(Convert.FromBase64String(IconData));
				}
				return icon;
			}
		}
	}
}
