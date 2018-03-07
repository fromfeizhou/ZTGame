Shader "Unlit/Alpha BlendedTest"
{
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {

	 Pass {
		ZWrite On
		ColorMask 0
	
	
	 }

		Pass {
		Blend SrcAlpha OneMinusSrcAlpha
	 Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
FallBack "Diffuse" 
}
