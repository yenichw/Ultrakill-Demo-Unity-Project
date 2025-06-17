using System;
using UnityEngine;

namespace PaintIn3D.Examples
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dDragPitchYaw")]
	[AddComponentMenu("Paint in 3D/Examples/Drag Pitch Yaw")]
	public class P3dDragPitchYaw : MonoBehaviour
	{
		[SerializeField]
		private Transform requires;

		[SerializeField]
		private KeyCode key = KeyCode.Mouse0;

		[SerializeField]
		private float pitch;

		[SerializeField]
		private float pitchSensitivity = 0.1f;

		[SerializeField]
		private float pitchMin = -90f;

		[SerializeField]
		private float pitchMax = 90f;

		[SerializeField]
		private float yaw;

		[SerializeField]
		private float yawSensitivity = 0.1f;

		[SerializeField]
		private float dampening = 10f;

		[SerializeField]
		private float currentPitch;

		[SerializeField]
		private float currentYaw;

		[NonSerialized]
		private P3dInputManager inputManager = new P3dInputManager();

		public Transform Requires
		{
			get
			{
				return requires;
			}
			set
			{
				requires = value;
			}
		}

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

		public float Pitch
		{
			get
			{
				return pitch;
			}
			set
			{
				pitch = value;
			}
		}

		public float PitchSensitivity
		{
			get
			{
				return pitchSensitivity;
			}
			set
			{
				pitchSensitivity = value;
			}
		}

		public float PitchMin
		{
			get
			{
				return pitchMin;
			}
			set
			{
				pitchMin = value;
			}
		}

		public float PitchMax
		{
			get
			{
				return pitchMax;
			}
			set
			{
				pitchMax = value;
			}
		}

		public float Yaw
		{
			get
			{
				return yaw;
			}
			set
			{
				yaw = value;
			}
		}

		public float YawSensitivity
		{
			get
			{
				return yawSensitivity;
			}
			set
			{
				yawSensitivity = value;
			}
		}

		public float Dampening
		{
			get
			{
				return dampening;
			}
			set
			{
				dampening = value;
			}
		}

		protected virtual void Update()
		{
			inputManager.Update(key);
			if ((requires == null || requires.gameObject.activeInHierarchy || Input.GetMouseButton(1)) && Application.isPlaying)
			{
				Vector2 averageDeltaScaled = inputManager.GetAverageDeltaScaled();
				pitch -= averageDeltaScaled.y * pitchSensitivity;
				yaw += averageDeltaScaled.x * yawSensitivity;
			}
			pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
			float t = P3dHelper.DampenFactor(dampening, Time.deltaTime);
			currentPitch = Mathf.Lerp(currentPitch, pitch, t);
			currentYaw = Mathf.Lerp(currentYaw, yaw, t);
			base.transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
		}
	}
}
