using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dVrTool")]
	[AddComponentMenu("Paint in 3D/Examples/VR Tool")]
	public class P3dVrTool : P3dLinkedBehaviour<P3dVrTool>
	{
		[SerializeField]
		private XRNode node = XRNode.RightHand;

		[SerializeField]
		private bool storeStates = true;

		[SerializeField]
		private Vector3 localOffset;

		[SerializeField]
		private Vector3 simulatedOffset = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 simulatedKeyOffset;

		[SerializeField]
		private float simulatedDampening = 5f;

		[SerializeField]
		private UnityEvent onGrabbed;

		[SerializeField]
		private UnityEvent onDropped;

		[SerializeField]
		private UnityEvent onTriggerPress;

		[SerializeField]
		private UnityEvent onTriggerRelease;

		[SerializeField]
		private UnityEvent onGripPress;

		[SerializeField]
		private UnityEvent onGripRelease;

		private static List<P3dVrTool> tempTools = new List<P3dVrTool>();

		public XRNode Node
		{
			get
			{
				return node;
			}
			set
			{
				node = value;
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

		public Vector3 LocalOffset
		{
			get
			{
				return localOffset;
			}
			set
			{
				localOffset = value;
			}
		}

		public Vector3 SimulatedOffset
		{
			get
			{
				return simulatedOffset;
			}
			set
			{
				simulatedOffset = value;
			}
		}

		public Vector3 SimulatedKeyOffset
		{
			get
			{
				return simulatedKeyOffset;
			}
			set
			{
				simulatedKeyOffset = value;
			}
		}

		public float SimulatedDampening
		{
			get
			{
				return simulatedDampening;
			}
			set
			{
				simulatedDampening = value;
			}
		}

		public UnityEvent OnGrabbed
		{
			get
			{
				if (onGrabbed == null)
				{
					onGrabbed = new UnityEvent();
				}
				return onGrabbed;
			}
		}

		public UnityEvent OnDropped
		{
			get
			{
				if (onDropped == null)
				{
					onDropped = new UnityEvent();
				}
				return onDropped;
			}
		}

		public UnityEvent OnTriggerPress
		{
			get
			{
				if (onTriggerPress == null)
				{
					onTriggerPress = new UnityEvent();
				}
				return onTriggerPress;
			}
		}

		public UnityEvent OnTriggerRelease
		{
			get
			{
				if (onTriggerRelease == null)
				{
					onTriggerRelease = new UnityEvent();
				}
				return onTriggerRelease;
			}
		}

		public UnityEvent OnGripPress
		{
			get
			{
				if (onGripPress == null)
				{
					onGripPress = new UnityEvent();
				}
				return onGripPress;
			}
		}

		public UnityEvent OnGripRelease
		{
			get
			{
				if (onGripRelease == null)
				{
					onGripRelease = new UnityEvent();
				}
				return onGripRelease;
			}
		}

		public void Grab(XRNode newNode)
		{
			if (node != newNode)
			{
				Drop();
				node = newNode;
				if (onGrabbed != null)
				{
					onGrabbed.Invoke();
				}
			}
		}

		[ContextMenu("Drop")]
		public void Drop()
		{
			if (node >= XRNode.LeftEye)
			{
				node = (XRNode)(-1);
				if (onDropped != null)
				{
					onDropped.Invoke();
				}
			}
		}

		[ContextMenu("Drop And Grab Next Tool")]
		public void DropAndGrabNextTool()
		{
			if (node >= XRNode.LeftEye)
			{
				XRNode newNode = node;
				Drop();
				List<P3dVrTool> tools = GetTools((XRNode)(-1));
				int num = tools.IndexOf(this);
				tools[(num + 1) % tools.Count].Grab(newNode);
			}
		}

		public static List<P3dVrTool> GetTools(XRNode node)
		{
			GetTools(node, ref tempTools);
			return tempTools;
		}

		public static void GetTools(XRNode node, ref List<P3dVrTool> tools)
		{
			if (tools == null)
			{
				tools = new List<P3dVrTool>();
			}
			else
			{
				tools.Clear();
			}
			P3dVrTool p3dVrTool = P3dLinkedBehaviour<P3dVrTool>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dVrTool>.InstanceCount; i++)
			{
				if (p3dVrTool.node == node)
				{
					tools.Add(p3dVrTool);
				}
				p3dVrTool = p3dVrTool.NextInstance;
			}
		}

		public static void DropAllTools(XRNode node)
		{
			P3dVrTool p3dVrTool = P3dLinkedBehaviour<P3dVrTool>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dVrTool>.InstanceCount; i++)
			{
				if (p3dVrTool.node == node)
				{
					p3dVrTool.Drop();
				}
				p3dVrTool = p3dVrTool.NextInstance;
			}
		}

		public void UpdateGripped(P3dVrManager vrManager)
		{
			Vector3 position = default(Vector3);
			bool flag = false;
			if (vrManager.TryGetPosition(node, ref position))
			{
				flag = true;
				if (vrManager.IsSimulation)
				{
					position += base.transform.rotation * localOffset;
					if (vrManager.GetTrigger(node))
					{
						position += base.transform.rotation * simulatedKeyOffset;
					}
				}
				position += base.transform.rotation * simulatedOffset;
			}
			Quaternion rotation = default(Quaternion);
			bool flag2 = false;
			if (vrManager.TryGetRotation(node, ref rotation))
			{
				flag2 = true;
			}
			float t = 1f;
			if (vrManager.IsSimulation)
			{
				t = P3dHelper.DampenFactor(simulatedDampening, Time.deltaTime);
			}
			if (flag)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, position, t);
			}
			if (flag2)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, t);
			}
			if (vrManager.GetTriggerPressed(node) && onTriggerPress != null)
			{
				onTriggerPress.Invoke();
			}
			if (vrManager.GetTriggerReleased(node) && onTriggerRelease != null)
			{
				onTriggerRelease.Invoke();
			}
			if (vrManager.GetGripPressed(node) && onGripPress != null)
			{
				onGripPress.Invoke();
			}
			if (vrManager.GetGripReleased(node) && onGripRelease != null)
			{
				onGripRelease.Invoke();
			}
		}

		protected virtual void Start()
		{
			if (node >= XRNode.LeftEye && onGrabbed != null)
			{
				onGrabbed.Invoke();
			}
		}
	}
}
