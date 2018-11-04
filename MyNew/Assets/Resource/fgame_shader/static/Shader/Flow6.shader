Shader "Custom/Flow6" {
	Properties {
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FlowColor ("Flow Color (A)", Color) = (1, 1, 1, 1)
		_FlowTexture ("Flow Texture", 2D) = ""{}
		_FlowMap ("FlowMap (RG) Alpha (B)", 2D) = ""{}
		_FlowMapOffset("FlowMap Offset", Vector) = (1, 1, 1, 1)
//		_FlowSpeed("Flow Speed", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		//#pragma Lighting false

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 2.0

		fixed4 _BaseColor;
		sampler2D _MainTex;
		fixed4 _FlowColor;
		sampler2D _FlowTexture;
		sampler2D _FlowMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FlowTexture;
			float2 uv_FlowMap;
		};

		float4 _FlowMapOffset;
		//float4 _FlowSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			half4 mainColor = tex2D (_MainTex, IN.uv_MainTex);

			half4 flowMap = tex2D (_FlowMap, IN.uv_FlowMap);
			flowMap.r = flowMap.r * 2 - 1;
			flowMap.g = flowMap.g * 2 - 1;

			float phase1 = _FlowMapOffset.x;
			float phase2 = _FlowMapOffset.y;

			half4 t1 = tex2D (_FlowTexture, IN.uv_FlowTexture + flowMap.rg * phase1);
			half4 t2 = tex2D (_FlowTexture, IN.uv_FlowTexture + flowMap.rg * phase2);
			half4 final = lerp( t1, t2, _FlowMapOffset.z);

			half flowMapColor = flowMap.b * _FlowColor.a;

			mainColor.rgb *= _BaseColor.rgb * (1 - flowMapColor);
			final.rgb *= _FlowColor.rgb * flowMapColor;

			o.Albedo = mainColor.rgb + final.rgb;
			//o.Albedo = mainColor.rgb;
		}
		ENDCG
	}
	//FallBack "Diffuse"
}
