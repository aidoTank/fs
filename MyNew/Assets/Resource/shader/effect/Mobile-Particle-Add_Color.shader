// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Roma/Particles/Additive_Color" {
Properties {
	_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
				Pass {
					Material{
					Diffuse[_TintColor]
					Ambient[_TintColor]
				}
			// 关闭亮度无法叠加
			Lighting On
			SetTexture[_MainTex]{
					// Pull the color property into this blender
					// 使颜色属性进入混合器
					constantColor[_TintColor]
					// And use the texture's alpha to blend between it and
					// vertex color
					// 使用纹理的alpha通道混合顶点颜色
					combine constant lerp(texture) previous
				}
				// Multiply in texture
				SetTexture[_MainTex]{
					combine previous * texture
				}
			}
	}
}
}