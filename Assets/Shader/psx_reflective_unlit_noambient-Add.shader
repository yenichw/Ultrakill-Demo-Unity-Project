Shader "psx/reflective/unlit_noambient-Add" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube ("Cubemap", Cube) = "" {}
		_TextureWarping ("Texture Warping", Range(0, 1)) = 1
		_VertexWarping ("Vertex Warping", Range(4, 10000)) = 160
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}