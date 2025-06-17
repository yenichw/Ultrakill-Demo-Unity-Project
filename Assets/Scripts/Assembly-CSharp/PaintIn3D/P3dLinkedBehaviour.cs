using System;
using UnityEngine;

namespace PaintIn3D
{
	public abstract class P3dLinkedBehaviour<T> : MonoBehaviour where T : P3dLinkedBehaviour<T>
	{
		[NonSerialized]
		public static T FirstInstance;

		[NonSerialized]
		public static int InstanceCount;

		[NonSerialized]
		public T PrevInstance;

		[NonSerialized]
		public T NextInstance;

		protected virtual void OnEnable()
		{
			T val = (T)this;
			if (FirstInstance != null)
			{
				FirstInstance.PrevInstance = val;
				PrevInstance = null;
				NextInstance = FirstInstance;
			}
			else
			{
				PrevInstance = null;
				NextInstance = null;
			}
			FirstInstance = val;
			InstanceCount++;
		}

		protected virtual void OnDisable()
		{
			if (FirstInstance == this)
			{
				FirstInstance = NextInstance;
				if (NextInstance != null)
				{
					NextInstance.PrevInstance = null;
				}
			}
			else
			{
				if (NextInstance != null)
				{
					NextInstance.PrevInstance = PrevInstance;
				}
				PrevInstance.NextInstance = NextInstance;
			}
			InstanceCount--;
		}
	}
}
