using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dButtonUndoAll")]
	[AddComponentMenu("Paint in 3D/Examples/Button Undo All")]
	public class P3dButtonUndoAll : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			UndoAll();
		}

		[ContextMenu("Undo All")]
		public void UndoAll()
		{
			P3dStateManager.UndoAll();
		}

		protected virtual void Update()
		{
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = (P3dStateManager.CanUndo ? 1f : 0.5f);
			}
		}
	}
}
