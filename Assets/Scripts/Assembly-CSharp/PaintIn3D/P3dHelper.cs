using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public static class P3dHelper
	{
		public const string HelpUrlPrefix = "http://carloswilkes.github.io/Documentation/PaintIn3D#";

		public const string ComponentMenuPrefix = "Paint in 3D/";

		private static Mesh quadMesh;

		private static Mesh sphereMesh;

		private static bool sphereMeshSet;

		private static Mesh quadMesh2;

		private static bool quadMeshSet;

		static P3dHelper()
		{
			quadMesh = new Mesh();
			quadMesh.vertices = new Vector3[4]
			{
				new Vector3(-1f, -1f, 0.5f),
				new Vector3(1f, -1f, 0.5f),
				new Vector3(-1f, 1f, 0.5f),
				new Vector3(1f, 1f, 0.5f)
			};
			quadMesh.uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
			quadMesh.triangles = new int[6] { 0, 2, 1, 3, 1, 2 };
			quadMesh.RecalculateBounds();
			quadMesh.RecalculateNormals();
			quadMesh.RecalculateTangents();
		}

		public static Quaternion NormalToCameraRotation(Vector3 normal, Camera optionalCamera = null)
		{
			Vector3 up = Vector3.up;
			Camera camera = GetCamera(optionalCamera);
			if (camera != null)
			{
				up = camera.transform.up;
			}
			return Quaternion.LookRotation(-normal, up);
		}

		public static Camera GetCamera(Camera camera = null)
		{
			if (camera == null || !camera.isActiveAndEnabled)
			{
				camera = Camera.main;
			}
			return camera;
		}

		public static Vector3 GetCameraUp(Camera camera = null)
		{
			camera = GetCamera(camera);
			if (!(camera != null))
			{
				return Vector3.up;
			}
			return camera.transform.up;
		}

		public static bool IndexInMask(int index, int mask)
		{
			mask &= 1 << index;
			return mask != 0;
		}

		public static RenderTexture GetRenderTexture(RenderTextureDescriptor desc)
		{
			return GetRenderTexture(desc, QualitySettings.activeColorSpace == ColorSpace.Gamma);
		}

		public static RenderTexture GetRenderTexture(RenderTextureDescriptor desc, bool sRGB)
		{
			desc.sRGB = sRGB;
			RenderTexture temporary = RenderTexture.GetTemporary(desc);
			temporary.DiscardContents();
			return temporary;
		}

		public static RenderTexture ReleaseRenderTexture(RenderTexture renderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
			return null;
		}

		public static bool CanReadPixels(TextureFormat format)
		{
			if (format == TextureFormat.RGBA32 || format == TextureFormat.ARGB32 || format == TextureFormat.RGB24 || format == TextureFormat.RGBAFloat || format == TextureFormat.RGBAHalf)
			{
				return true;
			}
			return false;
		}

		public static void ReadPixelsLinearGamma(Texture2D texture2D, RenderTexture renderTexture)
		{
			if (renderTexture != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				Texture2D texture2D2 = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipChain: false, linear: true);
				texture2D2.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				RenderTexture.active = active;
				Color[] pixels = texture2D2.GetPixels();
				for (int num = pixels.Length - 1; num >= 0; num--)
				{
					pixels[0] = pixels[0].gamma;
				}
				UnityEngine.Object.DestroyImmediate(texture2D2);
				texture2D.SetPixels(pixels);
				texture2D.Apply();
			}
		}

		public static void ReadPixels(Texture2D texture2D, RenderTexture renderTexture)
		{
			if (renderTexture != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				if (CanReadPixels(texture2D.format))
				{
					texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
					RenderTexture.active = active;
					texture2D.Apply();
					return;
				}
				Texture2D texture2D2 = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipChain: false);
				texture2D2.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				RenderTexture.active = active;
				Color32[] pixels = texture2D2.GetPixels32();
				UnityEngine.Object.DestroyImmediate(texture2D2);
				texture2D.SetPixels32(pixels);
				texture2D.Apply();
			}
		}

		public static bool Downsample(RenderTexture renderTexture, int steps, ref RenderTexture temporary)
		{
			if (steps > 0 && renderTexture != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTextureDescriptor desc = new RenderTextureDescriptor(renderTexture.width / 2, renderTexture.height / 2, renderTexture.format, 0);
				RenderTexture renderTexture2 = GetRenderTexture(desc);
				P3dPaintReplace.BlitFast(renderTexture2, renderTexture, Color.white);
				for (int i = 1; i < steps; i++)
				{
					desc.width /= 2;
					desc.height /= 2;
					renderTexture = renderTexture2;
					renderTexture2 = GetRenderTexture(desc);
					Graphics.Blit(renderTexture, renderTexture2);
					ReleaseRenderTexture(renderTexture);
				}
				temporary = renderTexture2;
				RenderTexture.active = active;
				return true;
			}
			return false;
		}

		public static bool HasMipMaps(Texture texture)
		{
			if (texture != null)
			{
				Texture2D texture2D = texture as Texture2D;
				if (texture2D != null)
				{
					return texture2D.mipmapCount > 0;
				}
				RenderTexture renderTexture = texture as RenderTexture;
				if (renderTexture != null)
				{
					return renderTexture.useMipMap;
				}
			}
			return false;
		}

		public static Mesh GetSphereMesh()
		{
			if (!sphereMeshSet)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphereMeshSet = true;
				sphereMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
			return sphereMesh;
		}

		public static Mesh GetQuadMesh()
		{
			if (!quadMeshSet)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
				quadMeshSet = true;
				quadMesh2 = gameObject.GetComponent<MeshFilter>().sharedMesh;
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
			return quadMesh2;
		}

		public static Texture2D GetReadableCopy(RenderTexture renderTexture, TextureFormat format = TextureFormat.ARGB32, bool mipMaps = false)
		{
			if (renderTexture != null)
			{
				Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, format, mipMaps, QualitySettings.activeColorSpace == ColorSpace.Linear);
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				RenderTexture.active = active;
				texture2D.Apply();
				return texture2D;
			}
			return null;
		}

		public static Texture2D GetReadableCopy(Texture texture, TextureFormat format = TextureFormat.ARGB32, bool mipMaps = false, int width = 0, int height = 0)
		{
			Texture2D texture2D = null;
			if (texture != null)
			{
				if (width <= 0)
				{
					width = texture.width;
				}
				if (height <= 0)
				{
					height = texture.height;
				}
				if (CanReadPixels(format))
				{
					RenderTexture active = RenderTexture.active;
					RenderTexture renderTexture = GetRenderTexture(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0));
					texture2D = new Texture2D(width, height, format, mipMaps, QualitySettings.activeColorSpace == ColorSpace.Linear);
					Graphics.Blit(texture, renderTexture);
					RenderTexture.active = renderTexture;
					texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
					RenderTexture.active = active;
					ReleaseRenderTexture(renderTexture);
					texture2D.Apply();
				}
			}
			return texture2D;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T val = a;
			a = b;
			b = val;
		}

		public static void SaveBytes(string saveName, byte[] data, bool save = true)
		{
			string value = null;
			if (data != null)
			{
				value = Convert.ToBase64String(data);
			}
			PlayerPrefs.SetString(saveName, value);
			if (save)
			{
				PlayerPrefs.Save();
			}
		}

		public static byte[] LoadBytes(string saveName)
		{
			string @string = PlayerPrefs.GetString(saveName);
			if (!string.IsNullOrEmpty(@string))
			{
				return Convert.FromBase64String(@string);
			}
			return null;
		}

		public static bool SaveExists(string saveName)
		{
			return PlayerPrefs.HasKey(saveName);
		}

		public static void ClearSave(string saveName, bool save = true)
		{
			if (PlayerPrefs.HasKey(saveName))
			{
				PlayerPrefs.DeleteKey(saveName);
				if (save)
				{
					PlayerPrefs.Save();
				}
			}
		}

		public static float GetAspect(Texture textureA, Texture textureB = null)
		{
			if (textureA != null)
			{
				return (float)textureA.width / (float)textureA.height;
			}
			if (textureB != null)
			{
				return (float)textureB.width / (float)textureB.height;
			}
			return 1f;
		}

		public static void Blit(RenderTexture renderTexture, Texture other)
		{
			RenderTexture active = RenderTexture.active;
			Graphics.Blit(other, renderTexture);
			RenderTexture.active = active;
		}

		public static void Blit(RenderTexture renderTexture, Material material)
		{
			RenderTexture active = RenderTexture.active;
			Graphics.Blit(null, renderTexture, material);
			RenderTexture.active = active;
		}

		public static Vector4 IndexToVector(int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(1f, 0f, 0f, 0f);
			case 1:
				return new Vector4(0f, 1f, 0f, 0f);
			case 2:
				return new Vector4(0f, 0f, 1f, 0f);
			case 3:
				return new Vector4(0f, 0f, 0f, 1f);
			default:
				return default(Vector4);
			}
		}

		public static void Draw(Material material, Mesh mesh, Matrix4x4 matrix, int subMesh, P3dCoord coord)
		{
			material.SetVector(P3dShader._Channel, IndexToVector((int)coord));
			material.SetPass(0);
			Graphics.DrawMeshNow(mesh, matrix, subMesh);
		}

		public static void Draw(Material material)
		{
			material.SetPass(0);
			Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity, 0);
		}

		public static Texture2D CreateTexture(int width, int height, TextureFormat format, bool mipMaps)
		{
			if (width > 0 && height > 0)
			{
				return new Texture2D(width, height, format, mipMaps);
			}
			return null;
		}

		public static Material GetMaterial(GameObject gameObject, int materialIndex = 0)
		{
			if (gameObject != null && materialIndex >= 0)
			{
				Renderer component = gameObject.GetComponent<Renderer>();
				if (component != null)
				{
					Material[] sharedMaterials = component.sharedMaterials;
					if (materialIndex < sharedMaterials.Length)
					{
						return sharedMaterials[materialIndex];
					}
				}
			}
			return null;
		}

		public static Material CloneMaterial(GameObject gameObject, int materialIndex = 0)
		{
			if (gameObject != null && materialIndex >= 0)
			{
				Renderer component = gameObject.GetComponent<Renderer>();
				if (component != null)
				{
					Material[] sharedMaterials = component.sharedMaterials;
					if (materialIndex < sharedMaterials.Length)
					{
						Material original = sharedMaterials[materialIndex];
						original = (sharedMaterials[materialIndex] = UnityEngine.Object.Instantiate(original));
						component.sharedMaterials = sharedMaterials;
						return original;
					}
				}
			}
			return null;
		}

		public static Material AddMaterial(Renderer renderer, Shader shader, int materialIndex = -1)
		{
			if (renderer != null)
			{
				List<Material> list = new List<Material>(renderer.sharedMaterials);
				Material material = new Material(shader);
				if (materialIndex <= 0)
				{
					materialIndex = list.Count;
				}
				list.Insert(materialIndex, material);
				renderer.sharedMaterials = list.ToArray();
				return material;
			}
			return null;
		}

		public static float Reciprocal(float a)
		{
			if (a == 0f)
			{
				return 0f;
			}
			return 1f / a;
		}

		public static float Divide(float a, float b)
		{
			if (b == 0f)
			{
				return 0f;
			}
			return a / b;
		}

		public static int Mod(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		public static Vector3 Reciprocal3(Vector3 xyz)
		{
			xyz.x = Reciprocal(xyz.x);
			xyz.y = Reciprocal(xyz.y);
			xyz.z = Reciprocal(xyz.z);
			return xyz;
		}

		public static float DampenFactor(float dampening, float elapsed)
		{
			if (dampening < 0f)
			{
				return 1f;
			}
			return 1f - Mathf.Pow((float)Math.E, (0f - dampening) * elapsed);
		}

		public static T Destroy<T>(T o) where T : UnityEngine.Object
		{
			UnityEngine.Object.Destroy(o);
			return null;
		}
	}
}
