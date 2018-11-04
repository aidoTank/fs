Shader "FGame/Particles/partical_screen_mask_add"
{
	Properties
	{
		_Color("Color",Color) = (.5,.5,.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffSet]_MaskTex("ViewMaskTex",2D) = "" {}
		_offX("ScreenOffSet X",float) = 0
		_offY("ScreenOffSet Y",float) = 0 
		_Scale("ScreenScale",float) = 0 
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		Blend SrcAlpha One
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD1;
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			fixed4 _Color;
			half _offX;
			half _offY;
			half _Scale;
			half _ScaleY;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				o.color = v.color;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.scrPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 ViewPos = tex2D(_MaskTex,half2(i.scrPos.x + _offX, i.scrPos.x + _offY));
				
				i.scrPos.x += _offX;
				i.scrPos.y += _offY;
				i.scrPos *= _Scale;
				fixed4 ViewPos = tex2D(_MaskTex, i.scrPos);
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				return col * _Color * ViewPos ;
			}
			ENDCG
		}
	}
}
