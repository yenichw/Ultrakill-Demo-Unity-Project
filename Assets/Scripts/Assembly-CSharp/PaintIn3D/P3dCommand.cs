using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dCommand
	{
		public P3dModel Model;

		public P3dGroup Group;

		public bool Preview;

		public int Priority;

		public int Index;

		public P3dBlendMode Blend;

		public Material Material;

		public Matrix4x4 Matrix;

		public Vector3 Position;

		public float Radius;

		public abstract bool RequireMesh { get; }

		public static int Compare(P3dCommand a, P3dCommand b)
		{
			int num = a.Priority.CompareTo(b.Priority);
			if (num > 0)
			{
				return 1;
			}
			if (num < 0)
			{
				return -1;
			}
			return a.Index.CompareTo(b.Index);
		}

		public virtual void SetLocation(Matrix4x4 matrix)
		{
			Matrix = matrix;
			Position = matrix.MultiplyPoint(Vector3.zero);
		}

		public void Apply(RenderTexture buffer)
		{
			Apply();
			Material.SetTexture(P3dShader._Buffer, buffer);
		}

		public abstract void Apply();

		public abstract void Pool();

		public abstract P3dCommand SpawnCopy();

		public P3dCommand SpawnCopyLocal(Transform transform)
		{
			P3dCommand p3dCommand = SpawnCopy();
			p3dCommand.Matrix = transform.worldToLocalMatrix * p3dCommand.Matrix;
			return p3dCommand;
		}

		public P3dCommand SpawnCopyWorld(Transform transform)
		{
			P3dCommand p3dCommand = SpawnCopy();
			p3dCommand.SetLocation(transform.localToWorldMatrix * p3dCommand.Matrix);
			return p3dCommand;
		}

		protected T SpawnCopy<T>(Stack<T> pool) where T : P3dCommand, new()
		{
			T obj = ((pool.Count > 0) ? pool.Pop() : new T());
			obj.Model = Model;
			obj.Group = Group;
			obj.Preview = Preview;
			obj.Blend = Blend;
			obj.Material = Material;
			obj.Matrix = Matrix;
			obj.Position = Position;
			obj.Radius = Radius;
			return obj;
		}
	}
}
