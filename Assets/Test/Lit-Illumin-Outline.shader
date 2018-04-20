Shader "MikuArtTech/Scene/Lit-Illumin-Outline" {
	Properties{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		_EmissionColor("Emission Color (RGB)", Color) = (0, 0, 0, 1)
		_EmissionMap("Emission (RGB)", 2D) = "white" {}
		_Emission("Emission (Lightmapper)", Float) = 1.0
	}

	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Illum;
		fixed4 _Color;
		half4  _EmissionColor;
		sampler2D _EmissionMap;
		fixed _Emission;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionMap;
			float2 uv_Illum;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
			o.Emission = _EmissionColor.rgb * tex2D(_EmissionMap, IN.uv_EmissionMap).rgb;
#if defined (UNITY_PASS_META)
			o.Emission *= _Emission.rrr;
#endif
			o.Alpha = c.a;
		}
		ENDCG

		UsePass "Hidden/Toon/Basic Outline/OUTLINE"
	}

	FallBack "Legacy Shaders/Self-Illumin/VertexLit"
	CustomEditor "LegacyIlluminShaderGUI"
}
