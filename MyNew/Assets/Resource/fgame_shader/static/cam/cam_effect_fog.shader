Shader "FGame/cam_effect_fog" {  
    Properties {  
        _MainTex ("Base (RGB)", 2D) = "" {}  
        _Value("Value",Range(0,1)) = 0
    }  
    Subshader {  
     Pass {  
            ZTest Always Cull Off ZWrite Off  
            Fog { Mode off }  
            Blend SrcAlpha OneMinusSrcAlpha   
            CGPROGRAM  

            #pragma vertex vert  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest 
            #include "UnityCG.cginc"  

            struct v2f {  
                float4 pos : POSITION;  
                float2 uv : TEXCOORD0;  
				float2 uv2 : TEXCOORD1;  
            };  

            sampler2D _MainTex;
            sampler2D _MaskTex;
            half _Scale;   // 屏幕比例宽高
			half4 _UvPos; // 当前世界坐标位置
			half _Value;

            v2f vert (appdata_img v) {  
                v2f o;  
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);  
                o.uv = v.texcoord;

				float2 newUv = o.uv * 2 - 1;
				// 只针对宽缩放，保持圆形
				newUv.x *= _Scale;
				// 偏移计算
				newUv.x -= _UvPos.x * _Scale;
				newUv.y -= _UvPos.y;
				// 转到01
				newUv = (newUv + 1) * 0.5f;
				o.uv2 = newUv;
                return o;    
            }  

            fixed4 frag (v2f i) : COLOR0 {  
                fixed4 color1 = tex2D (_MainTex, i.uv); 
				fixed4 mask = tex2D (_MaskTex, i.uv2);
				fixed4 h = color1 * mask;
                fixed4 col = lerp(h, color1, _Value);
                col.a = 1;
                return col;
          }   
          ENDCG  
       }  
	}
}