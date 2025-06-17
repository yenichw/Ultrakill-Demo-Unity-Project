using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dButtonIsolate")]
	[AddComponentMenu("Paint in 3D/Examples/Button Isolate")]
	public class P3dButtonIsolate : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private Transform target;

		public Transform Target
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

		protected virtual void Update()
		{
			if (target != null)
			{
				CanvasGroup component = GetComponent<CanvasGroup>();
				if (component != null)
				{
					component.alpha = (target.gameObject.activeInHierarchy ? 1f : 0.5f);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!(target != null))
			{
				return;
			}
			foreach (Transform item in target.transform.parent)
			{
				item.gameObject.SetActive(item == target);
			}
		}
	}
}
