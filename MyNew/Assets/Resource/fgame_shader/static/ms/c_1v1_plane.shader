Shader "FGame/static/ms/1v1_plane" {
	Properties {		
		_Color("Color",Color)= (1,1,1,1)	
		_MainTex("MainTex", 2D) = "white" {}
		_MaskEdgeTex("MaskEdgeTex",2D) = ""{}
		_MaskTex("MaskCol",2D) = ""{}
		// _MaskWaveTex("MaskWaveTex",2D) = ""{}
		//_EdgeColor("_EdgeColor",Color) = (1,1,1,1)
		_WorldPosCtrl("_WorldPosCtrl",Range(-1,1)) = 0
		//_RampCtrl("_RampCtrl",Range(-10,10)) = 0	
		_Brightness("Brightness",Float) = 1	
		// _WaveCtrlX("WaveCtrlX",Range(-2,2)) = 0
		// _WaveCtrlY("WaveCtrlY",Range(-2,2)) = 0
	}
	SubShader {
		
		Pass {		
			Tags{ "Queue"="Opaque" "RenderType"="Opaque"}	
			Blend SrcAlpha OneMinusSrcAlpha 
			//Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"	
						
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			sampler2D _MaskEdgeTex;
			float4 _MaskEdgeTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			// sampler2D _MaskWaveTex;
			// float4 _MaskWaveTex_ST;
			float4 _Color;	
			//float4 _EdgeColor;
			half _Brightness;
			//half _RampCtrl;	
			half _WorldPosCtrl;
			// half _WaveCtrlX;
			// half _WaveCtrlY;
			
			struct a2v {
				float4 vertex : POSITION;				
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float2 uv1 : TEXCOORD2;		
				//float2 uv2 : TEXCOORD3;
			};

			v2f vert(a2v v) {
				v2f f;			
				f.pos = UnityObjectToClipPos(v.vertex);	
				f.uv = TRANSFORM_TEX(v.texcoord , _MainTex);
				f.worldPos = mul(unity_ObjectToWorld, v.vertex);	
				f.uv1 = TRANSFORM_TEX(v.texcoord, _MaskTex);	
				//f.uv2 = TRANSFORM_TEX(v.texcoord, _MaskWaveTex); 				
				return f;
			}
			
			fixed4 frag(v2f f) : SV_Target {
				fixed4 TexColor = tex2D(_MainTex,f.uv) * _Color * _Brightness;
				fixed WorldPosCtrl = f.worldPos.y + _WorldPosCtrl; 
				fixed4 WorldPos = fixed4(f.worldPos.x, WorldPosCtrl, f.worldPos.z, f.worldPos.w);	
				fixed4 MaskTexCol = tex2D(_MaskEdgeTex,WorldPos);
				//fixed2 WaveUV = fixed2(f.uv2.x + _WaveCtrlX * _Time.x, f.uv2.y + _WaveCtrlY * _Time.y);
				//fixed4 MaskWave = tex2D(_MaskWaveTex, WaveUV) ;
				//fixed4 MaskEdgeWave = tex2D(_MaskTex,f.uv1) * MaskWave;
				//fixed Alpha = TexColor.a * MaskWave * MaskEdgeWave;
				TexColor.a *= MaskTexCol.a;
				//fixed3 col = TexColor.rgb * MaskTexCol.rgb + MaskEdgeWave.rgb;
				//fixed4 MaskCol = tex2D(_MaskEdgeTex,f.uv1);
				//fixed4 RampCol = lerp(TexColor,_EdgeColor,_RampCtrl);
				return TexColor * MaskTexCol ;
			}			
			ENDCG
		}
	} FallBack "Unlit/Texture"
}
	