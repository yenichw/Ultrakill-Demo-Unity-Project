using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintable")]
	[AddComponentMenu("Paint in 3D/Paintable")]
	public class P3dPaintable : P3dModel
	{
		public enum ActivationType
		{
			Awake = 0,
			OnEnable = 1,
			Start = 2,
			OnFirstUse = 3
		}

		[SerializeField]
		private ActivationType activation;

		[SerializeField]
		private Vector3 baseScale;

		[SerializeField]
		private List<Renderer> otherRenderers;

		[SerializeField]
		private bool activated;

		[NonSerialized]
		private List<P3dPaintableTexture> paintableTextures = new List<P3dPaintableTexture>();

		[NonSerialized]
		private static List<P3dMaterialCloner> tempMaterialCloners = new List<P3dMaterialCloner>();

		public override P3dPaintable Paintable
		{
			get
			{
				paintable = this;
				return paintable;
			}
			set
			{
			}
		}

		public ActivationType Activation
		{
			get
			{
				return activation;
			}
			set
			{
				activation = value;
			}
		}

		public Vector3 BaseScale
		{
			get
			{
				return baseScale;
			}
			set
			{
				baseScale = value;
			}
		}

		public List<Renderer> OtherRenderers
		{
			get
			{
				return otherRenderers;
			}
			set
			{
				otherRenderers = value;
			}
		}

		public bool Activated => activated;

		public List<P3dPaintableTexture> PaintableTextures => paintableTextures;

		public void ScaleSize(ref int width, ref int height)
		{
			if (baseScale != Vector3.zero)
			{
				float num = base.transform.localScale.magnitude / baseScale.magnitude;
				width = Mathf.CeilToInt((float)width * num);
				height = Mathf.CeilToInt((float)height * num);
			}
		}

		[ContextMenu("Activate")]
		public void Activate()
		{
			GetComponents(tempMaterialCloners);
			for (int num = tempMaterialCloners.Count - 1; num >= 0; num--)
			{
				tempMaterialCloners[num].Activate();
			}
			GetComponents(paintableTextures);
			for (int num2 = paintableTextures.Count - 1; num2 >= 0; num2--)
			{
				paintableTextures[num2].Activate();
			}
			activated = true;
		}

		public void ClearAll(Color color)
		{
			ClearAll(null, color);
		}

		public void ClearAll(Texture texture, Color color)
		{
			if (activated)
			{
				for (int num = paintableTextures.Count - 1; num >= 0; num--)
				{
					paintableTextures[num].Clear(texture, color);
				}
			}
		}

		public void Register(P3dPaintableTexture paintableTexture)
		{
			for (int num = paintableTextures.Count - 1; num >= 0; num--)
			{
				if (paintableTextures[num] == paintableTexture)
				{
					return;
				}
			}
			paintableTextures.Add(paintableTexture);
		}

		public void Unregister(P3dPaintableTexture paintableTexture)
		{
			for (int num = paintableTextures.Count - 1; num >= 0; num--)
			{
				if (paintableTextures[num] == paintableTexture)
				{
					paintableTextures.RemoveAt(num);
				}
			}
		}

		protected virtual void Awake()
		{
			if (activation == ActivationType.Awake && !activated)
			{
				Activate();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (activation == ActivationType.OnEnable && !activated)
			{
				Activate();
			}
			P3dPaintableManager.GetOrCreateInstance();
		}

		protected virtual void Start()
		{
			if (activation == ActivationType.Start && !activated)
			{
				Activate();
			}
		}
	}
}
