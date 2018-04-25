// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PengLuOccTransVF" {
    Properties     
    {     
        _MainTex ("Base (RGB)", 2D) = "white" {}    
        _RimColor("RimColor",Color) = (0,1,1,1)  
        _RimPower ("Rim Power", Range(0.1,8.0)) = 1.0  
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		[NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
    }    
        
    SubShader     
    {    
          
        LOD 300    
        Tags { "Queue" = "Geometry+500" "RenderType"="Opaque" }   
        Pass  
        {  
            Blend SrcAlpha One  
            ZWrite off  
            Lighting off  
  
            ztest greater  
  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"  
  
            float4 _RimColor;  
            float _RimPower;  
              
            struct appdata_t {  
                float4 vertex : POSITION;  
                float2 texcoord : TEXCOORD0;  
                float4 color:COLOR;  
                float4 normal:NORMAL;  
            };  
  
            struct v2f {  
                float4  pos : SV_POSITION;  
                float4  color:COLOR;  
            } ;  
            v2f vert (appdata_t v)  
            {  
                v2f o;  
                o.pos = UnityObjectToClipPos(v.vertex);  
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));  
                float rim = 1 - saturate(dot(viewDir,v.normal ));  
                o.color = _RimColor*pow(rim,_RimPower);  
                return o;  
            }  
            float4 frag (v2f i) : COLOR  
            {  
                return i.color;   
            }  
            ENDCG  
        }  
        pass    
        {    
            ZWrite on  
            ZTest less   
  
            /*CGPROGRAM
			#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

			inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
			{
				fixed diff = max (0, dot (s.Normal, lightDir));
				fixed nh = max (0, dot (s.Normal, halfDir));
				fixed spec = pow (nh, s.Specular*128) * s.Gloss;
				
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
				UNITY_OPAQUE_ALPHA(c.a);
				return c;
			}

			sampler2D _MainTex;
			sampler2D _BumpMap;
			half _Shininess;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb;
				o.Gloss = tex.a;
				o.Alpha = tex.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_MainTex));
			}
				
			ENDCG   */
        }

			CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

				inline fixed4 LightingMobileBlinnPhong(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
			{
				fixed diff = max(0, dot(s.Normal, lightDir));
				fixed nh = max(0, dot(s.Normal, halfDir));
				fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
				UNITY_OPAQUE_ALPHA(c.a);
				return c;
			}

			sampler2D _MainTex;
			sampler2D _BumpMap;
			half _Shininess;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb;
				o.Gloss = tex.a;
				o.Alpha = tex.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}
			ENDCG

    }   
    FallBack "Diffuse"  
}
