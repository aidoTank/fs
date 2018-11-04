Shader "FGame/static/ms/role" {
	Properties {
		_TintColor ("Color", Color) = (1,1,1,1)
		_SpeColor("SpeColor",Color)=(1,1,1,1)
		_DownColor("DownColor",Color)=(1,1,1,1)	
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MatCap("MatCap",2D) = "white" {}
		_MaskMatCap("MaskMatCap",2D)= "white" {}
		_Brightness("Brightness",Float) = 1
		_MatCapBrightness("MatCapBrightness",Float) = 1
		_AreaUpDown("AreaUpDown",Float)= 0
		_AreaBrightness("AreaBrightness",Float) = 1
		[Toggle]_CanMaskGloss("CanMaskGloss",Float) = 0
		_MaskGloss("MaskGloss",Range(0.1,30)) = 2
		_MaskBrightness("MaskBrightness",Float) = 1
		_Gloss("Gloss",Float) = 8.7
		_GlossBrightness("GlossBrightness",Float)= 0.21
		_GlossScale("GlossScale",Float) = 0.38
		_FresnelScale("FresnelScale",Float) = -2.94
		_LightDir("LightDir",Vector)=(0.88,-0.83,0.18,0)
		_LightDir2("LightDir2",Vector)=(0.88,-0.83,0.18,0)		
		_FluxayTex("FluxayTex",2D) = "white"{}
		_MaskFluxayTex("MaskFluxayTex",2D) = "white"{}
		_FluxayColor("FluxayColor",Color)=(1,1,1,1)
		_FluxayColor2("FluxayColor2",Color)=(1,1,1,1)
		_FluxayBrightness("FluxayBrightness",Float) = 0.74
		_FluxayColorLerpSpeed("FluxayColorLerpSpeed",Float) = 0.2
		[Toggle]_FluxayCanLerp("FluxayCanLerp",Float) = 0
		_UvFlowX("UvFlowX",Float) = 0.2
		_UvFlowY("UvFlowY",Float) = 0.5
		_ShadowColor("ShadowColor",Color)=(0.13,0.13,0.13,1)
		_ShadowPos("ShadowPos",Vector)=(0.01,0,-0.09,0.02)
	}
	SubShader {
		
		LOD 200
		Tags { "RenderType"="Opaque"  }
		Pass {		
			//此PASS处理主色调
			Tags { "LightMode"="ForwardBase" }
			Cull Off
			CGPROGRAM
			#pragma multi_compile_fwdbase	
			#pragma vertex vert 
			#pragma fragment frag 
			#include "UnityCG.cginc"
		
			fixed4 _TintColor;
			fixed4 _SpeColor;
			fixed4 _DownColor;
			half _AreaUpDown;
			half _AreaBrightness;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MatCap;
			sampler2D _MaskMatCap;
			half _MatCapBrightness;
			half _Gloss;
			half _GlossScale;
			half _Brightness;
			half _GlossBrightness;
			half _FresnelScale;
			float4 _LightDir;
			float4 _LightDir2;
			sampler2D _FluxayTex;
			float4 _FluxayTex_ST;
			sampler2D _MaskFluxayTex;
			half _FluxayBrightness;
			fixed4 _FluxayColor;
			half _UvFlowX;
			half _UvFlowY;
			fixed4 _FluxayColor2;
			half _FluxayColorLerpSpeed;
			fixed _FluxayCanLerp;
			fixed _CanMaskGloss;
			half _MaskGloss;
			half _MaskBrightness;

			struct a2v{
				float4 vertex:POSITION;	
				float3 normal:NORMAL;			
				float2 texcoord:TEXCOORD0;	
			};

			struct v2f{
				float4 pos:SV_POSITION;
				float4 uv:TEXCOORD0;	
				float3 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;	
				float2 matNormal:TEXCOORD3;
			};


			v2f vert(a2v v){
				v2f f;
				f.pos=UnityObjectToClipPos(v.vertex);
				f.uv.xy = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;		
				f.uv.zw = v.texcoord * _FluxayTex_ST.xy + _FluxayTex_ST.zw;	
				f.uv.z += _UvFlowX * _Time.y;
				f.uv.w += _UvFlowY * _Time.y;
				f.worldPos = mul(unity_ObjectToWorld,v.vertex);
				f.worldNormal = UnityObjectToWorldNormal(v.normal);

				float3 matWorldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
				matWorldNorm = mul((float3x3)UNITY_MATRIX_V, matWorldNorm);
				f.matNormal = matWorldNorm.xy * 0.5 + 0.5;
				return f;
			}

			fixed4 frag(v2f f):SV_Target{	
				fixed3 worldNormal = normalize(f.worldNormal);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(f.worldPos));
				fixed3 worldLightDir = normalize(_LightDir.xyz);
				fixed3 worldLightDir2 = normalize(_LightDir2.xyz);				
				fixed3 worldHlafDir = normalize(worldViewDir + worldLightDir);
				fixed3 worldHlafDir2 = normalize(worldViewDir + worldLightDir2);
			    //UV流光流动		
				fixed TexFluxay = tex2D(_FluxayTex,f.uv.zw).r;
				fixed MaskFluxay = tex2D(_MaskFluxayTex,f.uv.xy).r;
				fixed TrueFluxay = TexFluxay * MaskFluxay;
				fixed3 TrueFluxayColor = TrueFluxay * _FluxayBrightness * lerp(_FluxayColor.rgb,_FluxayColor2.rgb,_FluxayCanLerp*sin(_FluxayColorLerpSpeed*_Time.y)) ;
				//默认颜色
				fixed3 mainColor = tex2D(_MainTex,f.uv.xy).rgb * _TintColor.rgb;
				//MatCap
				fixed mcMask = tex2D(_MaskMatCap,f.uv.xy).r;
				fixed3 mc = tex2D(_MatCap, f.matNormal).rgb * mainColor * _MatCapBrightness;
				mainColor = lerp(mainColor,mc,mcMask);
				//遮罩高光
				fixed n2l = dot(worldNormal,worldLightDir2);
				fixed maskSpe = _CanMaskGloss*mcMask;
				fixed3 tureMaskSpe =_MaskBrightness* maskSpe*( _SpeColor.rgb * pow(max(0,n2l),_MaskGloss));
				//高光
				fixed n2h = dot(worldNormal,worldHlafDir2);										
				fixed3 spe =_SpeColor.rgb * pow(max(0,n2h),_Gloss);
				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * (_GlossScale - dot(worldHlafDir, worldNormal));					
				fixed3 tureSpe =(1-maskSpe) *(lerp(0, _SpeColor.rgb,(saturate(fresnel)))+ spe * _SpeColor.rgb * _GlossBrightness);
				//从上往下渐变颜色
				fixed3 UpDownColor =_AreaBrightness * mainColor * lerp(_DownColor,fixed3(0,0,0),saturate(f.worldPos.y+_AreaUpDown));
				//最终颜色
				fixed3 finColor = mainColor * _Brightness - UpDownColor + tureSpe +tureMaskSpe+ TrueFluxayColor;
				return fixed4(finColor,1);		
			}
			ENDCG
		}	

		UsePass "FGame/static/ms/role_shadow/SHADOW"
							
		//Pass {
		//	//处理点光源PASS
		//	Tags { "LightMode"="ForwardAdd" }			
		//	Blend One One
		
		//	CGPROGRAM

		//	#pragma multi_compile_fwdadd			
		//	#pragma vertex vert
		//	#pragma fragment frag			
		//	#include "Lighting.cginc"
		//	#include "AutoLight.cginc"
			
		//	fixed4 _TintColor;
		//	fixed4 _SpeColor;
		//	float _Gloss;
			
		//	struct a2v {
		//		float4 vertex : POSITION;
		//		float3 normal : NORMAL;
		//	};
			
		//	struct v2f {
		//		float4 pos : SV_POSITION;
		//		float3 worldNormal : TEXCOORD0;
		//		float3 worldPos : TEXCOORD1;
		//	};
			
		//	v2f vert(a2v v) {
		//		v2f o;
		//		o.pos = UnityObjectToClipPos(v.vertex);
				
		//		o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
		//		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
		//		return o;
		//	}
			
		//	fixed4 frag(v2f i) : SV_Target {
		//		fixed3 worldNormal = normalize(i.worldNormal);
		//		#ifdef USING_DIRECTIONAL_LIGHT
		//			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		//		#else
		//			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
		//		#endif
				
		//		fixed3 diffuse = _LightColor0.rgb * _TintColor.rgb * max(0, dot(worldNormal, worldLightDir));
				
		//		fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
		//		fixed3 halfDir = normalize(worldLightDir + viewDir);
		//		fixed3 specular = _LightColor0.rgb * _SpeColor.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
				
		//		#ifdef USING_DIRECTIONAL_LIGHT
		//			fixed atten = 1.0;
		//		#else
		//			#if defined (POINT)
		//		        float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
		//		        fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
		//		    #else
		//		        fixed atten = 1.0;
		//		    #endif
		//		#endif

		//		return fixed4((diffuse + specular) * atten, 1.0);
		//	}
			
		//	ENDCG
		//}
	
	}	
	
	SubShader {
		LOD 100
		Tags { "RenderType"="Opaque"  }
		UsePass "FGame/static/ms/diffuse/DIFFUSE"
		UsePass "FGame/static/ms/role_shadow/SHADOW"
	}
	// FallBack "Unlit/Texture"
}
