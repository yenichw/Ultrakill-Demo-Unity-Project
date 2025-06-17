using UnityEngine;
using UnityEngine.EventSystems;

namespace PaintIn3D.Examples
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dButtonLink")]
	[AddComponentMenu("Paint in 3D/Examples/Button Link")]
	public class P3dButtonLink : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private string url;

		public string Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Open();
		}

		[ContextMenu("This allows you to manually open the current URL.")]
		public void Open()
		{
			Open(url);
		}

		public void Open(string url)
		{
			Application.OpenURL(url);
		}
	}
}
