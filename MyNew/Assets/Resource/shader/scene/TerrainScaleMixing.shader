Shader "Roma/Scene/Terrain Scale Mixing" {
Properties {
	_Color("Main Color", Color) = (1,1,1,1)
	_Scale ("Scale", Range (0.07, 10)) = 4
	_Brightness ("Brightness", Range (0.5, 4)) = 2

	_Control ("Control (RGBA)", 2D) = "white" {}
	//_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
}
                
SubShader {
	Tags {
   		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf Lambert noforwardadd
//#pragma surface surf BlinnPhong 

struct Input {
	fixed2 uv_MainTex;
	fixed2 uv_Control : TEXCOORD0;
	fixed2 uv_Splat0 : TEXCOORD1;
	fixed2 uv_Splat1 : TEXCOORD2;
	fixed2 uv_Splat2 : TEXCOORD3;
	//float2 uv_Splat3 : TEXCOORD4;
};
 
fixed4 _Color;
sampler2D _Control;
sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
fixed _Scale, _Brightness;
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
	fixed4 col = fixed4(0.0f,0.0f,0.0f,0.0f);
	// col = c_r * rgb * rgb * 1
	col  = splat_control.r  * tex2D (_Splat0, IN.uv_Splat0 * 1/_Scale) * _Brightness * _Color;
	col += splat_control.g  * tex2D (_Splat1, IN.uv_Splat1 * 1/_Scale) * _Brightness * _Color;
	col += splat_control.b  * tex2D (_Splat2, IN.uv_Splat2 * 1/_Scale) * _Brightness * _Color;
	//col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3) * tex2D (_Splat3, IN.uv_Splat3 * 1/_Scale) * _Brightness;
	o.Albedo = col.rgb;
	o.Alpha = 1.0;
}
ENDCG 
}
// Fallback to Diffuse
Fallback "Diffuse"
}
