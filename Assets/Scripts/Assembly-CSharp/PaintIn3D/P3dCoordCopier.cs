using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dCoordCopier")]
	public class P3dCoordCopier : ScriptableObject
	{
		[SerializeField]
		private Mesh source;

		[SerializeField]
		private Mesh mesh;

		[NonSerialized]
		private static List<BoneWeight> boneWeights = new List<BoneWeight>();

		[NonSerialized]
		private static List<Color32> colors = new List<Color32>();

		[NonSerialized]
		private static List<Vector3> positions = new List<Vector3>();

		[NonSerialized]
		private static List<Vector3> normals = new List<Vector3>();

		[NonSerialized]
		private static List<Vector4> tangents = new List<Vector4>();

		[NonSerialized]
		private static List<Vector4> coords0 = new List<Vector4>();

		[NonSerialized]
		private static List<Vector4> coords1 = new List<Vector4>();

		[NonSerialized]
		private static List<Vector4> coords2 = new List<Vector4>();

		[NonSerialized]
		private static List<Vector4> coords3 = new List<Vector4>();

		[NonSerialized]
		private static List<int> indices = new List<int>();

		public Mesh Source
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}

		public void Generate()
		{
			if (!(source != null))
			{
				return;
			}
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			mesh.Clear(keepVertexLayout: false);
			mesh.name = source.name + " (Copied Coords)";
			mesh.bindposes = source.bindposes;
			mesh.bounds = source.bounds;
			mesh.subMeshCount = source.subMeshCount;
			source.GetBoneWeights(boneWeights);
			source.GetColors(colors);
			source.GetNormals(normals);
			source.GetTangents(tangents);
			source.GetUVs(0, coords0);
			source.GetUVs(1, coords1);
			source.GetUVs(2, coords2);
			source.GetUVs(3, coords3);
			source.GetVertices(positions);
			mesh.SetVertices(positions);
			for (int i = 0; i < source.subMeshCount; i++)
			{
				source.GetTriangles(indices, i);
				mesh.SetTriangles(indices, i);
			}
			mesh.boneWeights = boneWeights.ToArray();
			mesh.SetColors(colors);
			mesh.SetNormals(normals);
			mesh.SetTangents(tangents);
			mesh.SetUVs(0, coords1);
			mesh.SetUVs(1, coords1);
			mesh.SetUVs(2, coords2);
			mesh.SetUVs(3, coords3);
			if (source.blendShapeCount <= 0)
			{
				return;
			}
			Vector3[] deltaVertices = new Vector3[source.vertexCount];
			Vector3[] deltaNormals = new Vector3[source.vertexCount];
			Vector3[] deltaTangents = new Vector3[source.vertexCount];
			for (int j = 0; j < source.blendShapeCount; j++)
			{
				string blendShapeName = source.GetBlendShapeName(j);
				int blendShapeFrameCount = source.GetBlendShapeFrameCount(j);
				for (int k = 0; k < blendShapeFrameCount; k++)
				{
					source.GetBlendShapeFrameVertices(j, k, deltaVertices, deltaNormals, deltaTangents);
					mesh.AddBlendShapeFrame(blendShapeName, source.GetBlendShapeFrameWeight(j, k), deltaVertices, deltaNormals, deltaTangents);
				}
			}
		}

		protected virtual void OnDestroy()
		{
			P3dHelper.Destroy(mesh);
		}
	}
}
