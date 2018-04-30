Shader "Roma/Effect/UV扭曲动画" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_BGTex ("background Texture (RG)", 2D) = "white" {}
		_NoiseTex ("Distort Texture (RG)", 2D) = "white" {}
		_MainTex ("Alpha (A)", 2D) = "white" {}
		_HeatTime  ("Heat Time", range (-1,1)) = 0
		_ForceX  ("Strength X", range (0,1)) = 0.1
		_ForceY  ("Strength Y", range (0,1)) = 0.1
	}

	Category {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha   
		Cull Back Lighting Off ZWrite Off
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader {
			Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord: TEXCOORD0;
			};

			struct v2f {
				fixed4 vertex : POSITION;
				fixed2 uvmain : TEXCOORD0;
			};

			fixed4 _TintColor;
			fixed _ForceX;
			fixed _ForceY;
			fixed _HeatTime;
			fixed4 _MainTex_ST;
			fixed4 _NoiseTex_ST;
			fixed4 _BGTex_ST;
			sampler2D _NoiseTex;
			sampler2D _MainTex;
			sampler2D _BGTex;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 bgCol = tex2D(_BGTex, i.uvmain);
				fixed4 offsetColor1 = tex2D(_NoiseTex, i.uvmain + _Time.xz*_HeatTime);
				fixed4 offsetColor2 = tex2D(_NoiseTex, i.uvmain + _Time.yx*_HeatTime);
				i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceX;
				i.uvmain.y += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceY;
				return _TintColor * tex2D( _MainTex, i.uvmain) * bgCol;
			}
			ENDCG
			}
		}
		SubShader {
			Blend DstColor Zero
			Pass {
				Name "BASE"
				SetTexture [_MainTex] {	combine texture }
			}
		}
	}
}
