using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dShaderTemplate : ScriptableObject
	{
		public interface IHasTemplate
		{
			void SetTemplate(P3dShaderTemplate template);

			P3dShaderTemplate GetTemplate();
		}

		public enum Channel
		{
			Red = 0,
			Green = 1,
			Blue = 2,
			Alpha = 3
		}

		[Serializable]
		public class Write
		{
			public P3dGroup SourceGroup;

			public Channel SourceChannel;

			public Write GetCopy()
			{
				return new Write
				{
					SourceGroup = SourceGroup,
					SourceChannel = SourceChannel
				};
			}
		}

		[Serializable]
		public class Slot
		{
			public string Name;

			public string Alias;

			public Write WriteR;

			public Write WriteG;

			public Write WriteB;

			public Write WriteA;

			public string GetAlias()
			{
				if (string.IsNullOrEmpty(Alias))
				{
					return Name;
				}
				return Alias;
			}
		}

		private static List<P3dShaderTemplate> tempInstances = new List<P3dShaderTemplate>();

		[SerializeField]
		private Shader shader;

		[SerializeField]
		private List<Slot> slots;

		private static List<P3dShaderTemplate> cachedInstances = new List<P3dShaderTemplate>();

		private static bool cachedInstancesSet;

		public Shader Shader
		{
			get
			{
				return shader;
			}
			set
			{
				shader = value;
			}
		}

		public List<Slot> Slots
		{
			get
			{
				if (slots == null)
				{
					slots = new List<Slot>();
				}
				return slots;
			}
		}

		public static void UpdateCachedInstances()
		{
			cachedInstancesSet = true;
			cachedInstances.Clear();
		}

		public static List<P3dShaderTemplate> GetTemplates(Shader shader)
		{
			tempInstances.Clear();
			if (!cachedInstancesSet)
			{
				UpdateCachedInstances();
			}
			if (shader != null)
			{
				foreach (P3dShaderTemplate cachedInstance in cachedInstances)
				{
					if (cachedInstance.Shader == shader)
					{
						tempInstances.Add(cachedInstance);
					}
				}
			}
			return tempInstances;
		}
	}
}
