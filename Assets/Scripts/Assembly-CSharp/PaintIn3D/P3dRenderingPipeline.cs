using System;
using UnityEngine;

namespace PaintIn3D
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dRenderingPipeline")]
	public class P3dRenderingPipeline : ScriptableObject
	{
		private static P3dRenderingPipeline instance;

		[SerializeField]
		private bool isScriptable;

		public static bool IsScriptable
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<P3dRenderingPipeline>("P3dRenderingPipeline");
				}
				if (instance != null)
				{
					return instance.isScriptable;
				}
				return false;
			}
		}

		public static event Action<bool> OnPipelineChanged;
	}
}
