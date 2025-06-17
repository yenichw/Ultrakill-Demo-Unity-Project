using System;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dToggleParticles")]
	[AddComponentMenu("Paint in 3D/Examples/Toggle Particles")]
	public class P3dToggleParticles : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		[SerializeField]
		private ParticleSystem target;

		[SerializeField]
		protected bool storeStates = true;

		[NonSerialized]
		private P3dInputManager inputManager = new P3dInputManager();

		public KeyCode Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public ParticleSystem Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public bool StoreStates
		{
			get
			{
				return storeStates;
			}
			set
			{
				storeStates = value;
			}
		}

		protected virtual void LateUpdate()
		{
			inputManager.Update(key);
			if (!(target != null))
			{
				return;
			}
			if (Pressing())
			{
				if (storeStates && !target.isPlaying)
				{
					P3dStateManager.StoreAllStates();
				}
				target.Play();
			}
			else
			{
				target.Stop();
			}
		}

		private bool Pressing()
		{
			return inputManager.Fingers.Count > 0;
		}
	}
}
