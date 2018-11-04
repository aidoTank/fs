Shader "Roma/UI/Photo"   
{   
    Properties   
    {   
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}   
        _Color ("Tint", Color) = (1,1,1,1)   
           
        _StencilComp ("Stencil Comparison", Float) = 8   
        _Stencil ("Stencil ID", Float) = 0   
        _StencilOp ("Stencil Operation", Float) = 0   
        _StencilWriteMask ("Stencil Write Mask", Float) = 255   
        _StencilReadMask ("Stencil Read Mask", Float) = 255   
   
        _ColorMask ("Color Mask", Float) = 15   

		//_PowX ("_PowX", Float) = 3
		//_PowY ("_PowY", Float) = 1
    }   
   
    SubShader   
    {   
        Tags   
        {    
            "Queue"="Transparent"    
            "IgnoreProjector"="True"    
            "RenderType"="Transparent"    
            "PreviewType"="Plane"   
            "CanUseSpriteAtlas"="True"   
        }   
           
        Stencil   
        {   
            Ref [_Stencil]   
            Comp [_StencilComp]   
            Pass [_StencilOp]    
            ReadMask [_StencilReadMask]   
            WriteMask [_StencilWriteMask]   
        }   
   
        Cull Off   
        Lighting Off   
        ZWrite Off   
        ZTest [unity_GUIZTestMode]   
        Fog { Mode Off }   
        Blend One Zero   
        ColorMask [_ColorMask]   
        Blend One OneMinusSrcAlpha // shader颜色rgba*源A + 点累积颜色*(1-源A值)   
   
        Pass   
        {   
        CGPROGRAM   
            #pragma vertex vert   
            #pragma fragment frag   
            #include "UnityCG.cginc"   
               
            struct appdata_t   
            {   
                float4 vertex   : POSITION;   
                float4 color    : COLOR;   
                float2 texcoord : TEXCOORD0;   
            };   
   
            struct v2f   
            {   
                float4 vertex   : SV_POSITION;   
                fixed4 color    : COLOR;   
                half2 texcoord  : TEXCOORD0;   
            };   
               
            fixed4 _Color;   
   
            v2f vert(appdata_t IN)   
            {   
                v2f OUT;   
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);   
                OUT.texcoord = IN.texcoord;   
#ifdef UNITY_HALF_TEXEL_OFFSET   
                OUT.vertex.xy -= (_ScreenParams.zw-1.0);   
#endif   
                OUT.color = IN.color * _Color;   
                return OUT;   
            }   
   
            sampler2D _MainTex;   
			fixed _PowX;
			fixed _PowY;

            fixed4 frag(v2f IN) : SV_Target   
            {   
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
				//fixed width = _MainTex_TexelSize.z;
				//fixed height = _MainTex_TexelSize.w;
				//float x = newUV.x * width;
				//float y = newUV.y * height;

				//clip (color.a - 0.01);   
				//fixed2 newUV = IN.texcoord * 2 - 1;
				//float x = 1 - abs(newUV.x);
				//float y = 1 - abs(newUV.y);
				//color.a = min(x, y);
				//color.a = pow(color.a * _PowX, _PowY);
                return color;   
            }   
        ENDCG   
        }   
    }   
}   
