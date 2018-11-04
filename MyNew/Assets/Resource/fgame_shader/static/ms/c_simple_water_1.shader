// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_simple_water_1" {
	Properties {
		_TintColor ("Main Color", Color) = (0, 0.15, 0.115, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[NoScaleOffset] _EdgeTex ("EdgeTex", 2D) = "white" {}
		// _EdgeNoiseTex ("EdgeNoiseTex", 2D) = "white" {}
		_Brightness("Brightness",Float) = 1
		_Opacity("Opacity", Range(-2,2)) = 0.8
		_EdgeSpeed("EdgeSpeed",Float) = 1
		_EdgeBrightness("EdgeBrightness",Float) = 1
		_EdgeNoiseScale("EdgeNoiseScale",Float) = 1
	}
	// SubShader {
	// 	Blend SrcAlpha OneMinusSrcAlpha
	// 	Tags {"Queue"="Transparent-2"  "RenderType"="Transparent"}
	// 	LOD 200
	// 	Cull BACK
		
	// 	Stencil
	// 	{
	// 		Ref 1
	// 		Comp NotEqual
	// 		Pass IncrSat
	// 		Fail keep
	// 		ZFail keep
	// 	}	
	// 	Pass {			
	// 		CGPROGRAM
			
	// 		#include "UnityCG.cginc"				
	// 		#pragma vertex vert
	// 		#pragma fragment frag
			
	// 		fixed4 _TintColor;
	// 		sampler2D _MainTex;
	// 		float4 _MainTex_ST;
	// 		sampler2D _EdgeNoiseTex;
	// 		float4 _EdgeNoiseTex_ST;
	// 		half _Brightness;
	// 		fixed _Opacity;
	// 		sampler2D _EdgeTex;
	// 		half _EdgeSpeed;
	// 		half _EdgeBrightness;
	// 		half _EdgeNoiseScale;

			
	// 		struct a2v {
	// 			float4 vertex : POSITION;
	// 			float3 normal : NORMAL;
	// 			float4 tangent : TANGENT; 
	// 			float2 texcoord : TEXCOORD0;
	// 			float4 color : COLOR0;
	// 		};
			
	// 		struct v2f {
	// 			float4 pos : SV_POSITION;
	// 			float4 vertexColor : COLOR0;
	// 			float2 uv : TEXCOORD0;
	// 			float4 TtoW0 : TEXCOORD1;  
	// 			float4 TtoW1 : TEXCOORD2;  
	// 			float4 TtoW2 : TEXCOORD3; 
	// 			float2 uv2 : TEXCOORD4;
	// 		};
			
	// 		v2f vert(a2v v) {
	// 			v2f o;
	// 			UNITY_INITIALIZE_OUTPUT(v2f, o);	
	// 			o.pos = UnityObjectToClipPos(v.vertex);
				
	// 			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	// 			// o.uv.zw = TRANSFORM_TEX(v.texcoord, _WaveMap);
	// 			o.uv2.xy = TRANSFORM_TEX(v.texcoord, _EdgeNoiseTex);

	// 			o.vertexColor = v.color;
	// 			//切线空间到世界空间矩阵
	// 			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
	// 			fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);  
	// 			fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
	// 			fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w; 
				
	// 			o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);  
	// 			o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);  
	// 			o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);  
				
	// 			return o;
	// 		}
			
	// 		fixed4 frag(v2f i) : SV_Target {
	// 			float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
	// 			fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	// 			fixed4 refrCol = tex2D(_MainTex, i.uv.xy)*_Brightness * _TintColor;	
	// 			fixed edgeNoise = tex2D(_EdgeNoiseTex, i.uv2.xy).r;
	// 			fixed edgeColor = _EdgeBrightness * lerp(tex2D(_EdgeTex,edgeNoise * _EdgeNoiseScale *  frac( (half2(_Time.y * _EdgeSpeed + i.vertexColor.x,0)))).r,0,i.vertexColor.x);
	// 			fixed finalAlpha = i.vertexColor.x * _Opacity;
	// 			fixed3 finalColor =edgeColor+refrCol;
				
	// 			return fixed4(finalColor, finalAlpha);
	// 		}
			
	// 		ENDCG
	// 	}
	// }

	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags {"Queue"="Transparent-2"  "RenderType"="Transparent"}
		Cull BACK
		LOD 200

		Stencil
		{
			Ref 1
			Comp NotEqual
			Pass IncrSat
			Fail keep
			ZFail keep
		}	
		Pass {			
			CGPROGRAM
			
			#include "UnityCG.cginc"				
			#pragma vertex vert
			#pragma fragment frag
			
			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			// sampler2D _EdgeNoiseTex;
			// float4 _EdgeNoiseTex_ST;
			half _Brightness;
			fixed _Opacity;
			sampler2D _EdgeTex;
			half _EdgeSpeed;
			half _EdgeBrightness;
			half _EdgeNoiseScale;

			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 vertexColor : COLOR0;
				float2 uv : TEXCOORD0;
				float4 TtoW0 : TEXCOORD1;  
				float4 TtoW1 : TEXCOORD2;  
				float4 TtoW2 : TEXCOORD3; 
				float2 uv2 : TEXCOORD4;
			};
			
			v2f vert(a2v v) {
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);	
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				// o.uv.zw = TRANSFORM_TEX(v.texcoord, _WaveMap);
				//o.uv2.xy = TRANSFORM_TEX(v.texcoord, _EdgeNoiseTex);

				o.vertexColor = v.color;
				//切线空间到世界空间矩阵
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);  
			
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				// float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				// fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed4 refrCol = tex2D(_MainTex, i.uv.xy)*_Brightness * _TintColor;	
				//fixed edgeNoise = tex2D(_EdgeNoiseTex, i.uv2.xy).r;
				fixed edgeNoise = refrCol.r;
				fixed edgeColor = _EdgeBrightness * lerp(tex2D(_EdgeTex,edgeNoise * _EdgeNoiseScale *  frac( (half2(_Time.y * _EdgeSpeed + i.vertexColor.x,0)))).r,0,i.vertexColor.x);
				fixed finalAlpha = i.vertexColor.x * _Opacity;
				fixed3 finalColor =edgeColor+refrCol;
				
				return fixed4(finalColor, finalAlpha);
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
