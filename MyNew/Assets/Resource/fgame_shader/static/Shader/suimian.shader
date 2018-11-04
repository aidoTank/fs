Shader "Unlit/suimian"
{
	Properties
	{
		_Color("_Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_FlowTex("Flow", 2D) = "white" {}
		_FlowIndex("FlowIndex", Range(0, 1.1)) = 0.5

		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.000001, 9.0)) = 0.1
		_RimIntensity("_RimIntensity", Range(0.0, 3)) = 0.5

		_FlowColor("Flow Color (A)", Color) = (1, 1, 1, 1)
		_FlowTexture("Flow Texture", 2D) = ""{}
		_FlowMap("FlowMap (RG) Alpha (B)", 2D) = ""{}
		_FlowMapOffset("FlowMap Offset", Vector) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float2 uv_FlowTexture: TEXCOORD1;
				float2 uv_FlowMap: TEXCOORD2;
				UNITY_FOG_COORDS(3)
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			float _FlowIndex;

			fixed4 _RimColor;
			float _RimPower;
			uniform float _RimIntensity;

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
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				o.uv_FlowTexture = TRANSFORM_TEX(v.uv, _FlowTexture);
				o.uv_FlowMap = TRANSFORM_TEX(v.uv, _FlowMap);
				UNITY_TRANSFER_FOG(o,o.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				_FlowTex_ST.x = 1 / _FlowTex_ST.x;
				_FlowTex_ST.y = 1 / _FlowTex_ST.y;
				o.uv.zw = float2((worldPos.x + _FlowTex_ST.z) * _FlowTex_ST.x, (worldPos.z + _FlowTex_ST.w) * _FlowTex_ST.y);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				col *= _Color;

				float4 flow = tex2D(_FlowTex, i.uv.zw);
				float diff = flow.w - _FlowIndex;
				diff += 0.1;
				if (diff > 0) {
					//col.w = diff;
					diff = pow(diff, 1 / _RimPower) *_RimIntensity;
					col = col * (1 - diff) + _RimColor * diff;
					//col.w = 1 - diff;
				}

				half4 flowMap = tex2D(_FlowMap, i.uv_FlowMap);
				flowMap.r = flowMap.r * 2 - 1;
				flowMap.g = flowMap.g * 2 - 1;

				float phase1 = _FlowMapOffset.x;
				float phase2 = _FlowMapOffset.y;

				half4 t1 = tex2D(_FlowTexture, i.uv_FlowTexture + flowMap.rg * phase1);
				half4 t2 = tex2D(_FlowTexture, i.uv_FlowTexture + flowMap.rg * phase2);
				half4 final = lerp(t1, t2, _FlowMapOffset.z);

				half flowMapColor = flowMap.b * _FlowColor.a;

				col.rgb = col.rgb * (1 - flowMapColor) + _FlowColor * final.rgb;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
