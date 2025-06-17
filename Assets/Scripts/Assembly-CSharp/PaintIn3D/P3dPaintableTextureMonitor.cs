using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dPaintableTextureMonitor : MonoBehaviour
	{
		[SerializeField]
		private P3dPaintableTexture paintableTexture;

		[SerializeField]
		private P3dPaintableTexture registeredPaintableTexture;

		public P3dPaintableTexture PaintableTexture
		{
			get
			{
				return paintableTexture;
			}
			set
			{
				paintableTexture = value;
				Register();
			}
		}

		public bool Registered => registeredPaintableTexture != null;

		[ContextMenu("Register")]
		public void Register()
		{
			Unregister();
			if (paintableTexture != null)
			{
				paintableTexture.OnModified += HandleModified;
				registeredPaintableTexture = paintableTexture;
			}
		}

		[ContextMenu("Unregister")]
		public void Unregister()
		{
			if (registeredPaintableTexture != null)
			{
				registeredPaintableTexture.OnModified -= HandleModified;
				registeredPaintableTexture = null;
			}
		}

		protected virtual void OnEnable()
		{
			Register();
		}

		protected virtual void OnDisable()
		{
			Unregister();
		}

		protected virtual void Start()
		{
			HandleModified(preview: false);
		}

		protected abstract void UpdateMonitor(P3dPaintableTexture paintableTexture, bool preview);

		private void HandleModified(bool preview)
		{
			if (registeredPaintableTexture != null)
			{
				UpdateMonitor(paintableTexture, preview);
			}
		}
	}
}
