using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PaintIn3D
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(P3dPaintable))]
	[HelpURL("http://carloswilkes.github.io/Documentation/PaintIn3D#P3dPaintableTexture")]
	[AddComponentMenu("Paint in 3D/Paintable Texture")]
	public class P3dPaintableTexture : P3dLinkedBehaviour<P3dPaintableTexture>
	{
		public enum StateType
		{
			None = 0,
			FullTextureCopy = 1,
			LocalCommandCopy = 2
		}

		public enum MipType
		{
			Auto = 0,
			ForceOn = 1,
			ForceOff = 2
		}

		[Serializable]
		public class PaintableTextureEvent : UnityEvent<P3dPaintableTexture>
		{
		}

		[SerializeField]
		private P3dSlot slot = new P3dSlot(0, "_MainTex");

		[FormerlySerializedAs("channel")]
		[SerializeField]
		private P3dCoord coord;

		[SerializeField]
		private P3dGroup group;

		[SerializeField]
		private StateType state;

		[SerializeField]
		private int stateLimit;

		[SerializeField]
		private string saveName;

		[SerializeField]
		private string shaderKeyword;

		[SerializeField]
		private RenderTextureFormat format;

		[SerializeField]
		private MipType mipMaps;

		[SerializeField]
		private int width = 512;

		[SerializeField]
		private int height = 512;

		[SerializeField]
		private Color color = Color.white;

		[SerializeField]
		private Texture texture;

		[NonSerialized]
		private P3dPaintable cachedPaintable;

		[NonSerialized]
		private bool cachedPaintableSet;

		[SerializeField]
		private bool activated;

		[SerializeField]
		private RenderTexture current;

		[SerializeField]
		private RenderTexture preview;

		[SerializeField]
		private bool previewSet;

		[NonSerialized]
		private List<P3dPaintableState> paintableStates = new List<P3dPaintableState>();

		[NonSerialized]
		private int stateIndex;

		[NonSerialized]
		private P3dPaintable paintable;

		[NonSerialized]
		private bool paintableSet;

		[NonSerialized]
		private Material material;

		[NonSerialized]
		private bool materialSet;

		[NonSerialized]
		private List<P3dCommand> paintCommands = new List<P3dCommand>();

		[NonSerialized]
		private List<P3dCommand> previewCommands = new List<P3dCommand>();

		[NonSerialized]
		private List<P3dCommand> localCommands = new List<P3dCommand>();

		[NonSerialized]
		private static List<P3dPaintableTexture> tempPaintableTextures = new List<P3dPaintableTexture>();

		public P3dSlot Slot
		{
			get
			{
				return slot;
			}
			set
			{
				slot = value;
			}
		}

		public P3dCoord Channel
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

		public P3dGroup Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public StateType State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
			}
		}

		public int StateLimit
		{
			get
			{
				return stateLimit;
			}
			set
			{
				stateLimit = value;
			}
		}

		public string SaveName
		{
			get
			{
				return saveName;
			}
			set
			{
				saveName = value;
			}
		}

		public string ShaderKeyword
		{
			get
			{
				return shaderKeyword;
			}
			set
			{
				shaderKeyword = value;
			}
		}

		public RenderTextureFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		public MipType MipMaps
		{
			get
			{
				return mipMaps;
			}
			set
			{
				mipMaps = value;
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public Texture Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
			}
		}

		public bool Activated => activated;

		public bool CanUndo
		{
			get
			{
				if (state != 0)
				{
					return stateIndex > 0;
				}
				return false;
			}
		}

		public bool CanRedo
		{
			get
			{
				if (state != 0)
				{
					return stateIndex < paintableStates.Count - 1;
				}
				return false;
			}
		}

		public List<P3dPaintableState> States => paintableStates;

		public int StateIndex => stateIndex;

		public P3dPaintable CachedPaintable
		{
			get
			{
				if (!cachedPaintableSet)
				{
					cachedPaintable = GetComponent<P3dPaintable>();
					cachedPaintableSet = true;
				}
				return cachedPaintable;
			}
		}

		public RenderTexture Current
		{
			get
			{
				return current;
			}
			set
			{
				if (materialSet)
				{
					current = value;
					material.SetTexture(slot.Name, current);
				}
			}
		}

		public RenderTexture Preview => preview;

		public bool CommandsPending => paintCommands.Count + previewCommands.Count > 0;

		public event Action<P3dCommand> OnAddCommand;

		public event Action<bool> OnModified;

		public static List<P3dPaintableTexture> Filter(P3dModel model, P3dGroup group)
		{
			tempPaintableTextures.Clear();
			if (model.Paintable != null)
			{
				List<P3dPaintableTexture> paintableTextures = model.Paintable.PaintableTextures;
				for (int num = paintableTextures.Count - 1; num >= 0; num--)
				{
					P3dPaintableTexture p3dPaintableTexture = paintableTextures[num];
					if ((int)p3dPaintableTexture.group == (int)group)
					{
						tempPaintableTextures.Add(p3dPaintableTexture);
					}
				}
			}
			return tempPaintableTextures;
		}

		[ContextMenu("Clear States")]
		public void ClearStates()
		{
			if (paintableStates != null)
			{
				for (int num = paintableStates.Count - 1; num >= 0; num--)
				{
					paintableStates[num].Pool();
				}
				paintableStates.Clear();
				stateIndex = 0;
			}
		}

		[ContextMenu("Store State")]
		public void StoreState()
		{
			if (activated)
			{
				if (stateIndex != paintableStates.Count - 1)
				{
					TrimFuture();
					AddState();
				}
				if (state == StateType.FullTextureCopy)
				{
					TrimPast();
				}
				stateIndex = paintableStates.Count;
			}
		}

		[ContextMenu("Undo")]
		public void Undo()
		{
			if (CanUndo)
			{
				if (stateIndex == paintableStates.Count)
				{
					AddState();
				}
				ClearCommands();
				stateIndex--;
				switch (state)
				{
				case StateType.FullTextureCopy:
				{
					P3dPaintableState p3dPaintableState = paintableStates[stateIndex];
					P3dHelper.Blit(current, p3dPaintableState.Texture);
					break;
				}
				case StateType.LocalCommandCopy:
					RebuildFromCommands();
					break;
				}
				NotifyOnModified(preview: false);
			}
		}

		[ContextMenu("Redo")]
		public void Redo()
		{
			if (CanRedo)
			{
				ClearCommands();
				stateIndex++;
				switch (state)
				{
				case StateType.FullTextureCopy:
				{
					P3dPaintableState p3dPaintableState = paintableStates[stateIndex];
					P3dHelper.Blit(current, p3dPaintableState.Texture);
					NotifyOnModified(preview: false);
					break;
				}
				case StateType.LocalCommandCopy:
					RebuildFromCommands();
					break;
				}
			}
		}

		private void RebuildFromCommands()
		{
			P3dPaintReplace.Blit(current, texture, color);
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			for (int i = 0; i <= stateIndex; i++)
			{
				P3dPaintableState p3dPaintableState = paintableStates[i];
				int count = p3dPaintableState.Commands.Count;
				for (int j = 0; j < count; j++)
				{
					P3dCommand p3dCommand = p3dPaintableState.Commands[j].SpawnCopy();
					p3dCommand.SetLocation(localToWorldMatrix * p3dCommand.Matrix);
					AddCommand(p3dCommand);
				}
			}
			ExecuteCommands(sendNotifications: false);
			NotifyOnModified(preview: false);
		}

		private void AddState()
		{
			P3dPaintableState p3dPaintableState = P3dPaintableState.Pop();
			switch (state)
			{
			case StateType.FullTextureCopy:
				p3dPaintableState.Write(current);
				break;
			case StateType.LocalCommandCopy:
				p3dPaintableState.Write(localCommands);
				localCommands.Clear();
				break;
			}
			paintableStates.Add(p3dPaintableState);
		}

		private void TrimFuture()
		{
			for (int num = paintableStates.Count - 1; num >= stateIndex; num--)
			{
				paintableStates[num].Pool();
				paintableStates.RemoveAt(num);
			}
		}

		private void TrimPast()
		{
			for (int num = paintableStates.Count - stateLimit - 1; num >= 0; num--)
			{
				paintableStates[num].Pool();
				paintableStates.RemoveAt(num);
			}
		}

		public void NotifyOnModified(bool preview)
		{
			if (this.OnModified != null)
			{
				this.OnModified(preview);
			}
		}

		public Texture2D GetReadableCopy()
		{
			return P3dHelper.GetReadableCopy(current);
		}

		public byte[] GetPngData()
		{
			Texture2D readableCopy = GetReadableCopy();
			if (readableCopy != null)
			{
				byte[] result = readableCopy.EncodeToPNG();
				P3dHelper.Destroy(readableCopy);
				return result;
			}
			return null;
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			Clear(texture, color);
		}

		public void Clear(Texture texture, Color tint)
		{
			if (activated)
			{
				P3dPaintReplace.Blit(current, texture, tint);
			}
		}

		[ContextMenu("Replace")]
		public void Replace()
		{
			Replace(texture, color);
		}

		public void Replace(Texture texture, Color tint)
		{
			if (texture != null)
			{
				Resize(texture.width, texture.height, copyContents: false);
			}
			else
			{
				Resize(width, height, copyContents: false);
			}
			Clear(texture, tint);
		}

		public bool Resize(int width, int height, bool copyContents = true)
		{
			if (activated && (current.width != width || current.height != height))
			{
				RenderTextureDescriptor descriptor = current.descriptor;
				descriptor.width = width;
				descriptor.height = height;
				RenderTexture renderTexture = P3dHelper.GetRenderTexture(descriptor);
				if (copyContents)
				{
					P3dPaintReplace.Blit(renderTexture, current, Color.white);
					if (renderTexture.useMipMap)
					{
						renderTexture.GenerateMips();
					}
				}
				P3dHelper.ReleaseRenderTexture(current);
				current = renderTexture;
				return true;
			}
			return false;
		}

		[ContextMenu("Save")]
		public void Save()
		{
			Save(saveName);
		}

		public void Save(string saveName)
		{
			if (activated && !string.IsNullOrEmpty(saveName))
			{
				P3dHelper.SaveBytes(saveName, GetPngData());
			}
		}

		[ContextMenu("Load")]
		public void Load()
		{
			Load(saveName);
		}

		public void Load(string saveName, bool replace = true)
		{
			if (activated)
			{
				LoadFromData(P3dHelper.LoadBytes(saveName));
			}
		}

		public void LoadFromData(byte[] data, bool allowResize = true)
		{
			if (data != null && data.Length != 0)
			{
				Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false, QualitySettings.activeColorSpace == ColorSpace.Linear);
				texture2D.LoadImage(data);
				if (allowResize)
				{
					Replace(texture2D, Color.white);
				}
				else
				{
					Clear(texture2D, Color.white);
				}
				P3dHelper.Destroy(texture2D);
			}
		}

		public void HidePreview()
		{
			if (activated && current != null && material != null)
			{
				material.SetTexture(slot.Name, current);
			}
		}

		public static void HideAllPreviews()
		{
			P3dPaintableTexture p3dPaintableTexture = P3dLinkedBehaviour<P3dPaintableTexture>.FirstInstance;
			for (int i = 0; i < P3dLinkedBehaviour<P3dPaintableTexture>.InstanceCount; i++)
			{
				p3dPaintableTexture.HidePreview();
				p3dPaintableTexture = p3dPaintableTexture.NextInstance;
			}
		}

		[ContextMenu("Clear Save")]
		public void ClearSave()
		{
			P3dHelper.ClearSave(saveName);
		}

		public static void ClearSave(string saveName)
		{
			P3dHelper.ClearSave(saveName);
		}

		[ContextMenu("Update Material")]
		public void UpdateMaterial()
		{
			material = P3dHelper.GetMaterial(base.gameObject, slot.Index);
			materialSet = true;
		}

		[ContextMenu("Copy Size")]
		public void CopySize()
		{
			Texture texture = Slot.FindTexture(base.gameObject);
			if (texture != null)
			{
				width = texture.width;
				height = texture.height;
			}
		}

		[ContextMenu("Copy Texture")]
		public void CopyTexture()
		{
			Texture = Slot.FindTexture(base.gameObject);
		}

		[ContextMenu("Activate")]
		public void Activate()
		{
			if (activated)
			{
				return;
			}
			UpdateMaterial();
			if (material != null)
			{
				int num = width;
				int num2 = height;
				Texture texture = material.GetTexture(slot.Name);
				CachedPaintable.ScaleSize(ref num, ref num2);
				if (this.texture != null)
				{
					texture = this.texture;
				}
				if (!string.IsNullOrEmpty(shaderKeyword))
				{
					material.EnableKeyword(shaderKeyword);
				}
				RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, format, 0);
				desc.autoGenerateMips = false;
				if (mipMaps == MipType.ForceOn)
				{
					desc.useMipMap = true;
				}
				else if (mipMaps == MipType.Auto && P3dHelper.HasMipMaps(texture))
				{
					desc.useMipMap = true;
				}
				current = P3dHelper.GetRenderTexture(desc);
				P3dPaintReplace.Blit(current, texture, color);
				if (current.useMipMap)
				{
					current.GenerateMips();
				}
				material.SetTexture(slot.Name, current);
				activated = true;
				if (!string.IsNullOrEmpty(saveName))
				{
					Load();
				}
				NotifyOnModified(preview: false);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (!paintableSet)
			{
				paintable = GetComponent<P3dPaintable>();
				paintableSet = true;
			}
			paintable.Register(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			paintable.Unregister(this);
		}

		protected virtual void OnDestroy()
		{
			if (activated)
			{
				if (!string.IsNullOrEmpty(saveName))
				{
					Save();
				}
				P3dHelper.ReleaseRenderTexture(current);
				P3dHelper.ReleaseRenderTexture(preview);
				ClearStates();
			}
		}

		public void AddCommand(P3dCommand command)
		{
			if (command.Preview)
			{
				previewCommands.Add(command);
			}
			else
			{
				paintCommands.Add(command);
				if (state == StateType.LocalCommandCopy && !command.Preview)
				{
					P3dCommand item = command.SpawnCopyLocal(base.transform);
					localCommands.Add(item);
				}
			}
			if (this.OnAddCommand != null)
			{
				this.OnAddCommand(command);
			}
		}

		public void ClearCommands()
		{
			for (int num = previewCommands.Count - 1; num >= 0; num--)
			{
				previewCommands[num].Pool();
			}
			previewCommands.Clear();
			for (int num2 = paintCommands.Count - 1; num2 >= 0; num2--)
			{
				paintCommands[num2].Pool();
			}
			paintCommands.Clear();
			for (int num3 = localCommands.Count - 1; num3 >= 0; num3--)
			{
				localCommands[num3].Pool();
			}
			localCommands.Clear();
		}

		public void ExecuteCommands(bool sendNotifications)
		{
			if (!activated)
			{
				return;
			}
			bool flag = true;
			if (CommandsPending)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture swap = P3dHelper.GetRenderTexture(current.descriptor);
				Mesh preparedMesh = null;
				Matrix4x4 preparedMatrix = default(Matrix4x4);
				P3dPaintReplace.BlitFast(swap, current, Color.white);
				if (paintCommands.Count > 0)
				{
					ExecuteCommands(paintCommands, sendNotifications, ref current, ref swap, ref preparedMesh, ref preparedMatrix);
				}
				if (previewCommands.Count > 0)
				{
					if (!previewSet)
					{
						preview = P3dHelper.GetRenderTexture(current.descriptor);
						previewSet = true;
					}
					flag = false;
					preview.DiscardContents();
					Graphics.Blit(current, preview);
					previewCommands.Sort(P3dCommand.Compare);
					ExecuteCommands(previewCommands, sendNotifications, ref preview, ref swap, ref preparedMesh, ref preparedMatrix);
				}
				P3dHelper.ReleaseRenderTexture(swap);
				RenderTexture.active = active;
			}
			if (flag && previewSet)
			{
				P3dHelper.ReleaseRenderTexture(preview);
				preview = null;
				previewSet = false;
			}
			if (!materialSet)
			{
				UpdateMaterial();
			}
			material.SetTexture(slot.Name, previewSet ? preview : current);
		}

		private void ExecuteCommands(List<P3dCommand> commands, bool sendNotifications, ref RenderTexture main, ref RenderTexture swap, ref Mesh preparedMesh, ref Matrix4x4 preparedMatrix)
		{
			RenderTexture.active = main;
			for (int i = 0; i < commands.Count; i++)
			{
				P3dCommand p3dCommand = commands[i];
				RenderTexture.active = swap;
				p3dCommand.Apply(main);
				if (p3dCommand.RequireMesh)
				{
					p3dCommand.Model.GetPrepared(ref preparedMesh, ref preparedMatrix);
					P3dHelper.Draw(p3dCommand.Material, preparedMesh, preparedMatrix, slot.Index, coord);
				}
				else
				{
					P3dHelper.Draw(p3dCommand.Material);
				}
				P3dHelper.Swap(ref main, ref swap);
				p3dCommand.Pool();
			}
			commands.Clear();
			if (main.useMipMap)
			{
				main.GenerateMips();
			}
			if (sendNotifications)
			{
				NotifyOnModified(commands == previewCommands);
			}
		}
	}
}
