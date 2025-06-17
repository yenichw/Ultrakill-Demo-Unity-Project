using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dTapThrow")]
	[AddComponentMenu("Paint in 3D/Examples/Tap Throw")]
	public class P3dTapThrow : MonoBehaviour
	{
		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private float speed = 10f;

		[SerializeField]
		protected bool storeStates;

		public GameObject Prefab
		{
			get
			{
				return prefab;
			}
			set
			{
				prefab = value;
			}
		}

		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
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
			if (!(prefab != null) || !Input.GetMouseButtonDown(0) || P3dInputManager.PointOverGui(Input.mousePosition))
			{
				return;
			}
			Camera camera = P3dHelper.GetCamera();
			if (camera != null)
			{
				if (storeStates)
				{
					P3dStateManager.StoreAllStates();
				}
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				Quaternion rotation = Quaternion.LookRotation(ray.direction);
				GameObject gameObject = Object.Instantiate(prefab, ray.origin, rotation);
				gameObject.SetActive(value: true);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = gameObject.transform.forward * Speed;
				}
			}
		}
	}
}
