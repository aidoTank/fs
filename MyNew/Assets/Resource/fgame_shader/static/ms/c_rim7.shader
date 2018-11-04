// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "FGame/static/ms/rim6"
{
	//属性
	Properties{
		_TintColor("Diffuse", Color) = (1,1,1,1)
		_MainTex("Base 2D", 2D) = "white"{}
		
		_LightDir("_LightDir",Vector) = (0,1,0,1)
		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.000001, 3.0)) = 0.1
		_RimIntensity("_RimIntensity", Range(0.0, 1)) = 0.5

		// _OutlineCol("_OutlineCol", Color) = (1,1,1,1)
		// _OutlineFactor("_OutlineFactor",Range(0,0.1)) = 0.05
		
	}

	//子着色器	
	SubShader
	{ 
		Tags{ "Queue" = "Transparent-5" "RenderType" = "Opaque" "Tag" = "Opaque"}

		// Pass  
        // {  
		// 	Cull Front
		// 	ZWrite Off
		// 	Blend SrcAlpha  OneMinusSrcAlpha

        //     CGPROGRAM  
        //     #include "UnityCG.cginc"  
        //     fixed4 _OutlineCol;  
        //     float _OutlineFactor;  
              
        //     struct v2f  
        //     {  
        //         float4 pos : SV_POSITION;  
        //     };  
              
        //     v2f vert(appdata_full v)  
        //     {  
        //         v2f o;  
        //         o.pos = UnityObjectToClipPos(v.vertex);  
        //         float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);  
        //         float2 offset = TransformViewToProjection(vnormal.xy);
        //         o.pos.xy += offset * _OutlineFactor;  
        //         return o;  
        //     }  
              
        //     fixed4 frag(v2f i) : SV_Target  
        //     {  
        //         return _OutlineCol;  
        //     }  
              
        //     #pragma vertex vert  
        //     #pragma fragment frag  
        //     ENDCG  
        // }  

		Pass
		{
			LOD 200
			Cull back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
			#include "UnityCG.cginc"
			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _LightDir;
			fixed4 _RimColor;
			float _RimPower;
			uniform float _RimIntensity;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float3 worldViewDir : TEXCOORD2;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed4 color = tex2D(_MainTex, i.uv);
				
				
				float3 worldViewDir = normalize(i.worldViewDir);
				float rim = 1 - max(0, dot(worldViewDir, worldNormal));
				//rim *= 1 - max(0, dot(worldViewDir, worldNormal));
				fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower) *_RimIntensity;

				fixed3 worldLightDir = normalize(_LightDir.xyz);
				float lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
				rimColor *= lambert;

				color.rgb = color.rgb* _TintColor.rgb + rimColor;
				
				return color;
			}

			//使用vert函数和frag函数
			

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
