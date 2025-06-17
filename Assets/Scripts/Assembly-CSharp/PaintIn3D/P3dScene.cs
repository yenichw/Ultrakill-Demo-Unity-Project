using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public class P3dScene : ScriptableObject
	{
		[Serializable]
		public class Image
		{
			public P3dGroup Group;

			public int MatId;

			public int Width;

			public int Height;

			public byte[] Pixels;

			public RenderTexture Current;

			public RenderTexture Preview;

			public Image Clone
			{
				get
				{
					Image image = new Image
					{
						Group = Group,
						MatId = MatId,
						Width = Width,
						Height = Height,
						Pixels = null,
						Preview = null
					};
					if (Current != null)
					{
						image.Current = P3dHelper.GetRenderTexture(Current.descriptor);
						P3dPaintReplace.Blit(image.Current, Current, Color.white);
					}
					return image;
				}
			}

			public void Save()
			{
				Texture2D readableCopy = P3dHelper.GetReadableCopy(Current);
				if (readableCopy != null)
				{
					Pixels = readableCopy.EncodeToPNG();
					Width = Current.width;
					Height = Current.height;
					UnityEngine.Object.DestroyImmediate(readableCopy);
				}
			}
		}

		[Serializable]
		public class Layer
		{
			public string Name;

			public float Opacity = 1f;

			public List<Image> Images = new List<Image>();

			public Layer Clone
			{
				get
				{
					Layer layer = new Layer
					{
						Name = Name,
						Opacity = Opacity
					};
					foreach (Image image in Images)
					{
						layer.Images.Add(image.Clone);
					}
					return layer;
				}
			}

			public Image GetImage(int matId, P3dGroup group)
			{
				foreach (Image image2 in Images)
				{
					if (image2.MatId == matId && (int)image2.Group == (int)group)
					{
						return image2;
					}
				}
				Image image = new Image();
				image.MatId = matId;
				image.Group = group;
				Images.Add(image);
				return image;
			}

			public void Save()
			{
				foreach (Image image in Images)
				{
					image.Save();
				}
			}
		}

		[Serializable]
		public class Slot
		{
			public string Name;

			public bool Dirty;

			public RenderTexture Texture;
		}

		[Serializable]
		public class Mat : P3dShaderTemplate.IHasTemplate
		{
			public int Id;

			public string Name;

			public Material Material;

			public P3dShaderTemplate Template;

			public int Width;

			public int Height;

			public List<Slot> Slots = new List<Slot>();

			public Dictionary<P3dGroup, MergedLayer> MergedLayers = new Dictionary<P3dGroup, MergedLayer>();

			[NonSerialized]
			public MaterialPropertyBlock Properties;

			public Mat Clone => new Mat
			{
				Id = Id,
				Name = Name,
				Material = Material,
				Template = Template,
				Width = Width,
				Height = Height
			};

			public bool SizesMatch
			{
				get
				{
					foreach (Slot slot in Slots)
					{
						if (slot.Texture != null && (slot.Texture.width != Width || slot.Texture.height != Height))
						{
							return false;
						}
					}
					return true;
				}
			}

			public RenderTextureDescriptor Desc
			{
				get
				{
					RenderTextureDescriptor result = new RenderTextureDescriptor(Width, Height, RenderTextureFormat.ARGB32, 0);
					result.autoGenerateMips = false;
					result.useMipMap = false;
					return result;
				}
			}

			public void Resize()
			{
				foreach (Slot slot in Slots)
				{
					if (slot.Texture != null && (slot.Texture.width != Width || slot.Texture.height != Height))
					{
						RenderTextureDescriptor descriptor = slot.Texture.descriptor;
						descriptor.width = Width;
						descriptor.height = Height;
						RenderTexture renderTexture = P3dHelper.GetRenderTexture(descriptor);
						P3dPaintReplace.Blit(renderTexture, slot.Texture, Color.white);
						P3dHelper.ReleaseRenderTexture(slot.Texture);
						slot.Texture = renderTexture;
					}
				}
			}

			public void SetTemplate(P3dShaderTemplate template)
			{
				Template = template;
			}

			public P3dShaderTemplate GetTemplate()
			{
				return Template;
			}

			public Slot GetSlot(string name)
			{
				foreach (Slot slot2 in Slots)
				{
					if (slot2.Name == name)
					{
						slot2.Dirty = false;
						return slot2;
					}
				}
				Slot slot = new Slot();
				slot.Name = name;
				Slots.Add(slot);
				return slot;
			}

			public Texture GetFinalTexture(P3dGroup group)
			{
				MergedLayer value = null;
				if (MergedLayers.TryGetValue(group, out value))
				{
					return value.Final;
				}
				return null;
			}

			public void UpdateMergedLayers(P3dGroup group)
			{
				MergedLayer value = null;
				if (!MergedLayers.TryGetValue(group, out value))
				{
					value = new MergedLayer();
					MergedLayers.Add(group, value);
				}
				value.Dirty = false;
			}
		}

		[Serializable]
		public class Obj
		{
			public string Name;

			public Mesh Mesh;

			public bool Paintable = true;

			public P3dCoord Coord;

			public Transform Transform;

			public Vector3 Position;

			public Quaternion Rotation = Quaternion.identity;

			public Vector3 Scale = Vector3.one;

			public List<int> MatIds = new List<int>();

			public Matrix4x4 Matrix => Matrix4x4.TRS(Position, Rotation, Scale);

			public Obj Clone => new Obj
			{
				Name = Name,
				Mesh = Mesh,
				Position = Position,
				Rotation = Rotation,
				Scale = Scale,
				Transform = Transform,
				Paintable = Paintable,
				Coord = Coord,
				MatIds = new List<int>(MatIds)
			};
		}

		[Serializable]
		public class Cloner : IClone
		{
			public string Name;

			public Vector3 Position;

			public Vector3 Euler;

			public bool Flip;

			public Matrix4x4 Transform(Matrix4x4 matrix)
			{
				Vector3 position = Position;
				Quaternion quaternion = Quaternion.Euler(Euler);
				Matrix4x4 matrix4x = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
				Matrix4x4 matrix4x2 = Matrix4x4.Translate(position);
				Matrix4x4 matrix4x3 = Matrix4x4.Rotate(quaternion);
				Matrix4x4 matrix4x4 = Matrix4x4.Translate(-position);
				Matrix4x4 matrix4x5 = Matrix4x4.Rotate(Quaternion.Inverse(quaternion));
				if (Flip)
				{
					matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 0f), new Vector3(1f, 1f, -1f));
				}
				return matrix4x2 * matrix4x3 * matrix4x * matrix4x5 * matrix4x4 * matrix;
			}
		}

		public class MergedLayer
		{
			public RenderTexture Under;

			public RenderTexture Above;

			public RenderTexture Final;

			public Layer Layer;

			public bool Dirty;

			public void Clear()
			{
				Under = P3dHelper.ReleaseRenderTexture(Under);
				Above = P3dHelper.ReleaseRenderTexture(Above);
				Final = P3dHelper.ReleaseRenderTexture(Final);
			}
		}

		public List<Layer> Layers = new List<Layer>();

		public List<Mat> Mats = new List<Mat>();

		public List<Obj> Objs = new List<Obj>();

		public List<Cloner> Cloners = new List<Cloner>();

		public void Clear()
		{
			foreach (Mat mat in Mats)
			{
				foreach (KeyValuePair<P3dGroup, MergedLayer> mergedLayer in mat.MergedLayers)
				{
					mergedLayer.Value.Clear();
				}
				mat.MergedLayers.Clear();
			}
			foreach (Layer layer in Layers)
			{
				foreach (Image image in layer.Images)
				{
					image.Pixels = null;
				}
			}
		}

		public void UpdateMergedLayers(Layer currentLayer)
		{
			foreach (Mat mat in Mats)
			{
				foreach (MergedLayer value2 in mat.MergedLayers.Values)
				{
					value2.Dirty = true;
					if (value2.Layer != currentLayer)
					{
						value2.Clear();
						value2.Layer = currentLayer;
					}
				}
				if (mat.Template != null)
				{
					foreach (P3dShaderTemplate.Slot slot in mat.Template.Slots)
					{
						mat.UpdateMergedLayers(slot.WriteR.SourceGroup);
						mat.UpdateMergedLayers(slot.WriteG.SourceGroup);
						mat.UpdateMergedLayers(slot.WriteB.SourceGroup);
						mat.UpdateMergedLayers(slot.WriteA.SourceGroup);
					}
				}
				foreach (KeyValuePair<P3dGroup, MergedLayer> mergedLayer in mat.MergedLayers)
				{
					P3dGroup key = mergedLayer.Key;
					MergedLayer value = mergedLayer.Value;
					if (value.Dirty)
					{
						value.Clear();
						continue;
					}
					int num = Layers.IndexOf(currentLayer);
					if (value.Under == null)
					{
						value.Under = P3dHelper.GetRenderTexture(mat.Desc);
						P3dGroupData groupData = P3dGroupData.GetGroupData(key);
						if (groupData != null)
						{
							P3dPaintReplace.Blit(value.Under, groupData.DefaultTexture, groupData.DefaultColor);
						}
						else
						{
							P3dPaintReplace.Blit(value.Under, null, default(Color));
						}
						for (int i = 0; i < num; i++)
						{
							TryBlendInto(ref value.Under, Layers[i], mat.Id, key);
						}
					}
					if (num == Layers.Count - 1)
					{
						if (value.Above != null)
						{
							value.Above = P3dHelper.ReleaseRenderTexture(value.Above);
						}
					}
					else if (value.Above == null)
					{
						value.Above = P3dHelper.GetRenderTexture(mat.Desc);
						P3dPaintReplace.Blit(value.Above, null, default(Color));
						for (int j = num + 1; j < Layers.Count; j++)
						{
							TryBlendInto(ref value.Above, Layers[j], mat.Id, key);
						}
					}
					if (value.Final == null)
					{
						value.Final = P3dHelper.GetRenderTexture(mat.Desc);
					}
					P3dPaintReplace.Blit(value.Final, value.Under, Color.white);
					TryBlendInto(ref value.Final, currentLayer, mat.Id, key);
					if (value.Above != null)
					{
						value.Final = P3dPaintFill.Blit(value.Final, P3dBlendMode.AlphaBlend, value.Above, Color.white, 1f, 0f);
					}
				}
			}
		}

		private static void TryBlendInto(ref RenderTexture renderTexture, Layer layer, int matId, P3dGroup group)
		{
			Image image = layer.GetImage(matId, group);
			RenderTexture renderTexture2 = image.Preview ?? image.Current;
			if (renderTexture2 != null)
			{
				renderTexture = P3dPaintFill.Blit(renderTexture, P3dBlendMode.AlphaBlend, renderTexture2, Color.white, layer.Opacity, 0f);
			}
		}

		public void Save()
		{
			foreach (Layer layer in Layers)
			{
				layer.Save();
			}
		}

		public void RemoveObj(Obj obj)
		{
			if (obj != null)
			{
				Objs.Remove(obj);
			}
		}

		public Mat GetMat(int matId)
		{
			foreach (Mat mat in Mats)
			{
				if (mat.Id == matId)
				{
					return mat;
				}
			}
			return null;
		}

		public bool MatNameValid(string name, int maxCount = 1)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			int num = 0;
			foreach (Mat mat in Mats)
			{
				if (mat.Name == name)
				{
					num++;
					if (num > maxCount)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void RemoveMat(Mat mat)
		{
			if (mat != null)
			{
				Mats.Remove(mat);
			}
		}

		public bool ObjExists(Transform source)
		{
			foreach (Obj obj in Objs)
			{
				if (obj.Transform == source)
				{
					return true;
				}
			}
			return false;
		}

		public Mat AddMat(Material material, P3dShaderTemplate template, int size)
		{
			int num = 0;
			Mat mat = null;
			for (int i = 0; i < Mats.Count; i++)
			{
				Mat mat2 = Mats[i];
				if (mat2.Id > num)
				{
					num = mat2.Id;
				}
				if (mat2.Material == material)
				{
					mat = mat2;
				}
			}
			if (mat == null)
			{
				mat = new Mat();
				mat.Id = num + 1;
				mat.Name = ((material != null) ? material.name : "New Material");
				mat.Material = material;
				mat.Width = size;
				mat.Height = size;
				Mats.Add(mat);
			}
			mat.Template = template;
			return mat;
		}

		public void AddObj(Transform source, Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Material[] materials, P3dShaderTemplate[] templates, int size)
		{
			Obj obj = new Obj();
			obj.Name = source.name;
			obj.Transform = source;
			obj.Mesh = mesh;
			obj.Position = position;
			obj.Rotation = rotation;
			obj.Scale = scale;
			for (int i = 0; i < materials.Length; i++)
			{
				Mat mat = AddMat(materials[i], templates[i], size);
				if (mat.Slots.Count == 0 && mat.Template != null)
				{
					foreach (P3dShaderTemplate.Slot slot2 in mat.Template.Slots)
					{
						Slot slot = new Slot();
						slot.Name = slot2.Name;
						mat.Slots.Add(slot);
					}
				}
				obj.MatIds.Add(mat.Id);
			}
			Objs.Add(obj);
		}

		public Cloner AddCloner(string name, Vector3 position, Vector3 euler)
		{
			Cloner cloner = new Cloner();
			cloner.Name = name;
			cloner.Position = position;
			cloner.Euler = euler;
			Cloners.Add(cloner);
			return cloner;
		}

		public void RemoveRepeater(Cloner repeater)
		{
			if (repeater != null)
			{
				Cloners.Remove(repeater);
			}
		}

		public Layer AddLayer()
		{
			Layer layer = new Layer();
			layer.Name = "New Layer";
			Layers.Add(layer);
			return layer;
		}

		public void RemoveLayer(Layer layer)
		{
			if (layer != null)
			{
				Layers.Remove(layer);
			}
		}
	}
}
