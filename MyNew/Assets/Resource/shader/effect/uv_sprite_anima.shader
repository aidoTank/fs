Shader "Roma/Effect/序列帧动画" 
{
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SizeX ("列", Float) = 4  
		_SizeY ("行", Float) = 2
		_Speed ("播放速度", Float) = 150
	}
	SubShader {
		Tags    
		{    
			"Queue"="Transparent"    
			"IgnoreProjector"="True"   
			"RenderType"="Transparent"   
		}   
		Pass
		{
			Cull Off
			Lighting Off 
			Fog { Mode Off }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha   
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			uniform fixed _SizeX;
			uniform fixed _SizeY;
			uniform fixed _Speed;

			struct v2f {
				fixed4  pos : POSITION;
				fixed2  uv : TEXCOORD0;
				fixed4 vertColor : TEXCOORD1;
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				int index = floor(_Time.x * _Speed);
				int index_y = floor(_SizeY - (index + 1) / _SizeX);
				int index_x = index - index_y * _SizeX;
				o.uv = fixed2(v.texcoord.x /_SizeX + fmod(index_x / _SizeX, 1), v.texcoord.y /_SizeY + fmod(index_y / _SizeY, 1));
				o.vertColor = v.color;
				return o;
			}
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}


