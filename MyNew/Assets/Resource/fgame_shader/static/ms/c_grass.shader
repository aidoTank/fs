// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_grass" {
	Properties {		
		_MainTex ("MainTex", 2D) = "white" {}
		_TintColor("MainColor",Color) = (1,1,1,1) 
        //_Speed("Speed",Float) = 1  
        _Brightness("Intensity",Float) = 1  
		_Range("Range",Float) = 1
		_Offset("Offset",Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull back
		Offset -1,-1
		Fog {Mode Off}  

		Pass {		
							
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "Lighting.cginc"
					
			sampler2D _MainTex;
			float4 _MainTex_ST;	
			float4 _TintColor; 			
			//half _Speed;  
			half _Brightness;  
			half _Range;
			half _Offset;
			
			struct a2v {
				float4 vertex : POSITION;				
				float4 texcoord : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float4 uv : TEXCOORD0;	
				float4 worldPos : TEXCOORD1;	
			};
			
			v2f vert(a2v v) {
				v2f o;			
				UNITY_INITIALIZE_OUTPUT(v2f, o);	
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
				//通过世界坐标来计算偏移	 
				//float offsetXZ = sin((worldPos.x + worldPos.z) * _Offset - _Time.y )*cos(worldPos.y * _Brightness - _Time.y * _Speed);
				float offsetXZ = sin(worldPos.x * _Offset  - _Time.y );
			    //worldPos.xz = worldPos.xz +  offsetXZ * saturate(worldPos.y * _Brightness)*_Range;
				worldPos.xz = worldPos.xz +  offsetXZ * worldPos.y *_Range;
				o.pos = mul(UNITY_MATRIX_VP,worldPos);
				// o.pos = UnityObjectToClipPos(v.vertex);	
				// o.normal = UnityObjectToWorldDir(normalize(v.normal));
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {	

				fixed4 texColor = tex2D(_MainTex, i.uv.xy) *_TintColor;
				fixed4 worldPos = texColor * i.worldPos;
	
				//最终颜色
				// fixed3 finColor = texColor.rgb * _TintColor.rgb;
				//fixed4 finColor = texColor* _TintColor;
				// clip(texColor.a-0.5);
				// return fixed4(finColor,texColor.a);
				return texColor * _Brightness ;
			}			
			ENDCG
		}
		
	} 

	SubShader {
		LOD 100
		Tags { "RenderType"="Opaque"  }
		UsePass "FGame/static/ms/diffuse/DIFFUSE"
	}
	FallBack "Unlit/Texture"
	
}
