using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dModifyTextureRandom")]
	[AddComponentMenu("Paint in 3D/Modify/Modify Texture Random")]
	public class P3dModifyTextureRandom : MonoBehaviour, IModify, IModifyTexture
	{
		[SerializeField]
		private List<Texture> textures;

		public List<Texture> Textures
		{
			get
			{
				if (textures == null)
				{
					textures = new List<Texture>();
				}
				return textures;
			}
		}

		public void ModifyTexture(float pressure, ref Texture texture)
		{
			if (textures != null && textures.Count > 0)
			{
				int index = Random.Range(0, textures.Count);
				texture = textures[index];
			}
		}
	}
}
