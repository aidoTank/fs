Shader "FGame/static/ms/alpha" {
Properties {
	_TintColor("Tint Color", Color) = (1,1,1,1)
	_Brightness("Brightness",Range(-2,2)) = 1
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}	
	_Alpha("Alpha",Range(0,2))=1
	_AlphaBrightness("AlphaBrightness",Range(0,2)) = 0.2
	_AlphaTimeInterval ("AlphaTimeInterval",Range(0,20)) = 0.5
	_NoiseTex("NoiseTex",2D) = "white" {}	
	_NoiseBrightness("NoiseBrightness",Float) = 1
	_UvFlowX("UvFlowX",Float) = 1
	_UvFlowY("UvFlowY",Float) = 1
	_FresnelScale("FresnelScale",Float) = 1
	_GlossScale("GlossScale",Float) = 1
	_LightDir("LightDir",Vector)=(0.88,-0.83,0.18,0)
	_LightDir2("LightDir2",Vector)=(0.88,-0.83,0.18,0)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			half _AlphaTimeInterval;
			half _AlphaBrightness;
			fixed _Alpha;
			fixed _Brightness;
			sampler2D _NoiseTex;
			fixed4 _NoiseTex_ST;
			half _UvFlowX;
			half _UvFlowY;
			half _NoiseBrightness;
			half _FresnelScale;
			half _GlossScale;
			float4 _LightDir;

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				float3 normal:NORMAL;	
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed4 uv : TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
			};
						
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				o.uv.z = o.uv.z + _UvFlowX * _Time.y;
				o.uv.w = o.uv.w + _UvFlowY * _Time.y;
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 worldLightDir = normalize(_LightDir.xyz);
				fixed3 worldHlafDir = normalize(worldViewDir + worldLightDir);

				fixed noise = tex2D(_NoiseTex,i.uv.zw).r;
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * (_GlossScale - dot(worldHlafDir, worldNormal));

				fixed3 finColor =_Brightness* col.rgb * _TintColor * (noise+_NoiseBrightness);
				fixed alpha =_Alpha *( col.a + col.a * sin(_Time.y*_AlphaTimeInterval) * _AlphaBrightness);
				return fixed4(finColor,alpha * fresnel);
			}
		ENDCG
	}
	Pass {  
		Cull Front
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			half _AlphaTimeInterval;
			half _AlphaBrightness;
			fixed _Alpha;
			fixed _Brightness;
			sampler2D _NoiseTex;
			fixed4 _NoiseTex_ST;
			half _UvFlowX;
			half _UvFlowY;
			half _NoiseBrightness;
			half _FresnelScale;
			half _GlossScale;
			float4 _LightDir2;

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				float3 normal:NORMAL;	
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed4 uv : TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
			};
						
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				o.uv.z = o.uv.z + _UvFlowX * _Time.y;
				o.uv.w = o.uv.w + _UvFlowY * _Time.y;
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 worldLightDir = normalize(_LightDir2.xyz);
				fixed3 worldHlafDir = normalize(worldViewDir + worldLightDir);

				fixed noise = tex2D(_NoiseTex,i.uv.zw).r;
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * (_GlossScale - dot(worldHlafDir, worldNormal));

				fixed3 finColor =_Brightness* col.rgb * _TintColor * (noise+_NoiseBrightness);
				fixed alpha =_Alpha *( col.a + col.a * sin(_Time.y*_AlphaTimeInterval) * _AlphaBrightness);
				return fixed4(finColor,alpha * fresnel);
			}
		ENDCG
	}
}

}
