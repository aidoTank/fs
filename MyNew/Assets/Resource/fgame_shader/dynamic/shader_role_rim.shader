// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "FGame/dynamic/role_rim" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Shininess ("Shininess", Range (0.01, 1)) = 0.1
		_RimColor ("Rim Color", Color) = (1.0, 1.0,0.0,0.0)
		_RimPower ("Rim Power", Range(0,1)) = 0.2

		_ShadowColor("ShadowColor",Color)=(0.1,0.1,0.1,0.435)
		_ShadowPos("ShadowPos",Vector)=(-0.26,-0.05,0.2,0.02)
	 }

	SubShader {
		LOD 200
		Tags { "RenderType"="Opaque"}
		Pass {
     
			CGPROGRAM
     
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed _Shininess; 
			fixed4 _RimColor;
			fixed _RimPower;

			struct a2v {
				fixed4 vertex : POSITION;  
				fixed3 normal : NORMAL;   
				fixed4 texcoord : TEXCOORD0;
			};
     
			struct v2f {
				fixed4 pos : POSITION; 
				fixed2 uv : TEXCOORD0;
				fixed3 lightDir: TEXCOORD1; 
				fixed3 viewDir : TEXCOORD2; 
				fixed3 normal : TEXCOORD3;
			};
     
			v2f vert(a2v v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.lightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = normalize(v.normal);
				return o;
			}
     
			fixed4 frag(v2f i) : COLOR {
				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 diff = max(0, dot(i.normal, i.lightDir));
				// 半角
				fixed3 h = normalize(i.lightDir + i.viewDir);
				// 高光底数
				fixed nh = max(0, dot(i.normal,h));
				// 高光系数
				fixed spec = pow(nh,_Shininess*128);
				// 边缘
				fixed rim = 1.0f - saturate(dot(normalize(i.viewDir), i.normal));
				texColor.rgb = fixed3((ambi + diff + spec + 0.3f) * texColor) +  _RimColor * pow(rim, _RimPower * 4.0f);
				texColor.a = 1.0f;
				return texColor;
			}
			ENDCG
		}

		Pass {
			Name "SHADOW"
			Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrSat
                Fail keep
                ZFail keep
            }

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 
			#include "UnityCG.cginc"

			float4 _ShadowPos;
			float4 _ShadowColor;


			struct a2v{
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct v2f{
				float4 pos : SV_POSITION;
				float dis : TEXCOORD0;
			};

			v2f vert(a2v v){
				v2f f;
				float4 shadowPos;
				float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
				// 投射影子，让当前顶点的XZ的位置 + 当前顶点的高度作为偏移值
				// 然后将所有顶点的Y坐标调到0点
				shadowPos.x = worldPos.x + abs(worldPos.y) * _ShadowPos.x;			
				shadowPos.z = worldPos.z + abs(worldPos.y) * _ShadowPos.z;								
				shadowPos.y = _ShadowPos.y;	
				// 当前对象中心点
				float3 center = float3(unity_ObjectToWorld[0].w, shadowPos.y, unity_ObjectToWorld[2].w);
				// 当前顶点位置和中心点的距离
				f.dis = distance(center, shadowPos);
				// 转到裁剪空间
				f.pos = UnityWorldToClipPos(shadowPos);
				return f;
			}

			fixed4 frag(v2f f):Color{		
				return fixed4(_ShadowColor.xyz,
				_ShadowColor.a * smoothstep(1, 0, f.dis * _ShadowPos.w));
			}
			ENDCG
		}
	}

	// 无影子的
	SubShader {

		Tags { "RenderType"="Opaque"}
		Pass {
     
			CGPROGRAM
     
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed _Shininess; 
			fixed4 _RimColor;
			fixed _RimPower;

			struct a2v {
				fixed4 vertex : POSITION;  
				fixed3 normal : NORMAL;   
				fixed4 texcoord : TEXCOORD0;
			};
     
			struct v2f {
				fixed4 pos : POSITION; 
				fixed2 uv : TEXCOORD0;
				fixed3 lightDir: TEXCOORD1; 
				fixed3 viewDir : TEXCOORD2; 
				fixed3 normal : TEXCOORD3;
			};
     
			v2f vert(a2v v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.lightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = normalize(v.normal);
				return o;
			}
     
			fixed4 frag(v2f i) : COLOR {
				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 diff = max(0, dot(i.normal, i.lightDir));
				// 半角
				fixed3 h = normalize(i.lightDir + i.viewDir);
				// 高光底数
				fixed nh = max(0, dot(i.normal,h));
				// 高光系数
				fixed spec = pow(nh,_Shininess*128);
				// 边缘
				fixed rim = 1.0f - saturate(dot(normalize(i.viewDir), i.normal));
				texColor.rgb = fixed3((ambi + diff + spec + 0.3f) * texColor) +  _RimColor * pow(rim, _RimPower * 4.0f);
				texColor.a = 1.0f;
				return texColor;
			}
			ENDCG
		}
	}
}
    