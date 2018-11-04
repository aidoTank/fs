Shader "FGame/static/ms/alpha_ui" {
Properties {
	_TintColor("Tint Color", Color) = (1,1,1,1)
	_Brightness("Brightness",Range(-2,2)) = 1
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}	
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

			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;			
			fixed _Brightness;


			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};
						
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed3 finColor =_Brightness* col.rgb * _TintColor;	
				return fixed4(finColor,col.a*_TintColor.a);
			}
		ENDCG
	}
}

}
