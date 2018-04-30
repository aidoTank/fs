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
			// �ر������޷�����
			Lighting On
			SetTexture[_MainTex]{
					// Pull the color property into this blender
					// ʹ��ɫ���Խ�������
					constantColor[_TintColor]
					// And use the texture's alpha to blend between it and
					// vertex color
					// ʹ�������alphaͨ����϶�����ɫ
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