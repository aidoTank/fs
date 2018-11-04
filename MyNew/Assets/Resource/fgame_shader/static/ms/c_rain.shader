// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/rain" {
	Properties {		
		_Color("Color",Color)= (1,1,1,1)
		_Brightness("Brightness",Float) = 0.8
		_MainTex("Main Tex", 2D) = "white" {}	
		_UvFlowX("UvFlowX",Float)=0
		_UvFlowY("UvFlowY",Float)=0		
		_AlphaCenterX("AlphaCenterX",Float) = 52
		_AlphaCenterY("AlphaCenterY",Float) = 52
		_AlphaDistance("AlphaDistance",Float) = 11
		_AlphaValue("AlphaValue",Float) = 8
		_AlphaOp("AlphaOp",Range(0,1))=0.7
		
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
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
			float _Brightness;
			float _UvFlowX;
			float _UvFlowY;
			float _AlphaCenterX;
			float _AlphaCenterY;
			float _AlphaDistance;
			float _AlphaValue;
			float _AlphaOp;
			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float2 uv : TEXCOORD0;		
				float3 worldPos: TEXCOORD1;				
			};
			
			v2f vert(a2v v) {
				v2f f;				
				f.uv = TRANSFORM_TEX(v.texcoord, _MainTex)+ frac(float2((_UvFlowX),(_UvFlowY))*_Time.y);	
				f.worldPos = mul(unity_ObjectToWorld,v.vertex);				
				f.pos = UnityObjectToClipPos(v.vertex);				
				return f;
			}
			
			fixed4 frag(v2f f) : SV_Target {
				//主纹理	
				fixed4 TexColor =_Color * tex2D(_MainTex,f.uv);
				//通过中心算一个距离
				float colorA = distance(float3(_AlphaCenterX,0,_AlphaCenterY),f.worldPos.xyz);	
				//通过距离算透明值
				float distanceAlpha = lerp(-_AlphaDistance,_AlphaDistance,colorA*12*0.001)+_AlphaValue;
				fixed alpha =distanceAlpha;	

				fixed3 FinColor =_Brightness * TexColor.rgb;
				fixed FinAlpha = alpha * TexColor.a;
				return fixed4(FinColor,FinAlpha);
			}			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
