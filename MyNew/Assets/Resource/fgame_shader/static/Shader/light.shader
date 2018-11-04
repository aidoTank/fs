Shader "Unlit/light"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Light ("Light", 2D) = "white" {}
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
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Light;
			float4 _Light_ST;

			fixed3 DecodeLogLuv(in fixed4 vLogLuv)
			{
				fixed3x3 InverseM = fixed3x3(
					6.0014, -2.7008, -1.7996,
					-1.3320, 3.1029, -5.7721,
					0.3008, -1.0882, 5.6268);

				fixed Le = vLogLuv.z * 255 + vLogLuv.w;
				fixed3 Xp_Y_XYZp;
				Xp_Y_XYZp.y = exp2((Le - 127) / 2);
				Xp_Y_XYZp.z = Xp_Y_XYZp.y / vLogLuv.y;
				Xp_Y_XYZp.x = vLogLuv.x * Xp_Y_XYZp.z;
				fixed3 vRGB = mul(Xp_Y_XYZp, InverseM);
				return max(vRGB, 0);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = v.uv1.xy * _Light_ST.xy + _Light_ST.zw;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 light = tex2D(_Light, i.uv1); 
				fixed3 lm = DecodeLogLuv (light);
				col.rgb *= lm;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
