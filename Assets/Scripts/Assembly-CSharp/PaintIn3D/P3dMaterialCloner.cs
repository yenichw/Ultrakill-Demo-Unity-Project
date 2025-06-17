using UnityEngine;

namespace PaintIn3D
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(P3dPaintable))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dMaterialCloner")]
	[AddComponentMenu("Paint in 3D/Material Cloner")]
	public class P3dMaterialCloner : MonoBehaviour
	{
		[SerializeField]
		private int index;

		[SerializeField]
		private bool copySplatData;

		[SerializeField]
		private bool activated;

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		public bool CopySplatData
		{
			get
			{
				return copySplatData;
			}
			set
			{
				copySplatData = value;
			}
		}

		public bool Activated => activated;

		[ContextMenu("Reset Activation")]
		public void ResetActivation()
		{
			activated = false;
		}

		[ContextMenu("Activate")]
		public void Activate()
		{
			if (activated || index < 0)
			{
				return;
			}
			P3dPaintable component = GetComponent<P3dPaintable>();
			Renderer component2 = GetComponent<Renderer>();
			Material[] sharedMaterials = component2.sharedMaterials;
			activated = true;
			if (index >= sharedMaterials.Length)
			{
				return;
			}
			Material material = sharedMaterials[index];
			if (!(material != null))
			{
				return;
			}
			Material material2 = Object.Instantiate(material);
			if (copySplatData)
			{
				Material material3 = sharedMaterials[0];
				if (material3 != null)
				{
					material2.SetVector(P3dShader._BaseChannel, material3.GetVector(P3dShader._Channel));
					material2.SetTexture(P3dShader._BaseParallaxMap, material3.GetTexture(P3dShader._ParallaxMap));
					material2.SetVector(P3dShader._BaseParallaxMap_Transform, material3.GetVector(P3dShader._ParallaxMap_Transform));
				}
			}
			Replace(component2, sharedMaterials, material, material2);
			if (component.OtherRenderers == null)
			{
				return;
			}
			for (int num = component.OtherRenderers.Count - 1; num >= 0; num--)
			{
				Renderer renderer = component.OtherRenderers[num];
				if (renderer != null)
				{
					Replace(renderer, renderer.sharedMaterials, material, material2);
				}
			}
		}

		private void Replace(Renderer renderer, Material[] materials, Material oldMaterial, Material newMaterial)
		{
			bool flag = false;
			for (int num = materials.Length - 1; num >= 0; num--)
			{
				if (materials[num] == oldMaterial)
				{
					materials[num] = newMaterial;
					flag = true;
				}
			}
			if (flag)
			{
				renderer.sharedMaterials = materials;
			}
		}
	}
}
