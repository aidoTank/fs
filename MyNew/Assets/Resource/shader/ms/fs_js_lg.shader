// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32878,y:32660,varname:node_4013,prsc:2|emission-8650-OUT,custl-9168-OUT;n:type:ShaderForge.SFN_Tex2d,id:6241,x:31999,y:32294,ptovrint:False,ptlb:Base (RGB),ptin:_MainTex,varname:node_6241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5889da698b338a34f86dc3cfe23108b3,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Time,id:2953,x:31014,y:32921,varname:node_2953,prsc:2;n:type:ShaderForge.SFN_Panner,id:2854,x:31573,y:32626,varname:node_2854,prsc:2,spu:-0.2,spv:-0.2|UVIN-6554-UVOUT,DIST-1207-OUT;n:type:ShaderForge.SFN_Multiply,id:2934,x:32002,y:32576,varname:node_2934,prsc:2|A-7422-RGB,B-363-RGB;n:type:ShaderForge.SFN_Vector1,id:1870,x:31144,y:32750,varname:node_1870,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1207,x:31359,y:32834,varname:node_1207,prsc:2|A-1681-OUT,B-2953-T;n:type:ShaderForge.SFN_TexCoord,id:6554,x:31281,y:32483,varname:node_6554,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:363,x:31772,y:32531,ptovrint:False,ptlb:node_363,ptin:_node_363,varname:node_363,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9b9a29ca746411146b01d3ed47c19128,ntxv:0,isnm:False|UVIN-2854-UVOUT;n:type:ShaderForge.SFN_Multiply,id:388,x:31986,y:32919,varname:node_388,prsc:2|A-2934-OUT,B-6052-RGB;n:type:ShaderForge.SFN_Color,id:6052,x:31650,y:32909,ptovrint:False,ptlb:zfg,ptin:_zfg,varname:node_6052,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:8650,x:32255,y:32849,varname:node_8650,prsc:2|A-388-OUT,B-2418-OUT;n:type:ShaderForge.SFN_Vector1,id:2418,x:32255,y:33018,varname:node_2418,prsc:2,v1:5;n:type:ShaderForge.SFN_NormalVector,id:8957,x:31248,y:33296,prsc:2,pt:False;n:type:ShaderForge.SFN_LightVector,id:1366,x:31248,y:33454,varname:node_1366,prsc:2;n:type:ShaderForge.SFN_ViewReflectionVector,id:2603,x:31283,y:34090,varname:node_2603,prsc:2;n:type:ShaderForge.SFN_Dot,id:7398,x:32139,y:33315,varname:node_7398,prsc:2,dt:4|A-8957-OUT,B-1366-OUT;n:type:ShaderForge.SFN_Dot,id:2236,x:31991,y:33941,varname:node_2236,prsc:2,dt:1|A-1366-OUT,B-2603-OUT;n:type:ShaderForge.SFN_Power,id:3303,x:32353,y:33734,varname:node_3303,prsc:2|VAL-2236-OUT,EXP-2909-OUT;n:type:ShaderForge.SFN_Exp,id:2909,x:31941,y:34112,varname:node_2909,prsc:2,et:0|IN-6122-OUT;n:type:ShaderForge.SFN_Slider,id:6122,x:31436,y:34233,ptovrint:False,ptlb:ggdaxiao,ptin:_ggdaxiao,varname:node_6122,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:3;n:type:ShaderForge.SFN_Multiply,id:2438,x:32572,y:33734,varname:node_2438,prsc:2|A-3303-OUT,B-3763-OUT;n:type:ShaderForge.SFN_Slider,id:3763,x:32186,y:34293,ptovrint:False,ptlb:ggqiangdu,ptin:_ggqiangdu,varname:node_3763,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:3;n:type:ShaderForge.SFN_Add,id:1491,x:32743,y:33304,varname:node_1491,prsc:2|A-8693-OUT,B-5705-OUT;n:type:ShaderForge.SFN_Multiply,id:9168,x:32647,y:32891,varname:node_9168,prsc:2|A-3686-OUT,B-1491-OUT,C-3797-OUT;n:type:ShaderForge.SFN_Color,id:6551,x:32723,y:33811,ptovrint:False,ptlb:ggyanse,ptin:_ggyanse,varname:node_6551,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.1310344,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5705,x:32953,y:33736,varname:node_5705,prsc:2|A-2438-OUT,B-6551-RGB,C-8418-OUT;n:type:ShaderForge.SFN_Add,id:8693,x:32479,y:33216,varname:node_8693,prsc:2|A-7225-OUT,B-7398-OUT;n:type:ShaderForge.SFN_Vector1,id:7225,x:32255,y:33141,varname:node_7225,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:8418,x:32837,y:34085,varname:node_8418,prsc:2,v1:1;n:type:ShaderForge.SFN_AmbientLight,id:4021,x:32878,y:33446,varname:node_4021,prsc:2;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:1891,x:32314,y:33464,varname:node_1891,prsc:2|IN-7398-OUT,IMIN-7461-OUT,IMAX-6643-OUT,OMIN-7270-OUT,OMAX-3339-OUT;n:type:ShaderForge.SFN_Slider,id:7461,x:31896,y:33505,ptovrint:False,ptlb:R1,ptin:_R1,varname:node_7461,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.65,max:1;n:type:ShaderForge.SFN_Slider,id:6643,x:31884,y:33586,ptovrint:False,ptlb:R2,ptin:_R2,varname:node_6643,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.04,max:1;n:type:ShaderForge.SFN_Slider,id:7270,x:31884,y:33679,ptovrint:False,ptlb:R3,ptin:_R3,varname:node_7270,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.76,max:1;n:type:ShaderForge.SFN_Slider,id:3339,x:31884,y:33772,ptovrint:False,ptlb:R4,ptin:_R4,varname:node_3339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.48,max:1;n:type:ShaderForge.SFN_Cubemap,id:2834,x:33102,y:33223,ptovrint:False,ptlb:node_2834,ptin:_node_2834,varname:node_2834,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:2ed157b36cbecf246ae49a487d3782d0,pvfc:0;n:type:ShaderForge.SFN_Vector1,id:7637,x:33102,y:33114,varname:node_7637,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Add,id:3797,x:33415,y:33148,varname:node_3797,prsc:2|A-7637-OUT,B-2834-RGB;n:type:ShaderForge.SFN_Vector1,id:4501,x:32264,y:34044,varname:node_4501,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:442,x:31690,y:34112,varname:node_442,prsc:2,v1:2;n:type:ShaderForge.SFN_Color,id:9612,x:31969,y:32110,ptovrint:False,ptlb:Main Color,ptin:_MainColor,varname:node_9612,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3686,x:32289,y:32170,varname:node_3686,prsc:2|A-9612-RGB,B-6241-RGB;n:type:ShaderForge.SFN_Tex2d,id:7422,x:32196,y:32382,ptovrint:False,ptlb:a,ptin:_a,varname:node_7422,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:1681,x:30845,y:32687,ptovrint:False,ptlb:SEEP,ptin:_SEEP,varname:node_1681,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;proporder:9612-6241-7422-363-6052-1681-6122-3763-6551-7461-6643-7270-3339-2834;pass:END;sub:END;*/

Shader "Roma/Role/fs_js_lg" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _a ("a", 2D) = "white" {}
        _node_363 ("node_363", 2D) = "white" {}
        _zfg ("zfg", Color) = (1,0,0,1)
        _SEEP ("SEEP", Range(0, 3)) = 1
        _ggdaxiao ("ggdaxiao", Range(0, 3)) = 2
        _ggqiangdu ("ggqiangdu", Range(0, 3)) = 2
        _ggyanse ("ggyanse", Color) = (0,0.1310344,1,1)
        _R1 ("R1", Range(0, 1)) = 0.65
        _R2 ("R2", Range(0, 1)) = 0.04
        _R3 ("R3", Range(0, 1)) = 0.76
        _R4 ("R4", Range(0, 1)) = 0.48
        _node_2834 ("node_2834", Cube) = "_Skybox" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _node_363; uniform float4 _node_363_ST;
            uniform float4 _zfg;
            uniform float _ggdaxiao;
            uniform float _ggqiangdu;
            uniform float4 _ggyanse;
            uniform samplerCUBE _node_2834;
            uniform float4 _MainColor;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform float _SEEP;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
////// Emissive:
                float4 _a_var = tex2D(_a,TRANSFORM_TEX(i.uv0, _a));
                float4 node_2953 = _Time + _TimeEditor;
                float2 node_2854 = (i.uv0+(_SEEP*node_2953.g)*float2(-0.2,-0.2));
                float4 _node_363_var = tex2D(_node_363,TRANSFORM_TEX(node_2854, _node_363));
                float3 emissive = (((_a_var.rgb*_node_363_var.rgb)*_zfg.rgb)*5.0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_7398 = 0.5*dot(i.normalDir,lightDirection)+0.5;
                float3 finalColor = emissive + ((_MainColor.rgb*_MainTex_var.rgb)*((0.3+node_7398)+((pow(max(0,dot(lightDirection,viewReflectDirection)),exp(_ggdaxiao))*_ggqiangdu)*_ggyanse.rgb*1.0))*(0.6+texCUBE(_node_2834,viewReflectDirection).rgb));
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _node_363; uniform float4 _node_363_ST;
            uniform float4 _zfg;
            uniform float _ggdaxiao;
            uniform float _ggqiangdu;
            uniform float4 _ggyanse;
            uniform samplerCUBE _node_2834;
            uniform float4 _MainColor;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform float _SEEP;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_7398 = 0.5*dot(i.normalDir,lightDirection)+0.5;
                float3 finalColor = ((_MainColor.rgb*_MainTex_var.rgb)*((0.3+node_7398)+((pow(max(0,dot(lightDirection,viewReflectDirection)),exp(_ggdaxiao))*_ggqiangdu)*_ggyanse.rgb*1.0))*(0.6+texCUBE(_node_2834,viewReflectDirection).rgb));
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
