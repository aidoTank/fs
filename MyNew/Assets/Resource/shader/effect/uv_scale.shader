Shader "Roma/Effect/UV缩放"{
    Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_Intensity ("Intensity", Float) = 1.0
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
		_A ("振幅(最大和最小的幅度)", Range(0, 10)) = 5     
        _W ("角速度(缩放速度)", Range(0, 10)) = 5
		_K ("偏距(整体缩放大小)", Range(0, 10)) = 5
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
			float _Intensity;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed _A;
			fixed _W;
			fixed _K;
			fixed _Speed;

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
				o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
				o.uv = o.uv - 0.5f;
				o.uv = o.uv * (_A * sin(_W *_Time.y) + _K);
				o.uv = o.uv + 0.5f;
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