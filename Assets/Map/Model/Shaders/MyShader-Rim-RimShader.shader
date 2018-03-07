// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyShader/Rim/RimShader" {
    Properties{
        _RimColor("Rim Color",Color)=(1.0,1.0,1.0,1.0)//边缘光颜色
        _RimPower("Rim Power",Range(0.1,10))=3.0//Pow参数
        _RimIntensity("Rim Intensity",Range(0,100))=10//边缘光强度

        _MainTex("Base (RGB)",2D)="white"{}
    }
    SubShader{
        //当所有不透明物体渲染后开始渲染此物体
        Tags{"Queue"="Geometry+50" "RenderType"="Opaque"}

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ZTest Greater

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

                fixed4 _RimColor;
                float _RimPower;
                float _RimIntensity;

                struct a2v{
                    float4 vertex:POSITION;
                    float3 normal:NORMAL;
                };

                struct v2f{
                    float4 pos:SV_POSITION;
                    float4 worldPos:TEXCOORD0;
                    float3 worldNormal:TEXCOORD1;
                };

                v2f vert(a2v v){
                    v2f o;
                    o.pos=UnityObjectToClipPos(v.vertex);
                    o.worldPos=mul(unity_ObjectToWorld,v.vertex);
                    o.worldNormal=UnityObjectToWorldNormal(v.normal);
                    return o;
                }

                fixed4 frag(v2f i):SV_TARGET{
                    fixed3 worldNormalDir=normalize(i.worldNormal);
                    fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));
                    fixed rim=1-saturate(dot(worldNormalDir,worldViewDir));

                    fixed3 col=_RimColor.xyz*pow(rim,_RimPower)*_RimIntensity;
                    return fixed4(col,0.3);
                }
            ENDCG
        }

        Pass{
            Tags{"LightMode"="ForwardBase"}
            ZWrite On
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;


                struct a2v{
                    float4 vertex:POSITION;
                    float2 texcoord:TEXCOORD0;
                };

                struct v2f{
                    float4 pos:SV_POSITION;
                    float2 uv:TEXCOORD0;
                };

                v2f vert(a2v v){
                    v2f o;
                    o.pos=UnityObjectToClipPos(v.vertex);
                    o.uv=v.texcoord*_MainTex_ST.xy+_MainTex_ST.zw;
                    return o;
                }

                fixed4 frag(v2f i):SV_TARGET{
                    fixed3 col=tex2D(_MainTex,i.uv).rgb;

                    return fixed4(col,1);
                }

            ENDCG
        }


    }
    FallBack "Diffuse"
}