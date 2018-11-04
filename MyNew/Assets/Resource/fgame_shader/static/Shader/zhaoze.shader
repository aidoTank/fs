Shader "uker/zhaoze"
{
	Properties
	{
		_Color("_Color", Color) = (1, 1, 1, 1)
		_MainTex ("_MainTex", 2D) = "white" {}
		_FlowTex("_FlowTex", 2D) = "white" {}
		_FlowIndex("FlowIndex", Range(0, 1.1)) = 0.5
		_OffsetY("OffsetY", float) = 6
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma multi_compile_fwdbase
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				//float2 uv : TEXCOORD0;
				float4 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				SHADOW_COORDS(4)
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			float _FlowIndex;
			float _OffsetY;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				_FlowTex_ST.x = 1 / _FlowTex_ST.x;
				_FlowTex_ST.y = 1 / _FlowTex_ST.y;
				o.uv.zw = (worldPos.xz + _FlowTex_ST.zw) * _FlowTex_ST.xy;

				float4 col = tex2Dlod(_FlowTex, float4(o.uv.zw, 0, 0));
				float diff = _FlowIndex - col.w;
				if (diff > 0) {
					worldPos.y = worldPos.y + _OffsetY * diff;
				}
				o.pos = mul(UNITY_MATRIX_VP, worldPos);
				o.worldPos = worldPos.xyz;

				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + diffuse * atten, 1.0);
			}
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM
			#pragma multi_compile_fwdadd

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				//float2 uv : TEXCOORD0;
				float4 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				SHADOW_COORDS(4)
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			float _FlowIndex;
			float _OffsetY;

			v2f vert(appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				_FlowTex_ST.x = 1 / _FlowTex_ST.x;
				_FlowTex_ST.y = 1 / _FlowTex_ST.y;
				o.uv.zw = (worldPos.xz + _FlowTex_ST.zw) * _FlowTex_ST.xy;

				float4 col = tex2Dlod(_FlowTex, float4(o.uv.zw, 0, 0));
				float diff = _FlowIndex - col.w;
				if (diff > 0) {
					worldPos.y = worldPos.y + _OffsetY * diff;
				}
				o.pos = mul(UNITY_MATRIX_VP, worldPos);
				o.worldPos = worldPos.xyz;

				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(diffuse * atten, 1.0);
			}
			ENDCG
		}
		
	}
}
