using System;
using UnityEngine;

namespace PaintIn3D
{
	public static class P3dShader
	{
		public static int _BaseChannel;

		public static int _BaseParallaxMap;

		public static int _BaseParallaxMap_Transform;

		public static int _Buffer;

		public static int _Channel;

		public static int _ChannelR;

		public static int _ChannelG;

		public static int _ChannelB;

		public static int _ChannelA;

		public static int _Color;

		public static int _Direction;

		public static int _DstA;

		public static int _DstRGB;

		public static int _Hardness;

		public static int _ParallaxMap;

		public static int _ParallaxMap_Transform;

		public static int _ReplaceTexture;

		public static int _ReplaceColor;

		public static int _Squash;

		public static int _KernelSize;

		public static int _MainTex;

		public static int _Matrix;

		public static int _Minimum;

		public static int _NormalBack;

		public static int _NormalFront;

		public static int _Op;

		public static int _Opacity;

		public static int _Shape;

		public static int _ShapeChannel;

		public static int _SrcA;

		public static int _SrcRGB;

		public static int _Strength;

		public static int _Texture;

		public static int _TextureR;

		public static int _TextureG;

		public static int _TextureB;

		public static int _TextureA;

		public static int _Tiling;

		public static int _TileTexture;

		public static int _TileMatrix;

		public static int _TileBlend;

		public static int _Wrapping;

		static P3dShader()
		{
			_BaseChannel = Shader.PropertyToID("_BaseChannel");
			_BaseParallaxMap = Shader.PropertyToID("_BaseParallaxMap");
			_BaseParallaxMap_Transform = Shader.PropertyToID("_BaseParallaxMap_Transform");
			_Buffer = Shader.PropertyToID("_Buffer");
			_Channel = Shader.PropertyToID("_Channel");
			_ChannelR = Shader.PropertyToID("_ChannelR");
			_ChannelG = Shader.PropertyToID("_ChannelG");
			_ChannelB = Shader.PropertyToID("_ChannelB");
			_ChannelA = Shader.PropertyToID("_ChannelA");
			_Color = Shader.PropertyToID("_Color");
			_Direction = Shader.PropertyToID("_Direction");
			_DstA = Shader.PropertyToID("_DstA");
			_DstRGB = Shader.PropertyToID("_DstRGB");
			_Hardness = Shader.PropertyToID("_Hardness");
			_ParallaxMap = Shader.PropertyToID("_ParallaxMap");
			_ParallaxMap_Transform = Shader.PropertyToID("_ParallaxMap_Transform");
			_ReplaceTexture = Shader.PropertyToID("_ReplaceTexture");
			_ReplaceColor = Shader.PropertyToID("_ReplaceColor");
			_Squash = Shader.PropertyToID("_Squash");
			_KernelSize = Shader.PropertyToID("_KernelSize");
			_MainTex = Shader.PropertyToID("_MainTex");
			_Matrix = Shader.PropertyToID("_Matrix");
			_Minimum = Shader.PropertyToID("_Minimum");
			_NormalBack = Shader.PropertyToID("_NormalBack");
			_NormalFront = Shader.PropertyToID("_NormalFront");
			_Op = Shader.PropertyToID("_Op");
			_Opacity = Shader.PropertyToID("_Opacity");
			_Shape = Shader.PropertyToID("_Shape");
			_ShapeChannel = Shader.PropertyToID("_ShapeChannel");
			_SrcA = Shader.PropertyToID("_SrcA");
			_SrcRGB = Shader.PropertyToID("_SrcRGB");
			_Strength = Shader.PropertyToID("_Strength");
			_Texture = Shader.PropertyToID("_Texture");
			_TextureR = Shader.PropertyToID("_TextureR");
			_TextureG = Shader.PropertyToID("_TextureG");
			_TextureB = Shader.PropertyToID("_TextureB");
			_TextureA = Shader.PropertyToID("_TextureA");
			_Tiling = Shader.PropertyToID("_Tiling");
			_TileTexture = Shader.PropertyToID("_TileTexture");
			_TileMatrix = Shader.PropertyToID("_TileMatrix");
			_TileBlend = Shader.PropertyToID("_TileBlend");
			_Wrapping = Shader.PropertyToID("_Wrapping");
		}

		public static Shader Load(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				throw new Exception("Failed to find shader called: " + shaderName);
			}
			return shader;
		}

		public static Material Build(Shader shader)
		{
			return new Material(shader);
		}

		public static void BuildBlendMode(Material material, int index)
		{
			if (((uint)index & (true ? 1u : 0u)) != 0)
			{
				material.EnableKeyword("P3D_A");
			}
			if (((uint)index & 2u) != 0)
			{
				material.EnableKeyword("P3D_B");
			}
			if (((uint)index & 4u) != 0)
			{
				material.EnableKeyword("P3D_C");
			}
			if (((uint)index & 8u) != 0)
			{
				material.EnableKeyword("P3D_D");
			}
		}
	}
}
