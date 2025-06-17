using System;
using UnityEngine;

namespace PaintIn3D
{
	[Serializable]
	public struct P3dBlendMode
	{
		public const int ALPHA_BLEND = 0;

		public const int ALPHA_BLEND_INVERSE = 1;

		public const int ALPHA_BLEND_RGB = 2;

		public const int ADDITIVE = 3;

		public const int ADDITIVE_SOFT = 4;

		public const int SUBTRACTIVE = 5;

		public const int SUBTRACTIVE_SOFT = 6;

		public const int REPLACE = 7;

		public const int REPLACE_ORIGINAL = 8;

		public const int REPLACE_CUSTOM = 9;

		public const int MULTIPLY_INVERSE_RGB = 10;

		public const int COUNT = 11;

		public static readonly string[] NAMES = new string[11]
		{
			"Alpha Blend", "Alpha Blend Inverse", "Alpha Blend RGB", "Additive", "Additive Soft", "Subtractive", "Subtractive Soft", "Replace", "Replace Original", "Replace Custom",
			"Multiply RGB Inverse"
		};

		public int Index;

		public Texture Texture;

		public Color Color;

		public static P3dBlendMode AlphaBlend
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 0;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode AlphaBlendInverse
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 1;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode AlphaBlendRGB
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 2;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode Additive
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 3;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode AdditiveSoft
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 4;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode Subtractive
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 5;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode SubtractiveSoft
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 6;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode Replace
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 7;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode ReplaceOriginal
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 8;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode MultiplyInverseRGB
		{
			get
			{
				P3dBlendMode result = default(P3dBlendMode);
				result.Index = 10;
				result.Color = Color.white;
				return result;
			}
		}

		public static P3dBlendMode ReplaceCustom(Color color, Texture texture)
		{
			P3dBlendMode result = default(P3dBlendMode);
			result.Index = 9;
			result.Color = color;
			result.Texture = texture;
			return result;
		}

		public void Apply(Material material)
		{
			if (Index == 9 || Index == 8)
			{
				material.SetColor(P3dShader._ReplaceColor, Color);
				material.SetTexture(P3dShader._ReplaceTexture, Texture);
			}
		}

		public static string GetName(int index)
		{
			if (index >= 0 && index < 11)
			{
				return NAMES[index];
			}
			return null;
		}

		public static bool operator ==(P3dBlendMode a, int b)
		{
			return a.Index == b;
		}

		public static bool operator !=(P3dBlendMode a, int b)
		{
			return a.Index != b;
		}

		public static implicit operator int(P3dBlendMode a)
		{
			return a.Index;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}
