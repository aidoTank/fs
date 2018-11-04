// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/damageCircle" {
	Properties {		
		_Color("Color",Color) = (1,1,1,1)
		_AlphaValue("AlphaValue",Float) = 3		
		_AlphaValueSub("AlphaValueSub",Float) = 3
		_AlphaDistance("AlphaDistance",Float) = 30
		_AlphaFloat("AlphaFloat",Float) = 1
		_AlphaCenterX("AlphaCenterX",Float) = 52
		_AlphaCenterY("AlphaCenterY",Float) = 52
	}
	SubShader {
		Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass {			

			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
					
		
			float4 _Color;	
			float _AlphaValue;	
			float _AlphaCenterX;
			float _AlphaCenterY;
			float _AlphaValueSub;
			float _AlphaFloat;
			float _AlphaDistance;
			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float4 uv : TEXCOORD2;
				float3 worldPos: TEXCOORD1;
			};
			
			v2f vert(a2v v) {
				v2f o;			
				UNITY_INITIALIZE_OUTPUT(v2f,o);					
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);		
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {

				float colorA = distance(float3(_AlphaCenterX,0,_AlphaCenterY),i.worldPos.xyz);

				float distanceAlpha = lerp(-_AlphaDistance,_AlphaDistance,colorA*12*0.001);
				float distanceAlpha1 = lerp(_AlphaDistance,-_AlphaDistance,colorA*12*0.001);
				float alpha =  (distanceAlpha-_AlphaValue);
				float alpha1 = (distanceAlpha1-_AlphaValueSub);
				float finAlpha = alpha * alpha1 * _AlphaFloat;
				//最终颜色
				fixed3 finColor = _Color.rgb;		
				return fixed4(finColor,finAlpha);
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
