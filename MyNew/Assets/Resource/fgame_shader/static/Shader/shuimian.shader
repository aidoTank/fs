Shader "Unlit/shuimian"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FlowColor ("Flow Color (A)", Color) = (1, 1, 1, 1)
		_FlowTexture ("Flow Texture", 2D) = ""{}
		_FlowMap ("FlowMap (RG) Alpha (B)", 2D) = ""{}
		_FlowMapOffset("FlowMap Offset", Color) = (1, 1, 1, 1)
		_LightMap ("_Light", 2D) = "white" {}
		_factor("_factor", range(0, 10)) = 3
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
//			#pragma multi_compile_fog
			//#pragma target 2.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_FlowTexture : TEXCOORD1;
				float2 uv_FlowMap : TEXCOORD2;
				float2 uv_Light : TEXCOORD3;
			};

			struct v2f
			{
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_FlowTexture : TEXCOORD1;
				float2 uv_FlowMap : TEXCOORD2;
				float2 uv_Light : TEXCOORD3;
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

			sampler2D _LightMap;
			float4 _LightMap_ST;

			float _factor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
				o.uv_FlowTexture = TRANSFORM_TEX(v.uv_FlowTexture, _FlowTexture);
				o.uv_FlowMap = TRANSFORM_TEX(v.uv_FlowMap, _FlowMap);
				o.uv_Light = TRANSFORM_TEX(v.uv_Light, _LightMap);
				return o;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				half4 mainColor = tex2D (_MainTex, IN.uv_MainTex);

				half4 flowMap = tex2D (_FlowMap, IN.uv_FlowMap);
				flowMap.r = flowMap.r * 2 - 1;
				flowMap.g = flowMap.g * 2 - 1;

				float phase1 = _FlowMapOffset.x;
				float phase2 = _FlowMapOffset.y;

				half4 t1 = tex2D (_FlowTexture, IN.uv_FlowTexture + flowMap.rg * phase1);
				half4 t2 = tex2D (_FlowTexture, IN.uv_FlowTexture + flowMap.rg * phase2);
				half4 final = lerp( t1, t2, _FlowMapOffset.z);

				half flowMapColor = flowMap.b * _FlowColor.a;

				mainColor.rgb *= _BaseColor.rgb * (1 - flowMapColor);
				final.rgb *= _FlowColor.rgb * flowMapColor;
				mainColor.rgb = mainColor.rgb + final.rgb;

				fixed4 light = tex2D(_LightMap, IN.uv_Light);
				light.rgb = DecodeLightmap(light);

				mainColor.rgb *= light.rgb * _factor;
				mainColor *= _BaseColor;

				return mainColor;
			}
			ENDCG
		}
	}
}
