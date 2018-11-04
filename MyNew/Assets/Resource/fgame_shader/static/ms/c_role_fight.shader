Shader "FGame/static/ms/role_fight" {
	Properties {
		_TintColor ("Color", Color) = (1,1,1,1)	
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Brightness("Brightness",Float) = 1
		_ShadowColor("ShadowColor",Color)=(0.1,0.1,0.1,0.435)
		_ShadowPos("ShadowPos",Vector)=(-0.26,-0.05,0.2,0.02)
		_SpeColor("SpeColor",Color)=(0,0,0,0)	
		_Gloss("Gloss",Float) = 0.38
		_FresnelScale("FresnelScale",Float) = -2.94
		_LightDir("LightDir",Vector)=(0.88,-0.83,0.18,0)
		_SizeX("SizeX",int) = 1
		_SizeY("SizeY",int) = 1
		_ChooseX("ChooseX",int) = 1
		_ChooseY("ChooseY",int) = 1
		//_OutLine("OutLine",Float) = 2
		//_OutLine2("OutLine2",Float) = 0
		//_OutLine2("OutLine3",Float) = 0
		//_OutLineColor ("OutLineColor", Color) = (0.06,0.06,0.06,1)
		//_OutLineColor2 ("OutLineColor2", Color) = (0.06,0.06,0.06,1)	
		//_OutLineColor3 ("OutLineColor3", Color) = (0.06,0.06,0.06,1)	
	}
	SubShader {
		LOD 200
		Tags { "RenderType"="Opaque"  }
			Pass {	
		
				//此PASS处理主色调
				CGPROGRAM
				#pragma vertex vert 
				#pragma fragment frag 
				#include "UnityCG.cginc"
		
				fixed4 _TintColor;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _Brightness;
				fixed4 _SpeColor;
				float4 _LightDir;
				half _Gloss;
				half _FresnelScale;

				struct a2v{
					float4 vertex:POSITION;			
					float2 texcoord:TEXCOORD0;	
					float3 normal:NORMAL;	
				};

				struct v2f{
					float4 pos:SV_POSITION;
					float2 uv:TEXCOORD0;	
					float3 worldPos:TEXCOORD1;
					float3 worldNormal:TEXCOORD2;
				};

				v2f vert(a2v v){
					v2f f;
					f.pos=UnityObjectToClipPos(v.vertex);
					f.uv.xy = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;	
					f.worldPos = mul(unity_ObjectToWorld,v.vertex);
					f.worldNormal = UnityObjectToWorldNormal(v.normal);		
					return f;
				}

				fixed _SizeX;
				fixed _SizeY;
				fixed _ChooseX;
				fixed _ChooseY;
          
				fixed2 UV(fixed2 uv){
					uv.x *= 1/_SizeX;
					uv.y *= 1/_SizeY;

					fixed X = 1/_SizeX;
					fixed Y = 1/_SizeY;

					uv.x += _ChooseX*X;
					uv.y += _ChooseY*Y;

					return uv;
				}

				fixed4 frag(v2f f):SV_Target{	
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(f.worldPos));
					fixed3 worldLightDir = normalize(_LightDir.xyz);	
					fixed3 worldHlafDir = normalize(worldViewDir + worldLightDir);
					fixed3 worldNormal = normalize(f.worldNormal);
					//高光
					fixed fresnel = _FresnelScale + (1 - _FresnelScale) * (_Gloss * _TintColor.a - dot(worldHlafDir, worldNormal));
					//主色调
					fixed3 mainColor = tex2D(_MainTex,UV(f.uv.xy)).rgb * _TintColor.rgb*_Brightness;
					//最终颜色
					fixed3 finColor =mainColor + lerp(0, _SpeColor.rgb,(saturate(fresnel)));
					return fixed4(finColor,1);				
				}
				ENDCG
			}		
							
			UsePass "FGame/static/ms/role_shadow/SHADOW"
		}

	SubShader {
		LOD 100
		Tags { "RenderType"="Opaque"  }
		UsePass "FGame/static/ms/diffuse/DIFFUSE"
	}
	FallBack "Unlit/Texture"
}
