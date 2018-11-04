// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_water" {
	Properties {
		_Color ("Main Color", Color) = (0, 0.15, 0.115, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EdgeTex ("EdgeTex", 2D) = "white" {}
		_EdgeNoiseTex ("EdgeNoiseTex", 2D) = "white" {}
		_WaveMap ("Wave Map", 2D) = "bump" {}
		_Cubemap ("Environment Cubemap", Cube) = "_Skybox" {}
		_Brightness("Brightness",Float) = 1
		_WaveXSpeed ("Wave Horizontal Speed", Range(-0.1, 0.1)) = 0.01
		_WaveYSpeed ("Wave Vertical Speed", Range(-0.1, 0.1)) = 0.01
		_Distortion ("Distortion", Range(0, 100)) = 10
		_Gloss("Gloss", Range(0, 10)) = 3
		_Opacity("Opacity", Range(-2,2)) = 0.8
		_EdgeSpeed("EdgeSpeed",Float) = 1
		_EdgeBrightness("EdgeBrightness",Float) = 1
		_EdgeNoiseScale("EdgeNoiseScale",Float) = 1
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags {"Queue"="Transparent-2"  "RenderType"="Transparent"}
		Cull BACK
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
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			sampler2D _WaveMap;
			float4 _WaveMap_ST;
			samplerCUBE _Cubemap;
			fixed _WaveXSpeed;
			fixed _WaveYSpeed;
			float _Distortion;	
			half _Brightness;
			fixed _Opacity;
			sampler2D _EdgeTex;
			half _EdgeSpeed;
			sampler2D _EdgeNoiseTex;
			float4 _EdgeNoiseTex_ST;
			half _EdgeBrightness;
			half _EdgeNoiseScale;
			half _Gloss;

			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT; 
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 vertexColor : COLOR0;
				float4 uv : TEXCOORD0;
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
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _WaveMap);
				o.uv2.xy = TRANSFORM_TEX(v.texcoord, _EdgeNoiseTex);

				o.vertexColor = v.color;
				//切线空间到世界空间矩阵
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);  
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w; 
				
				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);  
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);  
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);  
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				float2 speed = _Time.y * float2(_WaveXSpeed, _WaveYSpeed);
				//扭曲法线制作波纹效果和地表扭曲效果
				fixed3 bump1 = UnpackNormal(tex2D(_WaveMap, i.uv.zw + speed)).rgb;
				fixed3 bump2 = UnpackNormal(tex2D(_WaveMap, i.uv.zw - speed)).rgb;
				fixed3 bump = normalize(bump1 + bump2);
				float2 offset = bump.xy * _Distortion * _MainTex_TexelSize.xy;
				fixed4 refrCol = tex2D(_MainTex, i.uv.xy + offset)*_Brightness * _Color;	
				//用模型顶点色来制作边缘波浪效果
				fixed edgeNoise = tex2D(_EdgeNoiseTex, i.uv2.xy).r;
				fixed edgeColor = _EdgeBrightness * lerp(tex2D(_EdgeTex,edgeNoise * _EdgeNoiseScale *  frac( (half2(_Time.y * _EdgeSpeed + i.vertexColor.x,0)))).r,0,i.vertexColor.x);
				//采样cubemap和菲涅尔反射
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));				
				fixed3 reflDir = reflect(-viewDir, bump);
				fixed3 reflCol = texCUBE(_Cubemap, reflDir).rgb;	
				fixed fresnel = pow(1 - saturate(dot(viewDir, bump)), _Gloss);
				//顶点色来制作边缘透明效果
				fixed finalAlpha = i.vertexColor.x * _Opacity;
				fixed3 finalColor =edgeColor+refrCol+ reflCol * fresnel + refrCol * (1 - fresnel);
				
				return fixed4(finalColor, finalAlpha);
			}
			
			ENDCG
		}
	}
	FallBack Off
}
