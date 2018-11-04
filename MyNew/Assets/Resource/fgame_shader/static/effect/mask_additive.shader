Shader "FGame/Particles/mask_additive"
{
	Properties
	{
		_Color("Color",Color) = (.5,.5,.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("MaskTex",2D) = ""{}
		_Brightness("Brightness",Range(0,4)) = .5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		Blend ONE ONE
		Cull OFF
		ZWrite OFF
		Lighting OFF

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			fixed4 _Color;
			half _Brightness;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv,_MaskTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Brightness;
				fixed4 MaskCol = tex2D(_MaskTex,i.uv1);
				return col * _Color * MaskCol;
			}
			ENDCG
		}
	}
}
