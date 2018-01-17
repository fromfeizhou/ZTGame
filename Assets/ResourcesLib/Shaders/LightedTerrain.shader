Shader "Custom/Environment/LightedTerrain"
{
    Properties
    {
        _Texture0 ("Texture 1", 2D) = "white" {}
        _Texture1 ("Texture 2", 2D) = "white" {}
        _Texture2 ("Texture 3", 2D) = "white" {}
        _Texture3 ("Texture 4", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        
        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _Texture0;
            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 weight : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                SHADOW_COORDS(4)
                UNITY_FOG_COORDS(5)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.weight = v.tangent;
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 t0 = tex2D(_Texture0, i.uv);
                fixed4 t1 = tex2D(_Texture1, i.uv);
                fixed4 t2 = tex2D(_Texture2, i.uv);
                fixed4 t3 = tex2D(_Texture3, i.uv);
                fixed4 tex = t0 * i.weight.x + t1 * i.weight.y + t2 * i.weight.z + t3 * i.weight.w;

                fixed3 albedo = tex.rgb;

                fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.rgb;

                float3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float halfLambert = dot(worldLight, i.worldNormal) * 0.5 + 0.5;
                fixed3 diffuse = albedo * _LightColor0.rgb * halfLambert;

                float3 worldView = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 halfDir = normalize(worldView + worldLight);
                fixed3 specular = albedo * _LightColor0.rgb * max(dot(halfDir, i.worldNormal), 0);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed4 col = fixed4(ambient + (diffuse + specular) * atten, tex.a);
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }

            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardAdd"
            }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _Texture0;
            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 weight : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.weight = v.tangent;
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 t0 = tex2D(_Texture0, i.uv);
                fixed4 t1 = tex2D(_Texture1, i.uv);
                fixed4 t2 = tex2D(_Texture2, i.uv);
                fixed4 t3 = tex2D(_Texture3, i.uv);
                fixed4 tex = t0 * i.weight.x + t1 * i.weight.y + t2 * i.weight.z + t3 * i.weight.w;

                fixed3 albedo = tex.rgb;

                float3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float halfLambert = dot(worldLight, i.worldNormal) * 0.5 + 0.5;
                fixed3 diffuse = albedo * _LightColor0.rgb * halfLambert;

                float3 worldView = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 halfDir = normalize(worldView + worldLight);
                fixed3 specular = albedo * _LightColor0.rgb * max(dot(halfDir, i.worldNormal), 0);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed4 col = fixed4((diffuse + specular ) * atten, tex.a);

                return col;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}