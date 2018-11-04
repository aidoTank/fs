// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Flow2"{
	Properties{
		_BumpMap("BumpMap",2D) = "white"{}
		_BumpFactor("Bump Scale", Range(0, 10.0)) = 1.0
	}
		SubShader{
		pass {
		Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		float4 _LightColor0;
		sampler2D _BumpMap;

		float _BumpFactor;

		struct v2f {
			float4 pos:SV_POSITION;
			float2 uv:TEXCOORD0;
			float3 lightDir:TEXCOORD1;
		};

		v2f vert(appdata_full v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
			o.uv = v.texcoord.xy;
			TANGENT_SPACE_ROTATION;
			o.lightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;//Direction Light
			o.lightDir = mul(rotation,o.lightDir);
			return o;
		}

		float4 frag(v2f i) :COLOR
		{
			float4 c = 1;
			float3 N = UnpackNormal(tex2D(_BumpMap,i.uv));
			float3 normalFactor = float3(_BumpFactor, _BumpFactor, 1);
			N = normalize(N * normalFactor);
			float diff = max(0,dot(N,i.lightDir));
			c = _LightColor0*diff;
			return c * 2;
		}
			ENDCG
	}
	}
}
