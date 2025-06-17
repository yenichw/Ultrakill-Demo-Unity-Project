using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[DisallowMultipleComponent]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintableManager")]
	[AddComponentMenu("Paint in 3D/Paintable Manager")]
	public class P3dPaintableManager : P3dLinkedBehaviour<P3dPaintableManager>
	{
		[NonSerialized]
		public static int MatrixCount;

		[NonSerialized]
		public static int ClonerCount;

		[NonSerialized]
		private static List<Matrix4x4> tempMatrices = new List<Matrix4x4>();

		[NonSerialized]
		private static List<IClone> tempCloners = new List<IClone>();

		[NonSerialized]
		private static List<IModify> tempModifiers = new List<IModify>();

		public static P3dPaintableManager GetOrCreateInstance()
		{
			if (P3dLinkedBehaviour<P3dPaintableManager>.InstanceCount == 0)
			{
				GameObject obj = new GameObject(typeof(P3dPaintableManager).Name);
				obj.hideFlags = HideFlags.DontSave;
				obj.AddComponent<P3dPaintableManager>();
			}
			return P3dLinkedBehaviour<P3dPaintableManager>.FirstInstance;
		}

		public static Material[] BuildMaterialsBlendModes(string shaderName)
		{
			Shader shader = P3dShader.Load(shaderName);
			Material[] array = new Material[11];
			for (int i = 0; i < 11; i++)
			{
				Material material = P3dShader.Build(shader);
				P3dShader.BuildBlendMode(material, i);
				array[i] = material;
			}
			return array;
		}

		public static Material BuildMaterial(string shaderName)
		{
			return P3dShader.Build(P3dShader.Load(shaderName));
		}

		public static void BuildModifiers(GameObject gameObject)
		{
			gameObject.GetComponents(tempModifiers);
		}

		public static void ModifyColor(float pressure, ref Color color)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyColor modifyColor)
				{
					modifyColor.ModifyColor(pressure, ref color);
				}
			}
		}

		public static void ModifyAngle(float pressure, ref float angle)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyAngle modifyAngle)
				{
					modifyAngle.ModifyAngle(pressure, ref angle);
				}
			}
		}

		public static void ModifyOpacity(float pressure, ref float opacity)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyOpacity modifyOpacity)
				{
					modifyOpacity.ModifyOpacity(pressure, ref opacity);
				}
			}
		}

		public static void ModifyHardness(float pressure, ref float hardness)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyHardness modifyHardness)
				{
					modifyHardness.ModifyHardness(pressure, ref hardness);
				}
			}
		}

		public static void ModifyRadius(float pressure, ref float radius)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyRadius modifyRadius)
				{
					modifyRadius.ModifyRadius(pressure, ref radius);
				}
			}
		}

		public static void ModifyTexture(float pressure, ref Texture texture)
		{
			for (int i = 0; i < tempModifiers.Count; i++)
			{
				if (tempModifiers[i] is IModifyTexture modifyTexture)
				{
					modifyTexture.ModifyTexture(pressure, ref texture);
				}
			}
		}

		public static void SubmitAll(P3dCommand command, bool preview, int priority, int layerMask, P3dGroup group, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			command.Model = null;
			command.Group = group;
			command.Preview = preview;
			command.Priority = priority;
			DoSubmitAll(command, layerMask, group, model, paintableTexture);
			BuildCloners(command.Matrix);
			for (int i = 0; i < ClonerCount; i++)
			{
				for (int j = 0; j < MatrixCount; j++)
				{
					command.SetLocation(Clone(i, j));
					DoSubmitAll(command, layerMask, group, model, paintableTexture);
				}
			}
		}

		private static void DoSubmitAll(P3dCommand command, int layerMask, P3dGroup group, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			if (model != null)
			{
				if (paintableTexture != null)
				{
					Submit(command, model, paintableTexture);
				}
				else
				{
					SubmitAll(command, model, group);
				}
			}
			else if (paintableTexture != null)
			{
				Submit(command, paintableTexture.CachedPaintable, paintableTexture);
			}
			else
			{
				SubmitAll(command, layerMask, group);
			}
		}

		private static void SubmitAll(P3dCommand command, int layerMask, P3dGroup group)
		{
			List<P3dModel> list = P3dModel.FindOverlap(command.Position, command.Radius, layerMask);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				SubmitAll(command, list[num], group);
			}
		}

		private static void SubmitAll(P3dCommand command, P3dModel model, P3dGroup group)
		{
			List<P3dPaintableTexture> list = P3dPaintableTexture.Filter(model, group);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				Submit(command, model, list[num]);
			}
		}

		public static void Submit(P3dCommand command, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			Submit(command, model, paintableTexture, command.Preview, command.Priority);
		}

		public static void Submit(P3dCommand command, P3dModel model, P3dPaintableTexture paintableTexture, bool preview, int priority)
		{
			P3dCommand p3dCommand = command.SpawnCopy();
			p3dCommand.Model = model;
			p3dCommand.Group = 0;
			p3dCommand.Preview = preview;
			p3dCommand.Priority = priority;
			if (p3dCommand.Blend.Index == 8)
			{
				p3dCommand.Blend.Color = paintableTexture.Color;
				p3dCommand.Blend.Texture = paintableTexture.Texture;
			}
			paintableTexture.AddCommand(p3dCommand);
		}

		public static void BuildCloners(Matrix4x4 matrix, List<IClone> cloners = null)
		{
			tempMatrices.Clear();
			tempCloners.Clear();
			tempMatrices.Add(matrix);
			if (cloners != null)
			{
				for (int i = 0; i < cloners.Count; i++)
				{
					IClone clone = cloners[i];
					if (clone != null)
					{
						tempCloners.Add(clone);
					}
				}
			}
			else
			{
				P3dClone p3dClone = P3dLinkedBehaviour<P3dClone>.FirstInstance;
				for (int j = 0; j < P3dLinkedBehaviour<P3dClone>.InstanceCount; j++)
				{
					tempCloners.Add(p3dClone);
					p3dClone = p3dClone.NextInstance;
				}
			}
			MatrixCount = 1;
			ClonerCount = tempCloners.Count;
		}

		public static Matrix4x4 Clone(int clonerIndex, int matrixIndex)
		{
			if (matrixIndex == 0)
			{
				MatrixCount = tempMatrices.Count;
			}
			Matrix4x4 matrix4x = tempCloners[clonerIndex].Transform(tempMatrices[matrixIndex]);
			tempMatrices.Add(matrix4x);
			return matrix4x;
		}

		protected virtual void LateUpdate()
		{
			if (this == P3dLinkedBehaviour<P3dPaintableManager>.FirstInstance && P3dLinkedBehaviour<P3dModel>.InstanceCount > 0)
			{
				ClearAll();
				UpdateAll();
			}
			else
			{
				P3dHelper.Destroy(base.gameObject);
			}
		}

		private void ClearAll()
		{
			P3dModel p3dModel = P3dLinkedBehaviour<P3dModel>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dModel>.InstanceCount; i++)
			{
				p3dModel.Prepared = false;
				p3dModel = p3dModel.NextInstance;
			}
		}

		private void UpdateAll()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.ExecuteCommands(sendNotifications: true);
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}
	}
}
