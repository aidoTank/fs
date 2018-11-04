Shader "Unlit/Flow7"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FlowColor ("Flow Color (A)", Color) = (1, 1, 1, 1)
		_FlowTexture ("Flow Texture", 2D) = ""{}
		_FlowMap ("FlowMap (RG) Alpha (B)", 2D) = ""{}
		_FlowMapOffset("FlowMap Offset", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" "IgnoreProjector" = "True"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			struct v2f
			{
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			fixed4 _BaseColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _FlowColor;
			sampler2D _FlowTexture;
			float4 _FlowTexture_ST;
			sampler2D _FlowMap;
			float4 _FlowMap_ST;
			float4 _FlowMapOffset;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _FlowTexture);
				o.uv2 = TRANSFORM_TEX(v.uv2, _FlowMap);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 mainColor = tex2D (_MainTex, i.uv0);

				half4 flowMap = tex2D (_FlowMap, i.uv2);
				flowMap.r = flowMap.r * 2 - 1;
				flowMap.g = flowMap.g * 2 - 1;

				float phase1 = _FlowMapOffset.x;
				float phase2 = _FlowMapOffset.y;

				half4 t1 = tex2D (_FlowTexture, i.uv1 + flowMap.rg * phase1);
				half4 t2 = tex2D (_FlowTexture, i.uv1 + flowMap.rg * phase2);
				half4 final = lerp( t1, t2, _FlowMapOffset.z);

				half flowMapColor = flowMap.b * _FlowColor.a;
				mainColor.rgb *= _BaseColor.rgb * (1 - flowMapColor);
				final.rgb *= _FlowColor.rgb * flowMapColor;
				// sample the texture
				mainColor.rgb = mainColor.rgb + final.rgb;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, mainColor);
				return mainColor;
			}
			ENDCG
		}
	}
}
