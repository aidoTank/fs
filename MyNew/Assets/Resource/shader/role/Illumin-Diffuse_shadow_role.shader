Shader "Roma/Role/Self-Illumin_Diffuse_Shadow_Role" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Shadow ("shadow val", Range (0.0, 1.0)) = 0.4
	}
	SubShader {
		 Tags { "RenderType"="Opaque" "IgnoreProjector"="true"}
		 LOD 200
			LOD 100
			Fog { Mode Off }
			Lighting Off

		 CGPROGRAM
		 #pragma surface surf LambertTest noforwardadd
		 #pragma multi_compile_fwdbase nodirlightmap
		 fixed _Shadow;
		 sampler2D _MainTex;
		 fixed4 _Color;
		 struct Input {
			fixed2 uv_MainTex;
		 };

		 fixed4 LightingLambertTest(SurfaceOutput s, float3 lightDir, float atten)
		 {
			  fixed diff = max(_Shadow, dot(s.Normal, lightDir));
			  fixed4 c;
			  c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
			  c.a = s.Alpha;
			  return c;
		 }


		 void surf (Input IN, inout SurfaceOutput o) {
			  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			  o.Albedo = c.rgb;
			  o.Alpha = c.a;
			  o.Emission = c.rgb * 0.5f;
		}
		ENDCG
	}
	// 系统shader参数阴影必须使用
	//Fallback "VertexLit"
}