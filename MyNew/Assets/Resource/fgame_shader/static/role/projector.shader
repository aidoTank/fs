// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "FGame/Projector" {
    Properties{
		_TintColor("Tint Color", Color) = (1,1,1,1)
        _ShadowTex("Cookie", 2D) = "gray" {}
    }
        Subshader{
        Tags{ "Queue" = "Transparent" }
        Pass{
        ZWrite Off
        ColorMask RGB
      
        Blend SrcAlpha OneMinusSrcAlpha
        Offset -1, -1

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        #include "UnityCG.cginc"

		float4 _TintColor;

        struct v2f {
            float4 uvShadow : TEXCOORD0;
            //float4 uvFalloff : TEXCOORD1;
            //UNITY_FOG_COORDS(2)
            float4 pos : SV_POSITION;
        };

        float4x4 unity_Projector;
        //float4x4 unity_ProjectorClip;

        v2f vert(float4 vertex : POSITION)
        {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, vertex);
            o.uvShadow = mul(unity_Projector, vertex);
            //o.uvFalloff = mul(unity_ProjectorClip, vertex);
            //UNITY_TRANSFER_FOG(o,o.pos);
            return o;
        }

        sampler2D _ShadowTex;


        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
            //texS.a = 1.0-texS.a;
            return texS * _TintColor;
        }
            ENDCG
        }
    }
}