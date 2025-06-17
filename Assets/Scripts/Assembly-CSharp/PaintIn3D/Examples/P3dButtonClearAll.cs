using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dButtonClearAll")]
	[AddComponentMenu("Paint in 3D/Examples/Button Clear All")]
	public class P3dButtonClearAll : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private bool clearStates = true;

		public bool ClearStates
		{
			get
			{
				return clearStates;
			}
			set
			{
				clearStates = value;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			ClearAll();
		}

		[ContextMenu("Clear All")]
		public void ClearAll()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.Clear();
				if (clearStates)
				{
					p3dPaintableTexture.ClearStates();
				}
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}
	}
}
