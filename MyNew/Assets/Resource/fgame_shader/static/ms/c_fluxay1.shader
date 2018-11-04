// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/fluxay" {
	Properties {		
		_Color("Color",Color)= (1,1,1,1)
		_Brightness("Brightness",Float) = 0.8
		_MainTex ("Main Tex", 2D) = "white" {}		
		_FluxayTex("FluxayTex",2D) = "white"{}
		_FluxayColor("FluxayColor",Color)=(1,1,1,1)
		_FluxayFloat("FluxayFloat",Float) = 0.18
		_ReflectColor ("Reflection Color", Color) = (1, 1, 1, 1)		
		_CubeMap ("Reflection Cubemap", Cube) = "_Skybox" {}
		_CubeBrightness ("CubeBrightness",Float) = 0.13
		_UvFlowX("UvFlowX",Float) = 2
		_UvFlowY("UvFlowY",Float) = 8
		_UvFlow("UvFlow",Vector) = (0.25,0.15,0.88,0.74)
	}
	SubShader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass {			

			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"	
						
			sampler2D _MainTex;
			float4 _MainTex_ST;		
			float4 _Color;	
			fixed4 _ReflectColor;	
			sampler2D _FluxayTex;
			samplerCUBE _CubeMap;	
			float _CubeBrightness;	
			float _Gloss;
			float _FresnelScale;	
			float _UvFlowX;
			float _UvFlowY;
			float4 _UvFlow;
			float _FluxayFloat;
			float4 _FluxayColor;
			float _Brightness;
			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
				float4 vertexColor: COLOR0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float2 uv : TEXCOORD0;			
				float3 worldPos: TEXCOORD1;				
				fixed3 worldViewDir : TEXCOORD3;
				fixed3 worldNormal:TEXCOORD2;
				fixed3 worldRefr : TEXCOORD4;
			};
			
			v2f vert(a2v v) {
				v2f f;				
				f.uv = TRANSFORM_TEX(v.texcoord, _MainTex);				
				f.pos = UnityObjectToClipPos(v.vertex);
				f.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
				f.worldNormal = UnityObjectToWorldNormal(v.normal);	
				f.worldViewDir = UnityWorldSpaceViewDir(f.worldPos);	
				f.worldRefr = reflect(-f.worldViewDir, f.worldNormal);						
				return f;
			}
			
			fixed4 frag(v2f f) : SV_Target {
			    //UV流光流动
				float2 center = f.uv - _UvFlow.xy;
				float Uvx = f.uv.x + center * sin(length(center * _UvFlowX) - _Time.y*_UvFlow.z)*_UvFlow.w;	
				float Uvy = f.uv.y + center * sin(length(center * _UvFlowY) - _Time.y*_UvFlow.z)*_UvFlow.w;		
				fixed TexFluxay = tex2D(_FluxayTex,float2(Uvx,Uvy)).a;
				//主纹理	
				fixed4 TexColor =_Color * tex2D(_MainTex,f.uv);
				//假反射
				fixed3 Reflection = _CubeBrightness * texCUBE(_CubeMap, f.worldRefr).rgb * _ReflectColor.rgb;
				fixed3 FinColor =_Brightness * (TexColor.rgb + Reflection + TexFluxay * _FluxayFloat * _FluxayColor.rgb);
				return fixed4(FinColor,TexColor.a);
			}			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
