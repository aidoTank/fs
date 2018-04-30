Shader "Roma/Role/Self-Illumin_Diffuse_Shadow_Arms" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Shadow ("shadow val", Range (0.0, 1.0)) = 0.4
		_HVal("H", Range(0.0, 1.0)) = 0		// 调整色相值
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
		 fixed _HVal;

		 struct Input {
			fixed2 uv_MainTex;
		 };

		 fixed4 LightingLambertTest(SurfaceOutput s, fixed3 lightDir, fixed atten)
		 {
			  fixed diff = max(_Shadow, dot(s.Normal, lightDir));
			  fixed4 c;
			  c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
			  c.a = s.Alpha;
			  return c;
		 }

		 fixed3 rgb2hsv(fixed3 c) {
			 fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			 fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
			 fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

			 fixed d = q.x - min(q.w, q.y);
			 fixed e = 1.0e-10;
			 return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		 }

		 fixed3 hsv2rgb(fixed3 c) {
			 c = fixed3(c.x, clamp(c.yz, 0.0, 1.0));
			 fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			 fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			 return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		 }


		 void surf (Input IN, inout SurfaceOutput o) {
			  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			  fixed3 hsv = rgb2hsv(c.rgb);
			  fixed affectMult = step(0, hsv.r) * step(hsv.r, 1);
			  // _HVal是将xyz优化为一个变量
			  fixed3 rgb = hsv2rgb(hsv + fixed3(_HVal,0,0) * affectMult);

			  o.Albedo = rgb;
			  o.Alpha = c.a;
			  o.Emission = rgb * 0.5f;
		}
		ENDCG
	}
	// 系统shader参数阴影必须使用
	//Fallback "VertexLit"
}