Shader "Roma/Effect/UV移动"{
    Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1.0
        _ScrollY ("Base layer Scroll speed Y", Float) = 0.0
        _Intensity ("Intensity", Float) = 1.0
    }

	SubShader {
		Tags    
		{    
			"Queue"="Transparent"    
			"IgnoreProjector"="True"   
			"RenderType"="Transparent"
		}   
		Pass {
			Cull Off
			Lighting Off 
			Fog { Mode Off }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha   
			CGPROGRAM
        	#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			fixed _ScrollX;
			fixed _ScrollY;
			fixed _Intensity;

			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 color : TEXCOORD1;
				fixed4 vertColor : TEXCOORD2;
			};

			v2f vert (appdata_full v)
			{
			   v2f o;
			   o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			   o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(fixed2(_ScrollX, _ScrollY) * _Time);
			   o.color = fixed4(_Intensity, _Intensity, _Intensity, 1) * _Color;
			   o.vertColor = v.color;
			   return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D (_MainTex, i.uv) * i.color * i.vertColor;
				return tex;
			}
			ENDCG 
        }
    }
}