// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_simple_desert" {
	Properties {		
		_Color("Color",Color) = (1,1,1,1)
		_AlphaCenterX("AlphaCenterX",Float) = 52
		_AlphaCenterY("AlphaCenterY",Float) = 52
		_AlphaValue("AlphaValue",Float) = 8
		_AlphaDistance("AlphaDistance",Float) = 11
		_AlphaOp("AlphaOp",Range(0,1))=0.7

	}
	SubShader {
		Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass {			

			ZWrite Off
			Cull back
			Blend SrcAlpha OneMinusSrcAlpha			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"
					
			fixed4 _Color;
			half _AlphaValue;
			float _AlphaCenterX;
			float _AlphaCenterY;
			float _AlphaOp;
			float _AlphaDistance;

			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos: TEXCOORD1;			
			};
			
			v2f vert(a2v v) {
				v2f o;	
				UNITY_INITIALIZE_OUTPUT(v2f, o);				
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);		
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {	
				
				float colorA = distance(float3(_AlphaCenterX,0,_AlphaCenterY),i.worldPos.xyz);	
				//通过距离算透明值
				float distanceAlpha = lerp(-_AlphaDistance,_AlphaDistance,colorA*12*0.001) + _AlphaValue;
				fixed alpha = min(_AlphaOp,distanceAlpha);			
				return fixed4(_Color.rgb,alpha);
			}			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
