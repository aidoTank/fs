// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

Shader "Roma/Mobile LightMap(Static)" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		unity_Lightmap ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {
		tags {"RenderType" = "Opaque"}
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;	// lightmap使用模型的UV2
			};
            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;	// 存储计算后的lightmap坐标信息
            };

			// 在5.x之前需要声明这两个内置参数
			// sampler2D unity_Lightmap;
			// half4 unity_LightmapST;

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy;
                o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }
 
            fixed4 _Color;
            sampler2D _MainTex;
 
            fixed4 frag (v2f i) : COLOR {
                fixed4 final = fixed4(0,0,0,0);
				// 对主图采样
                fixed3 diffuse = tex2D(_MainTex, i.uv).rgb * _Color;
				// 对光照图采样，DecodeLightmap可根据不同平台进行解码
				// Unity烘焙的LightMap是32位的HDR图
				// 在PC端光照图编码为RGBM，在移动端编码为double-LDR
                fixed3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2) );
				// 叠加主色和光照图颜色
				fixed3 color = (diffuse * lightmap) ;
                final = fixed4 (color, 1);
                return final;
            }
            ENDCG
        }
    }
    FallBack Off
}