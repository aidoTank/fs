// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Flow3"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightTex("Light", 2D) = "black" {}
		_FlowTex("Flow", 2D) = "white" {}
		_FlowX("FlowX", Float) = 256
		_FlowZ("FlowZ", Float) = 256
		_FlowIndex("FlowIndex", Range(0, 1.1)) = 0.5
		_OffsetY("OffsetY", float) = 6
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
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _LightTex;
			float4 _LightTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			float _FlowX;
			float _FlowZ;
			float _FlowIndex;
			float _OffsetY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);

				o.uv1 = TRANSFORM_TEX(v.uv1, _FlowTex);

				//UNITY_TRANSFER_FOG(o,o.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

				_FlowTex_ST.x = 1 /_FlowTex_ST.x;
				_FlowTex_ST.y = 1 / _FlowTex_ST.y;
				o.uv1 = float2((worldPos.x + _FlowTex_ST.z) * _FlowTex_ST.x, (worldPos.z + _FlowTex_ST.w) * _FlowTex_ST.y);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv0);
				fixed4 light = tex2D(_FlowTex, i.uv1);
				light.x = light.w;
				col = col + light;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
