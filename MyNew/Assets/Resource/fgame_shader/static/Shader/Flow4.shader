// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Flow4"
{
	Properties
	{
		_Color("_Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_LightTex("Light", 2D) = "black" {}
		_FlowTex("Flow", 2D) = "white" {}
		_FlowIndex("FlowIndex", Range(0, 1.1)) = 0.5
		_OffsetY("OffsetY", float) = 6
		_factor("_factor", range(0, 10)) = 3

		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.000001, 3.0)) = 0.1
		_RimIntensity("_RimIntensity", Range(0.0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			};

			struct v2f
			{
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _LightTex;
			float4 _LightTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			float _FlowIndex;
			float _OffsetY;
			float _factor;

			fixed4 _RimColor;
			float _RimPower;
			uniform float _RimIntensity;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				_FlowTex_ST.x = 1 /_FlowTex_ST.x;
				_FlowTex_ST.y = 1 / _FlowTex_ST.y;
				o.uv2 = float2((worldPos.x + _FlowTex_ST.z) * _FlowTex_ST.x, (worldPos.z + _FlowTex_ST.w) * _FlowTex_ST.y);

				float4 col = tex2Dlod(_FlowTex, float4(o.uv2.x, o.uv2.y, 0, 0));
				float diff = _FlowIndex - col.w;
				if (diff > 0) {
					worldPos.y = worldPos.y + _OffsetY * diff;
				}

//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _LightTex);
				return o;

			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv0);
				fixed4 light = tex2D(_LightTex, i.uv1);
				light.rgb = DecodeLightmap(light);

				float4 flow = tex2Dlod(_FlowTex, float4(i.uv2.x, i.uv2.y, 0, 0));
				float diff = _FlowIndex - flow.w;
				//可以优化
				if (diff > 0) {
					light *= _Color;
				}

				col *= light * _factor;
				return col;
			}
			ENDCG
		}
	}
}
