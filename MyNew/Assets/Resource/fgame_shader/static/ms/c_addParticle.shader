// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FGame/static/ms/c_addParticle" {
	Properties {		
		_MainTex ("Main Tex", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_UvAniX("UvAniX",Float) = 0    //X轴方向UV流动，X轴扭动效果会叠加，所以有值时候_CenterDir要勾选
		_UvAniY("UvAniY",Float) = 0	   //Y轴方向UV流动，Y轴扭动效果会叠加，所以有值时候_CenterDir不要勾选
		_UvFlowX("UvFlowX",Float) = 2  //X轴方向UV偏移量
		_UvFlowY("UvFlowY",Float) = 8  //Y轴方向UV偏移量
		[Toggle]_FixationPostion("FixationPostion",Float)=0 //不勾选为不固定方向扭动，_CenterDir失效，勾选为固定方向扭动，_CenterDir生效
		[Toggle]_CenterDir("CenterDir",Float) = 0   //勾选为Y轴方向扭动，不勾选为X轴方向扭动
		_UvFlow("UvFlow",Vector) = (0,0,0,0)  //x,y为UV的位置，最小值0，最大值1，z为抖动频率，w为幅度
		_AddParticle("AddParticle",Range(-2,2))=0
		_OffectVertex("OffectVertex",Vector) = (0,0,0,1)
		
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass {			

			ZWrite Off
			Cull Off
			Blend SrcAlpha One			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
					
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			half _UvAniX;
			half _UvAniY;
			half _UvFlowX;
			half _UvFlowY;
			float4 _UvFlow;
			fixed _AddParticle;
			float4 _OffectVertex;
			fixed4 _Color;
			fixed _CenterDir;
			fixed _FixationPostion;
			
			struct a2v {
				float4 vertex : POSITION;				
				float2 texcoord : TEXCOORD0;
				fixed4 vertexColor: COLOR0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;				
				float2 uv : TEXCOORD2;
				fixed4 color: TEXCOORD0;
				float3 worldPos: TEXCOORD1;
			
			};
			
			v2f vert(a2v v) {
				v2f o;
				v.vertex.x =v.vertex.x + sin(v.vertex.x * _Time.y * _OffectVertex.w) * _OffectVertex.x;
				v.vertex.y =v.vertex.y + sin(v.vertex.y * _Time.y * _OffectVertex.w) * _OffectVertex.y;
				v.vertex.z =v.vertex.z + sin(v.vertex.z * _Time.y * _OffectVertex.w) * _OffectVertex.z;

				o.color = v.vertexColor;		
				_MainTex_ST.z = _Time.y * _UvAniX;
				_MainTex_ST.w = _Time.y * _UvAniY;
				o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);	

				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {	
			
				half2 center = i.uv - _UvFlow.xy;
				half centerDir = lerp(center.x,center.y,_CenterDir);
				half rotateX = sin(length(center * _UvFlowX) - _Time.y*_UvFlow.z)*_UvFlow.w;	
				half rotateY = sin(length(center * _UvFlowY) - _Time.y*_UvFlow.z)*_UvFlow.w;	
				half rotateFixationX = centerDir * sin(length(center * _UvFlowX) - _Time.y*_UvFlow.z)*_UvFlow.w;	
				half rotateFixationY = centerDir * sin(length(center * _UvFlowY) - _Time.y*_UvFlow.z)*_UvFlow.w;
				half finalRotateX = lerp(rotateX,rotateFixationX,_FixationPostion);
				half finalRotateY = lerp(rotateY,rotateFixationY,_FixationPostion);
				i.uv.x = i.uv.x + finalRotateX;
				i.uv.y = i.uv.y + finalRotateY;		
				//UV流动效果
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);

			    //透明值整合
				fixed alpha = texColor.a * i.color.a;

				return fixed4(_Color.rgb * texColor.rgb * i.color.xyz +_Color.rgb * texColor.rgb*_AddParticle,alpha);
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
