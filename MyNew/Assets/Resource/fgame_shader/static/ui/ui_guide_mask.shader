Shader "Roma/UI/ui_guide_mask"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

	_MaskTex("Mask", 2D) = "white" {}

		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		ColorMask[_ColorMask]
		Blend zero OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_ALPHACLIP

	struct appdata_t
	{
		fixed4 vertex : POSITION;
		fixed4 color : COLOR;
		fixed2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		fixed4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		fixed2 texcoord : TEXCOORD0;
		fixed4 worldPosition : TEXCOORD1;
		fixed2 mask_uv : TEXCOORD2;
	};

	int _type;
	fixed4 _Color;
	fixed4 _TextureSampleAdd;
	fixed4 _ClipRect;

	sampler2D _MainTex;
	sampler2D _MaskTex;
	float4 _MaskTex_ST;

	float uvWidth;
	float uvHeight;
	float uvX;
	float uvY;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.worldPosition = IN.vertex;
		OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

		OUT.texcoord = IN.texcoord;

#ifdef UNITY_HALF_TEXEL_OFFSET
		OUT.vertex.xy += (_ScreenParams.zw - 1.0)*fixed2(-1,1);
#endif

		OUT.color = IN.color * _Color;

		OUT.mask_uv = TRANSFORM_TEX(IN.texcoord, _MaskTex);
		return OUT;
	}

	

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

		color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

	#ifdef UNITY_UI_ALPHACLIP
		clip(color.a - 0.001);
	#endif
		
		float x = (IN.mask_uv.x - uvX) / uvWidth;
		float y = (IN.mask_uv.y - uvY) / uvHeight;

		color.a = 1 - color.a * tex2D(_MaskTex, fixed2(x, y)).a;
		color.a = color.a * IN.color.a;
		return color;
		}
			ENDCG
		}
	}
}
