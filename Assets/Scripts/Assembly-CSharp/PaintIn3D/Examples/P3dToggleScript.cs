using System;
using UnityEngine;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dToggleScript")]
	[AddComponentMenu("Paint in 3D/Examples/Toggle Script")]
	public class P3dToggleScript : MonoBehaviour
	{
		[SerializeField]
		private KeyCode key;

		[SerializeField]
		private MonoBehaviour target;

		[SerializeField]
		protected bool storeStates;

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

		public MonoBehaviour Target
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

		protected virtual void Update()
		{
			inputManager.Update(key);
			if (!(target != null))
			{
				return;
			}
			if (Pressing())
			{
				if (storeStates && !target.enabled)
				{
					P3dStateManager.StoreAllStates();
				}
				target.enabled = true;
			}
			else
			{
				target.enabled = false;
			}
		}

		private bool Pressing()
		{
			for (int i = 0; i < inputManager.Fingers.Count; i++)
			{
				P3dInputManager.Finger finger = inputManager.Fingers[i];
				if (finger.Index >= 0 || finger.Index == -1)
				{
					return true;
				}
			}
			return false;
		}
	}
}
