Shader "Hidden/Paint in 3D/Fill" {
	Properties {
		_Buffer ("Buffer", 2D) = "black" {}
		_ReplaceTexture ("Replace Texture", 2D) = "white" {}
		_Texture ("Texture", 2D) = "white" {}
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