Shader "Roma/Role/Self-Illumin_Diffuse-Role" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque"}
	LOD 100
	Fog { Mode Off }
	Lighting Off
CGPROGRAM

#pragma surface surf Lambert noforwardadd
#pragma multi_compile_fwdbase nodirlightmap

sampler2D _MainTex;
fixed4 _Color;

struct Input {
	fixed2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	//o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	o.Emission = c.rgb;
	o.Alpha = c.a;
}
ENDCG
} 
// 系统shader参数阴影必须使用
//Fallback "VertexLit"
//CustomEditor "LegacyIlluminShaderGUI"
}
