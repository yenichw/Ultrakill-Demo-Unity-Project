using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PaintIn3D
{
	[ExecuteInEditMode]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dSeamFixer")]
	public class P3dSeamFixer : ScriptableObject
	{
		private class Edge
		{
			public bool Used;

			public int IndexA;

			public int IndexB;

			public Vector2 CoordA;

			public Vector2 CoordB;
		}

		private struct Point
		{
			public int Index;

			public Vector2 Coord;
		}

		public static float DebugScale;

		[SerializeField]
		private Mesh source;

		[FormerlySerializedAs("channel")]
		[SerializeField]
		private P3dCoord coord;

		[SerializeField]
		private float threshold = 1E-06f;

		[SerializeField]
		private float border = 0.005f;

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

		[NonSerialized]
		private static List<Edge> edges = new List<Edge>();

		[NonSerialized]
		private static List<Point> points = new List<Point>();

		[NonSerialized]
		private static List<int> insertions = new List<int>();

		[NonSerialized]
		private static float areaThreshold;

		[NonSerialized]
		private static float coordThreshold;

		[NonSerialized]
		private static Vector2 startCoord;

		[NonSerialized]
		private static List<Vector3> deltaVertices = new List<Vector3>();

		[NonSerialized]
		private static List<Vector3> deltaNormals = new List<Vector3>();

		[NonSerialized]
		private static List<Vector3> deltaTangents = new List<Vector3>();

		[NonSerialized]
		private static Mesh currentSource;

		[NonSerialized]
		private static Mesh currentOutput;

		[NonSerialized]
		private static P3dCoord currentCoord;

		[NonSerialized]
		private static float currentBorder;

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

		public P3dCoord Coord
		{
			get
			{
				return coord;
			}
			set
			{
				coord = value;
			}
		}

		public float Threshold
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public float Border
		{
			get
			{
				return border;
			}
			set
			{
				border = value;
			}
		}

		[ContextMenu("Generate")]
		public void Generate()
		{
			Generate(source, ref mesh, coord, threshold, border);
		}

		protected virtual void OnDestroy()
		{
			P3dHelper.Destroy(mesh);
		}

		public static void Generate(Mesh source, ref Mesh output, P3dCoord coord, float threshold, float border)
		{
			if (!(source != null))
			{
				return;
			}
			if (output == null)
			{
				output = new Mesh();
			}
			currentOutput = output;
			currentSource = source;
			currentCoord = coord;
			currentBorder = border;
			areaThreshold = threshold * threshold;
			coordThreshold = threshold * threshold;
			output.Clear(keepVertexLayout: false);
			insertions.Clear();
			output.name = source.name + " (Fixed Seams)";
			output.bindposes = source.bindposes;
			output.bounds = source.bounds;
			output.subMeshCount = source.subMeshCount;
			source.GetBoneWeights(boneWeights);
			source.GetColors(colors);
			source.GetNormals(normals);
			source.GetTangents(tangents);
			source.GetUVs(0, coords0);
			source.GetUVs(1, coords1);
			source.GetUVs(2, coords2);
			source.GetUVs(3, coords3);
			source.GetVertices(positions);
			DoGenerate();
			output.boneWeights = boneWeights.ToArray();
			output.SetColors(colors);
			output.SetNormals(normals);
			output.SetTangents(tangents);
			output.SetUVs(0, coords0);
			output.SetUVs(1, coords1);
			output.SetUVs(2, coords2);
			output.SetUVs(3, coords3);
			if (source.blendShapeCount <= 0)
			{
				return;
			}
			Vector3[] collection = new Vector3[source.vertexCount];
			Vector3[] collection2 = new Vector3[source.vertexCount];
			Vector3[] collection3 = new Vector3[source.vertexCount];
			for (int i = 0; i < source.blendShapeCount; i++)
			{
				string blendShapeName = source.GetBlendShapeName(i);
				int blendShapeFrameCount = source.GetBlendShapeFrameCount(i);
				for (int j = 0; j < blendShapeFrameCount; j++)
				{
					source.GetBlendShapeFrameVertices(i, j, collection, collection2, collection3);
					deltaVertices.Clear();
					deltaNormals.Clear();
					deltaTangents.Clear();
					deltaVertices.AddRange(collection);
					deltaNormals.AddRange(collection2);
					deltaTangents.AddRange(collection3);
					for (int k = 0; k < insertions.Count; k++)
					{
						int index = insertions[k];
						deltaVertices.Add(deltaVertices[index]);
						deltaNormals.Add(deltaNormals[index]);
						deltaTangents.Add(deltaTangents[index]);
					}
					output.AddBlendShapeFrame(blendShapeName, source.GetBlendShapeFrameWeight(i, j), deltaVertices.ToArray(), deltaNormals.ToArray(), deltaTangents.ToArray());
				}
			}
		}

		private static void DoGenerate()
		{
			for (int i = 0; i < currentSource.subMeshCount; i++)
			{
				edges.Clear();
				currentSource.GetTriangles(indices, i);
				for (int j = 0; j < indices.Count; j += 3)
				{
					AddTriangle(indices[j], indices[j + 1], indices[j + 2]);
				}
				if (DebugScale > 0f)
				{
					for (int num = edges.Count - 1; num >= 0; num--)
					{
						Edge edge = edges[num];
						if (edge.Used)
						{
							Debug.DrawLine(edge.CoordA * DebugScale, edge.CoordB * DebugScale, new Color(1f, 1f, 1f, 0.5f), 1f);
						}
						else
						{
							Debug.DrawLine(edge.CoordA * DebugScale, edge.CoordB * DebugScale, new Color(1f, 1f, 1f, 0.25f), 1f);
						}
					}
				}
				for (int num2 = edges.Count - 1; num2 >= 0; num2--)
				{
					Edge edge2 = edges[num2];
					if (!edge2.Used)
					{
						edge2.Used = true;
						points.Clear();
						startCoord = edge2.CoordA;
						points.Add(new Point
						{
							Index = edge2.IndexA,
							Coord = edge2.CoordA
						});
						points.Add(new Point
						{
							Index = edge2.IndexB,
							Coord = edge2.CoordB
						});
						TraceEdges(edge2.CoordB);
					}
				}
				currentOutput.SetVertices(positions);
				currentOutput.SetTriangles(indices, i);
			}
		}

		private static void AddTriangle(int a, int b, int c)
		{
			Vector4 vector = default(Vector4);
			Vector4 vector2 = default(Vector4);
			Vector4 vector3 = default(Vector4);
			switch (currentCoord)
			{
			case P3dCoord.First:
				vector = coords0[a];
				vector2 = coords0[b];
				vector3 = coords0[c];
				break;
			case P3dCoord.Second:
				vector = coords1[a];
				vector2 = coords1[b];
				vector3 = coords1[c];
				break;
			case P3dCoord.Third:
				vector = coords2[a];
				vector2 = coords2[b];
				vector3 = coords2[c];
				break;
			case P3dCoord.Fourth:
				vector = coords3[a];
				vector2 = coords3[b];
				vector3 = coords3[c];
				break;
			}
			Vector2 vector4 = vector2 - vector;
			if (Vector3.Cross(rhs: (Vector2)(vector3 - vector), lhs: vector4).sqrMagnitude >= areaThreshold)
			{
				if ((vector2.x - vector.x) * (vector3.y - vector.y) - (vector3.x - vector.x) * (vector2.y - vector.y) >= 0f)
				{
					AddTriangle(a, b, c, vector, vector2, vector3);
				}
				else
				{
					AddTriangle(c, b, a, vector3, vector2, vector);
				}
			}
		}

		private static void AddTriangle(int a, int b, int c, Vector2 coordA, Vector2 coordB, Vector2 coordC)
		{
			RemoveOrAddEdge(a, b, coordA, coordB);
			RemoveOrAddEdge(b, c, coordB, coordC);
			RemoveOrAddEdge(c, a, coordC, coordA);
		}

		private static void RemoveOrAddEdge(int a, int b, Vector2 coordA, Vector2 coordB)
		{
			for (int num = edges.Count - 1; num >= 0; num--)
			{
				Edge edge = edges[num];
				if (Overlap(edge.CoordB - coordA) && Overlap(edge.CoordA - coordB))
				{
					edge.Used = true;
					return;
				}
			}
			Edge edge2 = new Edge();
			edge2.IndexA = a;
			edge2.IndexB = b;
			edge2.CoordA = coordA;
			edge2.CoordB = coordB;
			edges.Add(edge2);
		}

		private static void TraceEdges(Vector2 head)
		{
			bool flag = false;
			while (!flag)
			{
				flag = true;
				for (int num = edges.Count - 1; num >= 0; num--)
				{
					Edge edge = edges[num];
					if (!edge.Used && Overlap(head - edge.CoordA))
					{
						edge.Used = true;
						points.Add(new Point
						{
							Index = edge.IndexB,
							Coord = edge.CoordB
						});
						head = edge.CoordB;
						flag = false;
					}
				}
			}
			if (!Overlap(head - startCoord) || points.Count <= 2)
			{
				return;
			}
			Point point = points[1];
			Point point2 = points[2];
			points.Add(new Point
			{
				Index = point.Index,
				Coord = point.Coord
			});
			points.Add(new Point
			{
				Index = point2.Index,
				Coord = point2.Coord
			});
			int count = positions.Count;
			for (int i = 1; i < points.Count - 1; i++)
			{
				Point point3 = points[i - 1];
				Point point4 = points[i];
				Point point5 = points[i + 1];
				Vector2 normalized = (point3.Coord - point4.Coord).normalized;
				normalized = new Vector2(0f - normalized.y, normalized.x);
				Vector2 normalized2 = (point4.Coord - point5.Coord).normalized;
				normalized2 = new Vector2(0f - normalized2.y, normalized2.x);
				Vector2 vector = normalized + normalized2;
				float sqrMagnitude = vector.sqrMagnitude;
				Vector2 vector2 = point4.Coord;
				if (sqrMagnitude > 0f)
				{
					sqrMagnitude = Mathf.Sqrt(sqrMagnitude);
					vector2 += vector / sqrMagnitude * currentBorder;
				}
				AddVertex(point4.Index, vector2);
			}
			for (int j = 0; j < points.Count - 3; j++)
			{
				int index = points[j + 1].Index;
				int index2 = points[j + 2].Index;
				int num2 = count + j;
				int num3 = num2 + 1;
				indices.Add(index);
				indices.Add(index2);
				indices.Add(num2);
				indices.Add(num3);
				indices.Add(num2);
				indices.Add(index2);
				if (DebugScale > 0f)
				{
					switch (currentCoord)
					{
					case P3dCoord.First:
						Debug.DrawLine(coords0[index] * DebugScale, coords0[index2] * DebugScale, Color.green, 1f);
						Debug.DrawLine(coords0[num2] * DebugScale, coords0[num3] * DebugScale, Color.blue, 1f);
						break;
					case P3dCoord.Second:
						Debug.DrawLine(coords1[index] * DebugScale, coords1[index2] * DebugScale, Color.green, 1f);
						Debug.DrawLine(coords1[num2] * DebugScale, coords1[num3] * DebugScale, Color.blue, 1f);
						break;
					case P3dCoord.Third:
						Debug.DrawLine(coords2[index] * DebugScale, coords2[index2] * DebugScale, Color.green, 1f);
						Debug.DrawLine(coords2[num2] * DebugScale, coords2[num3] * DebugScale, Color.blue, 1f);
						break;
					case P3dCoord.Fourth:
						Debug.DrawLine(coords3[index] * DebugScale, coords3[index2] * DebugScale, Color.green, 1f);
						Debug.DrawLine(coords3[num2] * DebugScale, coords3[num3] * DebugScale, Color.blue, 1f);
						break;
					}
				}
			}
		}

		private static bool Overlap(Vector2 delta)
		{
			return Vector2.SqrMagnitude(delta) <= coordThreshold;
		}

		private static void AddVertex(int index, Vector2 coord)
		{
			if (boneWeights.Count > 0)
			{
				boneWeights.Add(boneWeights[index]);
			}
			if (colors.Count > 0)
			{
				colors.Add(colors[index]);
			}
			if (normals.Count > 0)
			{
				normals.Add(normals[index]);
			}
			if (tangents.Count > 0)
			{
				tangents.Add(tangents[index]);
			}
			if (coords0.Count > 0)
			{
				coords0.Add(coords0[index]);
			}
			if (coords1.Count > 0)
			{
				coords1.Add(coords1[index]);
			}
			if (coords2.Count > 0)
			{
				coords2.Add(coords2[index]);
			}
			if (coords3.Count > 0)
			{
				coords3.Add(coords3[index]);
			}
			switch (currentCoord)
			{
			case P3dCoord.First:
				if (coords0.Count > 0)
				{
					coords0[coords0.Count - 1] = coord;
				}
				break;
			case P3dCoord.Second:
				if (coords1.Count > 0)
				{
					coords1[coords1.Count - 1] = coord;
				}
				break;
			case P3dCoord.Third:
				if (coords2.Count > 0)
				{
					coords2[coords2.Count - 1] = coord;
				}
				break;
			case P3dCoord.Fourth:
				if (coords3.Count > 0)
				{
					coords3[coords3.Count - 1] = coord;
				}
				break;
			}
			positions.Add(positions[index]);
			insertions.Add(index);
		}
	}
}
