Shader "FGame/static/ms/diffuse_cull_off" {
	Properties {		
		_Color("Color",Color)= (1,1,1,1)	
		_MainTex("MainTex", 2D) = "white" {}	
		_Brightness("Brightness",Float) = 1	
	}
	SubShader {
		
		Pass {		
			Tags{ "Queue"="Opaque" "RenderType"="Opaque"}	
			Cull OFF	
					
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"	
						
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			float4 _Color;	
			float _Brightness;	
			
			struct a2v {
				float4 vertex : POSITION;				
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float2 uv : TEXCOORD0;			
			};

			v2f vert(a2v v) {
				v2f f;	
				f.pos = UnityObjectToClipPos(v.vertex);	
				f.uv = TRANSFORM_TEX(v.texcoord,_MainTex);						
				return f;
			}
			
			fixed4 frag(v2f f) : SV_Target {
				fixed3 TexColor =_Color.rgb * tex2D(_MainTex,f.uv).rgb;	
				return fixed4(_Brightness * TexColor,1);
			}			
			ENDCG
		}
	} FallBack "Unlit/Texture"
}
	