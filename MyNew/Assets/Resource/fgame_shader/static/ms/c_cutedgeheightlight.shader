// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_cut_edge_heightlight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex("_SecondTex",2D) = ""{}
		_SecondOffx("_SecondOffx",Range(1,5)) = 2
		_Color ("Color", Color) = (0,0,0,0)
		_BottomLight("BottomLight", Range(0,2)) = .5
		//_rimEdge("RimEdge",Range(0,1)) = .5
		_textureAdd("TextureAdd",Range(1,6)) = 2
		//_BottomLightTime("BottomLightTime",Range(1,4)) = 2
	}

	SubShader
	{	
		Blend SrcAlpha One
		//Blend One One
		ZWrite Off
		ZTest On
		Cull Off

		Tags
		{
			"Queue"="Transparent+2" 
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 2.0
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float2 uv1 :TEXCOORD4;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SecondTex;
			float4 _SecondTex_ST;
			fixed _SecondOffx;
			half _BottomLight;
			fixed4 _Color;
			fixed _textureAdd;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1)/2;
				o.screenuv.y = o.screenuv.y;
				o.objectPos = v.vertex.xyz;		
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				o.uv1 = TRANSFORM_TEX(v.uv,_SecondTex) + frac(fixed2(-_SecondOffx,0)* _Time.x);
				return o;
			}
			
			

			float triWave(float t, float offset, float yOffset)
			{
				return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			}

			//Texture RGB（）
			fixed4 texColor(v2f i, float rim)
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv) * 2;
				mainTex.r *= triWave(_Time.x * 5, -i.objectPos.y * 2, -0.7) * _textureAdd;
				mainTex.g *= saturate(rim) * (sin(_Time.z + mainTex.b * 5) + 1) * _textureAdd;
				return mainTex.r * _Color + mainTex.g * _Color ;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//Bottom Light(低光)

				half rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 1000;//值越大，边缘越淡
				half northPole = 1-i.objectPos.y - (2 - _BottomLight);
				half glow = max(max(0, rim), northPole);
				fixed4 SecondCol = tex2D(_SecondTex, i.uv1) * _Color;
				fixed4 glowColor = fixed4(lerp(_Color.rgb, fixed3(1, 1, 1) * SecondCol * 12 , pow(glow, 2)), 1) ;
				
				fixed4 hexes = texColor(i, rim);
				fixed4 col = _Color * pow(_Color.a,2) + glowColor * glow + hexes;
				//
				return col;
			}
			ENDCG
		}
	}
}
