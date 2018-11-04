Shader "Unlit/Flow5"
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
				//float2 uv2 : TEXCOORD2;
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
				o.uv1 = TRANSFORM_TEX(v.uv1, _LightTex);
				float4 col = tex2Dlod(_FlowTex, float4(o.uv0.x, o.uv0.y, 0, 0));
				if (col.w < _FlowIndex) {
					o.vertex .y = o.vertex.y + _OffsetY;
				}
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv0);
				fixed4 light = tex2D(_LightTex, i.uv1);
				col = col + light;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
