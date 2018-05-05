// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Roma/Model/OutLine2" {  
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Shininess ("Shininess", Range (0.01, 1)) = 0.3
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0,1)) = 0.5

	 }
	SubShader {
		Tags { "RenderType"="Opaque" "IgnoreProjector"="true"}
		LOD 200
   
		Pass {
		Tags { "LightMode" = "ForwardBase" }
     
		CGPROGRAM
     
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		sampler2D _MainTex;
		fixed4 _MainTex_ST;
		fixed _Shininess; 
		fixed4 _RimColor;
		fixed _RimPower;

		struct a2v {
			fixed4 vertex : POSITION;  
			fixed3 normal : NORMAL;   
			fixed4 texcoord : TEXCOORD0;
		};
     
		struct v2f {
			fixed4 pos : POSITION; 
			fixed2 uv : TEXCOORD0;
			fixed3 lightDir: TEXCOORD1; 
			fixed3 viewDir : TEXCOORD2; 
			fixed3 normal : TEXCOORD3;
		};
     
		v2f vert(a2v v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			//o.lightDir = _WorldSpaceLightPos0.xyz;
			//o.viewDir = WorldSpaceViewDir(v.vertex);
			//o.normal = normalize(UnityObjectToWorldNormal(v.normal));

			o.lightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
			o.viewDir = ObjSpaceViewDir(v.vertex);
			o.normal = normalize(v.normal);
			return o;
		}
     
		fixed4 frag(v2f i) : COLOR {
			fixed4 texColor = tex2D(_MainTex, i.uv);
			fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 diff = max(0, dot(i.normal, i.lightDir));
			fixed3 h = normalize(i.lightDir + i.viewDir);
			fixed nh = max(0, dot(i.normal,h));
			fixed spec = pow(nh,_Shininess*128);
			fixed rim = 1.0f - saturate(dot(normalize(i.viewDir), i.normal));
			texColor.rgb = fixed3((ambi + diff + spec + 0.3f) * texColor) +  _RimColor * pow(rim, _RimPower * 4.0f);
			texColor.a = 1.0f;
			return texColor;
		}
		ENDCG
		}
	}
	//FallBack "Roma/Model/Self-Illumin_Diffuse"
}
    