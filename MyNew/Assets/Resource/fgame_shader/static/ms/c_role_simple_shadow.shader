Shader "FGame/static/ms/role_shadow" {
	Properties {
		_ShadowColor("ShadowColor",Color)=(0.1,0.1,0.1,0.435)
		_ShadowPos("ShadowPos",Vector)=(-0.26,-0.05,0.2,0.02)

	}
	SubShader {

		Pass {	
			//投影到地面的阴影		
			NAME "SHADOW"			
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
				float4 pos:SV_POSITION;
				float colorA:TEXCOORD0;
			};

			v2f vert(a2v v){
				v2f f;
				float4 shadowPos;
				float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
				shadowPos.x = worldPos.x + lerp(0,1,abs(worldPos.y) * _ShadowPos.x);			
				shadowPos.z = worldPos.z + lerp(0,1,abs(worldPos.y) * _ShadowPos.z);								
				shadowPos.y = _ShadowPos.y;	
				float3 center = float3(unity_ObjectToWorld[0].w,shadowPos.y,unity_ObjectToWorld[2].w);
				float3 shadowCenter = float3(shadowPos.x,shadowPos.y,shadowPos.z);
				f.colorA = distance(center,shadowCenter);
				f.pos= UnityWorldToClipPos(shadowPos);		
				return f;
			}

			fixed4 frag(v2f f):Color{	
				return fixed4(_ShadowColor.xyz,_ShadowColor.a * smoothstep(1,0,f.colorA*_ShadowPos.w));
			}
			ENDCG
		}	
	}
}
