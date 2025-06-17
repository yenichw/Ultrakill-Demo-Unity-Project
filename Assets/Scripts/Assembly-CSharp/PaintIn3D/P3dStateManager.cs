namespace PaintIn3D
{
	public static class P3dStateManager
	{
		public static bool CanUndo
		{
			get
			{
				P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
				for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
				{
					if (p3dPaintableTexture.CanUndo)
					{
						return true;
					}
					p3dPaintableTexture = p3dPaintableTexture.NextInstance;
				}
				return false;
			}
		}

		public static bool CanRedo
		{
			get
			{
				P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
				for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
				{
					if (p3dPaintableTexture.CanRedo)
					{
						return true;
					}
					p3dPaintableTexture = p3dPaintableTexture.NextInstance;
				}
				return false;
			}
		}

		public static void StoreAllStates()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.StoreState();
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}

		public static void ClearAllStates()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.ClearStates();
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}

		public static void UndoAll()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.Undo();
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}

		public static void RedoAll()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.Redo();
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}
	}
}
