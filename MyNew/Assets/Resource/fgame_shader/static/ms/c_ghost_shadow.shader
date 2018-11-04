Shader "FGame/static/ms/ghost_shadow" {
	   Properties {
	   	_TintColor ("Color", Color) = (1,1,1,1)	
        _MainTex ("Main Tex", 2D) = "white" {}
        _DotProduct("Normal intensity",Range(-2,1))=0.25
    }
    SubShader {
        Tags { "Queue"="Transparent" "IngnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        CGPROGRAM
     
        #pragma surface surf Lambert alpha

		fixed4 _TintColor;
        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
        };

        float _DotProduct;

        void surf (Input IN, inout SurfaceOutput o) {

            fixed4 c = tex2D(_MainTex,IN.uv_MainTex) * 2 * _TintColor;
            o.Albedo = c.rgb;

            float border = 1-(abs(dot(IN.viewDir,IN.worldNormal)));
            float alpha = (border * (1-_DotProduct) + _DotProduct);

            o.Alpha = c.a*alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}