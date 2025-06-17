Shader "Hidden/Paint in 3D/Sphere Blur" {
	Properties {
		_Hardness ("Hardness", Float) = 1
		_Opacity ("Opacity", Float) = 1
		_Squash ("Squash", Float) = 1
		_KernelSize ("Kernel Size", Float) = 0.01
		_Channel ("Channel", Vector) = (1,0,0,0)
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