Shader "Roma/Scene/BlinnPhong_Bump_Cube" {  
    Properties {  
        _MainTint ("Diffuse Tint", Color) = (1,1,1,1)  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
		_MainTint2 ("Diffuse Tint2", Color) = (1,1,1,1)  
		_MainTex2 ("Base2 (RGB)", 2D) = "white" {}  

		_BumpMap ("Normalmap", 2D) = "bump" {}
		_BumpSize ("NormalmapSize", Range (0.03, 1)) = 0.078125
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _Cubemap ("CubeMap", CUBE) = ""{}  
        _ReflAmount ("Reflection Amount", Range(0.01, 1)) = 0.5 
    }  
      
    SubShader {  
		Tags { "RenderType"="Opaque" }
		LOD 100

        CGPROGRAM  
		#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview
		fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
		{
			fixed diff = max (0, dot (s.Normal, lightDir));
			fixed nh = max (0, dot (s.Normal, halfDir));
			fixed spec = pow (nh, s.Specular*128) * s.Gloss;
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			UNITY_OPAQUE_ALPHA(c.a);
			return c;
		}

        float4 _MainTint;
        sampler2D _MainTex;
		float4 _MainTint2;
		sampler2D _MainTex2;
		sampler2D _BumpMap;
		half _BumpSize;
		half _Shininess;
        samplerCUBE _Cubemap;
        float _ReflAmount;  

        struct Input {  
            float2 uv_MainTex;
			float2 uv_MainTex2;
            float3 worldRefl; // 摄像机入射方向与法线计算出的反射向量（需声明内部数据）
			INTERNAL_DATA	
        };  
		
        void surf (Input IN, inout SurfaceOutput o) {  
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _MainTint;  
			half4 c2 = tex2D (_MainTex2, IN.uv_MainTex2) * _MainTint2;
            o.Albedo = c.rgb + c2.rgb;  
            o.Alpha = c.a;
			o.Specular = _Shininess;	
			o.Gloss = c.a;				
			// 通过反射向量，使用texCUBE进行纹理采样
			o.Emission = texCUBE(_Cubemap, 1 - IN.worldRefl).rgb * _ReflAmount;
			o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_MainTex)) * _BumpSize;
        }  
        ENDCG  
    }
}