Shader "Roma/UI/Font" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader {

		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Lighting Off 
		Cull Off 
		ZTest [unity_GUIZTestMode]
		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform fixed4 _Color;
			
			int _type;   

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
#ifdef UNITY_HALF_TEXEL_OFFSET
				o.vertex.xy += (_ScreenParams.zw-1.0)*fixed2(-1,1);
#endif
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = i.color;
				color.a *= tex2D(_MainTex, i.texcoord).a;
				clip (color.a - 0.01);
				if(_type == 1)   
                {   
                    color = color * 1.8f;   
                }   
                else if(_type == 2)   
                {   
                    color.rgb = color.rgb * 0.6f;   
                }   
                else if(_type == 3)   
                {   
                    fixed grey = dot(color.rgb, fixed3(0.22, 0.707, 0.071));      
                    color = fixed4(grey,grey,grey,color.a);     
                }
				return color;
			}
			ENDCG 
		}
	}
}
