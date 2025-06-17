using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dButtonRedoAll")]
	[AddComponentMenu("Paint in 3D/Examples/Button Redo All")]
	public class P3dButtonRedoAll : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			RedoAll();
		}

		[ContextMenu("Redo All")]
		public void RedoAll()
		{
			P3dStateManager.RedoAll();
		}

		protected virtual void Update()
		{
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = (P3dStateManager.CanRedo ? 1f : 0.5f);
			}
		}
	}
}
