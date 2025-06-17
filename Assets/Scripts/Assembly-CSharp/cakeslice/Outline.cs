using System.Linq;
using UnityEngine;

namespace cakeslice
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class Outline : MonoBehaviour
	{
		public int color;

		public bool eraseRenderer;

		[HideInInspector]
		public int originalLayer;

		[HideInInspector]
		public Material[] originalMaterials;

		public Renderer Renderer { get; private set; }

		private void Awake()
		{
			Renderer = GetComponent<Renderer>();
		}
	}
}
