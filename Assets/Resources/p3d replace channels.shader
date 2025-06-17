Shader "Hidden/Paint in 3D/Replace Channels" {
	Properties {
		_TextureR ("Texture R", 2D) = "white" {}
		_TextureG ("Texture G", 2D) = "white" {}
		_TextureB ("Texture B", 2D) = "white" {}
		_TextureA ("Texture A", 2D) = "white" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}