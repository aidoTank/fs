Shader "Roma/Model/Stone" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		CGPROGRAM

		#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;

		struct Input {
			fixed2 uv_MainTex;
			fixed2 uv_Illum;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed grey = dot(tex.rgb, fixed3(0.22, 0.707, 0.071));
			o.Albedo = grey;
		}
		ENDCG
	}
		//FallBack "Legacy Shaders/Self-Illumin/VertexLit"
		//CustomEditor "LegacyIlluminShaderGUI"
}
