Shader "FGame/static/ms/diffuse" {
	Properties {		
		_TintColor("Color",Color)= (1,1,1,1)	
		_MainTex("MainTex", 2D) = "white" {}	
		_Brightness("Brightness",Float) = 1	
	}
	SubShader {
		
		Pass {		
			Tags{ "Queue"="Opaque" "RenderType"="Opaque"}	
			Name "DIFFUSE"	
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"	

			sampler2D _MainTex;
			float4 _MainTex_ST;			
			float4 _TintColor;	
			float _Brightness;	
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			
			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor * _Brightness;
				return col;
			}			
			ENDCG
		}
	} 
}
	