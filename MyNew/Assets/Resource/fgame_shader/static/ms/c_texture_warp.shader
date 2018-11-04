Shader "FGame/static/ms/c_texture_warp"
{
	Properties
	{
		_Color("Color",Color) = (.5,.5,.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		_SeconedTex("SeconedTex",2D) = ""{}
		_ThirdTex("ThirdTex",2D) = ""{}
		_DarkTex("DarkTex",2D) = ""{}
		//_Speed("Speed",Range(0,6)) = 1
		_ScrollX("ScrollX",Range(-1,1)) =.1
		_ScrollY ("ScrollY",Range(-1,1)) =.1
		_RotSpeed("RotSpeed",Range(-1,1)) = .5
		_Intansity("Intansity",Range(1,6)) =1
		//_DarkTexBrightness("DarkTexBrightness",Range(-1,1)) = 0

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		//Blend  SrcColor Zero
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SeconedTex;
			float4 _SeconedTex_ST;
			sampler2D _ThirdTex;
			float4 _ThirdTex_ST;
			sampler2D _DarkTex;
			float4 _DarkTex_ST;
			float4 _NoiseColor;
			half _ColorBrightness;

			fixed4 _Color;
			half _ScrollX;
			half _ScrollY;
			//half _Speed;
			half _RotSpeed;
			half _Intansity;
			half _DarkTexBrightness;


			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(fixed2(_ScrollX,_ScrollY) * _Time);
				o.uv1 = TRANSFORM_TEX(v.uv,_SeconedTex);
				//o.uv2 = TRANSFORM_TEX(v.uv,_ThirdTex);
				half2 uv = v.uv.xy - half2(0.5,0.5);
                uv = half2(uv.x*cos(_RotSpeed*_Time.y)-uv.y*sin(_RotSpeed*_Time.y),uv.y*cos(_RotSpeed*_Time.y) + uv.x*sin(_RotSpeed*_Time.y));
                uv += half2(0.5,0.5);
                o.uv2 = TRANSFORM_TEX(uv,_ThirdTex);
				o.uv3 = TRANSFORM_TEX(uv,_DarkTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
				fixed4 NosieCol = tex2D(_MainTex,i.uv2);
				fixed4 DarkCol = tex2D(_DarkTex,i.uv3);
				// fixed4 col = fixed4(1,1,1,1);
				// fixed4 DarkColCtrl = lerp(DarkCol, col, _DarkTexBrightness);
				//fixed NoiseColor = tex2D(_NoiseTex,f.uv.zw).r;		
				//fixed4 NoiseBrightness = DarkCol * _ColorBrightness * NoiseColor;
				//fixed4 NoiseDark =(1-_ColorDark) *(1-NoiseColor) *_FlowDark;

				//fixed Nosie = tex2D(_MainTex,i.uv).a;
				fixed2 ChanelCol = fixed2(i.uv.x - NosieCol.g, i.uv.y - NosieCol.r - _Time.x);
				fixed2 ChanelColT = fixed2(i.uv2.x - NosieCol.b, i.uv2.y - NosieCol.r - _Time.y);
				fixed4 Result = tex2D(_SeconedTex, ChanelCol);
				fixed4 ThirdTexCol = tex2D(_ThirdTex, ChanelColT/20) * _Intansity;
				fixed4 FinallyCol =  Result * _Color * ThirdTexCol * DarkCol;
				return FinallyCol;
			}
			ENDCG
		}
	}
}
