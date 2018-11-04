// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/magmaMove" {
	Properties {		
		_Color("Color",Color)= (1,1,1,1)
		_ColorBrightness("ColorBrightness",Color)=(1,1,1,1)
		_ColorDark("ColorDark",Color)=(1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}	
		_NoiseTex("NoiseTex",2D) = "white" {}
		_FlowBrightness("FlowBrightness",Float) = 1.49
		_FlowDark("FlowDark",Range(-1,0)) = -1
	}
	SubShader {
		
		Pass {		
			Tags{ "Queue"="Opaque" "RenderType"="Opaque"}				
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"	
						
			sampler2D _MainTex;
			float4 _MainTex_ST;	
			sampler2D _NoiseTex;	
			float4 _NoiseTex_ST;
			float4 _Color;
			float _FlowBrightness;
			float4 _ColorBrightness;
			float _FlowDark;
			float4 _ColorDark;	

			
			struct a2v {
				float4 vertex : POSITION;				
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float4 uv : TEXCOORD0;			
			};

			v2f vert(a2v v) {
				v2f f;	
				//uv流动			
				f.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;	
				f.uv.zw = v.texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw; 
			
				f.pos = UnityObjectToClipPos(v.vertex);							
				return f;
			}
			
			fixed4 frag(v2f f) : SV_Target {
				//根据UV流动贴图（透明度来区分亮部和暗部颜色）和噪音贴图计算颜色,注意暗部需要取反颜色才能正常显示
				fixed NoiseColor = tex2D(_NoiseTex,f.uv.zw).r;		
				fixed4 NoiseBrightness = _ColorBrightness * NoiseColor *_FlowBrightness;
				fixed4 NoiseDark =(1-_ColorDark) *(1-NoiseColor) *_FlowDark;
				//最终颜色
				fixed4 TexColor =_Color * tex2D(_MainTex,float2(f.uv.x,f.uv.y));	
				fixed4 FinColor = TexColor + NoiseBrightness + NoiseDark ;
				return fixed4(FinColor.rgb, 1);
			}			
			ENDCG
		}
	} 
	FallBack "Unlit/Texture"
}
