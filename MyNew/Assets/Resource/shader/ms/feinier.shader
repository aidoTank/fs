// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.19 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.19;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-6822-OUT;n:type:ShaderForge.SFN_Fresnel,id:1493,x:31827,y:33193,varname:node_1493,prsc:2|EXP-3286-OUT;n:type:ShaderForge.SFN_Vector1,id:3286,x:31639,y:33211,varname:node_3286,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Tex2d,id:3219,x:31827,y:32972,ptovrint:False,ptlb:node_3219,ptin:_node_3219,varname:node_3219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2545,x:32104,y:33144,varname:node_2545,prsc:2|A-3219-RGB,B-1493-OUT,C-7297-RGB;n:type:ShaderForge.SFN_Color,id:7297,x:31827,y:33363,ptovrint:False,ptlb:node_7297,ptin:_node_7297,varname:node_7297,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:8248,x:31954,y:32587,ptovrint:False,ptlb:node_8248,ptin:_node_8248,varname:node_8248,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2cb84c21e30fad847adbfe71b870fb5a,ntxv:0,isnm:False|UVIN-1501-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3919,x:31954,y:32775,ptovrint:False,ptlb:node_3919,ptin:_node_3919,varname:node_3919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2ff97a2a47116ba429411c084c7de3f9,ntxv:3,isnm:False|UVIN-6995-UVOUT;n:type:ShaderForge.SFN_Panner,id:1501,x:31798,y:32549,varname:node_1501,prsc:2,spu:0,spv:0.05;n:type:ShaderForge.SFN_Panner,id:6995,x:31798,y:32739,varname:node_6995,prsc:2,spu:0.05,spv:0;n:type:ShaderForge.SFN_Multiply,id:7155,x:32167,y:32680,varname:node_7155,prsc:2|A-8248-RGB,B-3919-RGB,C-9120-RGB;n:type:ShaderForge.SFN_Add,id:6822,x:32490,y:32823,varname:node_6822,prsc:2|A-7155-OUT,B-2545-OUT;n:type:ShaderForge.SFN_Color,id:9120,x:32198,y:32858,ptovrint:False,ptlb:node_9120,ptin:_node_9120,varname:node_9120,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:3219-7297-8248-3919-9120;pass:END;sub:END;*/

Shader "Shader Forge/feinier" {
    Properties {
        _node_3219 ("node_3219", 2D) = "white" {}
        _node_7297 ("node_7297", Color) = (1,1,1,1)
        _node_8248 ("node_8248", 2D) = "white" {}
        _node_3919 ("node_3919", 2D) = "bump" {}
        _node_9120 ("node_9120", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
			Cull back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _TimeEditor;
            uniform sampler2D _node_3219; uniform float4 _node_3219_ST;
            uniform float4 _node_7297;
            uniform sampler2D _node_8248; uniform float4 _node_8248_ST;
            uniform sampler2D _node_3919; uniform float4 _node_3919_ST;
            uniform float4 _node_9120;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_5424 = _Time + _TimeEditor;
                float2 node_1501 = (i.uv0+node_5424.g*float2(0,0.05));
                float4 _node_8248_var = tex2D(_node_8248,TRANSFORM_TEX(node_1501, _node_8248));
                float2 node_6995 = (i.uv0+node_5424.g*float2(0.05,0));
                float4 _node_3919_var = tex2D(_node_3919,TRANSFORM_TEX(node_6995, _node_3919));
                float4 _node_3219_var = tex2D(_node_3219,TRANSFORM_TEX(i.uv0, _node_3219));
                float3 emissive = ((_node_8248_var.rgb*_node_3919_var.rgb*_node_9120.rgb)+(_node_3219_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),0.8)*_node_7297.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
