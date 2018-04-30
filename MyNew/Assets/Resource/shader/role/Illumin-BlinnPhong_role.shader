Shader "Roma/Role/Self-Illumin_BlinnPhong_Role" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)// 高光颜色
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125// 高光指数【光泽度】
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf BlinnPhongTest
		// 半角向量和BilnnPhong:使用入射光线和视线的中间平均值，即半角向量，然后使用该半角和法线计算出一个和视角相关的高光。
		// 相关参数：【lightDir点到光源单位向量】【viewDir点到摄像机单位向量】【atten衰减系数】【_LightColor0场景中平行光的颜色】
		float4 LightingBlinnPhongTest(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			// 1.半角向量：求（点到光源+点到摄像机）的单位向量，他们的中间平均值
			float3 h = normalize(lightDir + viewDir);
			// 2.漫反射系数【点到光源单位向量与法线向量的余弦值】
			float diff = max(0, dot(s.Normal, lightDir));
			// 3.高光底数【半角向量与法线向量的余弦值】
			float nh = max(0, dot(s.Normal, h));
			// 4.高光系数：根据高光低数和高光指数求得
			float spec = pow(nh, s.Specular * 128.0) * s.Gloss;
			float4 c;
			// 5.最终光照rgb = 漫反射+半角高光
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
			return c;
		}

		sampler2D _MainTex; // 主材质
		fixed4 _Color;  // 主材质颜色
		fixed _Shininess; // 高光指数
		struct Input {
			float2 uv_MainTex; // 主材质的UV信息
		};

		void surf(Input IN, inout SurfaceOutput o) {
			// 取主纹理的对应当前像素点的值
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			// Albedo反照率，即物体反射光的数量与外来光数量的比值。
			// Albedo = 主纹理 x 主色调，反映了物体的基色，与任何光相关的信息（比如diffuse, shiness等）无关
			o.Albedo = tex.rgb * _Color.rgb;    // 颜色纹理=主纹理*主材质颜色

			// Gloss光滑度[0, 1]，用于控制反射的模糊程度，值越大，高光反射越清晰，反之则越模糊，0将无高光效果。
			// 光滑度的“滑”是面的概念，代表物体整体的光滑程度
			// 比如说，同样一块金属，在它生锈的过程中，其反射就会慢慢变弱，可以通过Gloss值控制
			// 实际上它是针对高光计算结果的附加系数
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;      // 透明度
			// Shininess光泽度[0, 1]，又叫高光指数或镜面反射指数，注意，它在SurfaceOutput结构中的命名（Specular）很容易让人误解为它是高光强度，其实不然，它是高光指数
			// 光泽度的“泽”是点的概念，代表物体某个高光点的光泽程度
			o.Specular = _Shininess; // 越小光泽度越高，0为全白
			o.Emission = tex.rgb * 0.85f;
		}
		ENDCG
	}
}
