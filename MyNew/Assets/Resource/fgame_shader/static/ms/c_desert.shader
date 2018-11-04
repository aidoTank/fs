// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/desert" {
	Properties {		
		_MainTex ("MainTex", 2D) = "white" {}
		_NoiseTex("NoiseTex",2D) = "white" {}		
		_SequenceTex("SequenceTex",2D) = "white" {}
		_SequenceWarpTex("SequenceWarpTex",2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_SequenceColor("SequenceColor",Color) = (1,1,1,1)
		_UvFlowX("UvFlowX",Float) = 2
		_UvFlowY("UvFlowY",Float) = 8
		_HorizontalAmount ("Horizontal Amount", Float) = 4
    	_VerticalAmount ("Vertical Amount", Float) = 4
		_SequenceSpeed ("SequenceSpeed", Range(1, 100)) = 30
		_BlurValue("BlurValue",Range(0,1)) = 1
		_AddParticle("AddParticle",Range(-2,2))=0
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
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"
					
			sampler2D _MainTex;
			float4 _MainTex_ST;	
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;	
			sampler2D _SequenceTex;	
			float4 _SequenceTex_ST;	
			sampler2D _SequenceWarpTex;	
			float4 _SequenceWarpTex_ST;	
			float _UvFlowX;
			float _UvFlowY;
			fixed _AddParticle;	
			fixed4 _Color;
			half _AlphaValue;
			float _AlphaCenterX;
			float _AlphaCenterY;
			float _AlphaOp;
			float _AlphaDistance;
			float _HorizontalAmount;
			float _VerticalAmount;
			float _SequenceSpeed;
			fixed4 _SequenceColor;
			fixed _BlurValue;

			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float4 uv : TEXCOORD2;
				float3 worldPos: TEXCOORD1;
				float4 circleUv: TEXCOORD3;
				float4 sequenceTexUv: TEXCOORD4;
			};
			
			v2f vert(a2v v) {
				v2f o;	
				UNITY_INITIALIZE_OUTPUT(v2f, o);				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _NoiseTex) + frac(float2((_UvFlowX),(_UvFlowY))*_Time.y);
				o.circleUv.xy = v.texcoord.xy;

				o.sequenceTexUv.xy = TRANSFORM_TEX(v.texcoord, _SequenceTex);
				o.sequenceTexUv.zw = TRANSFORM_TEX(v.texcoord, _SequenceWarpTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);		

				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {	
				//扭曲	
				fixed4 sequenceColor = tex2D(_SequenceWarpTex, i.sequenceTexUv.zw);					
				//UV流动效果
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed4 noiseColor = tex2D(_NoiseTex, i.uv.zw+texColor.r);	
				//序列
				float time = floor(_Time.y * _SequenceSpeed);  
				float row = floor(time / _HorizontalAmount);
				float column = time - row * _HorizontalAmount;			
				half2 uv = i.sequenceTexUv.xy + half2(column, -row);
				uv.x /=  _HorizontalAmount;
				uv.y /= _VerticalAmount;	
				fixed blurValue = min(sequenceColor.r,_BlurValue);	
				fixed4 sequenceTex = tex2D(_SequenceTex, uv + texColor.r) * _SequenceColor*blurValue;				
				//通过中心算一个距离
				float colorA = distance(float3(_AlphaCenterX,0,_AlphaCenterY),i.worldPos.xyz);	
				//通过距离算透明值
				float distanceAlpha = lerp(-_AlphaDistance,_AlphaDistance,colorA*12*0.001)+_AlphaValue;
				fixed alpha =min(_AlphaOp,distanceAlpha);	
		
				//最终颜色
				fixed3 finColor =sequenceTex + _Color.rgb * texColor.rgb   + _Color.rgb * texColor.rgb*_AddParticle + noiseColor.r*0.05;
				//fixed3 finColor =tureCircleColor +_Color.rgb* noiseColor.r *_AddParticle;				
				return fixed4(finColor,alpha);
			}			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
