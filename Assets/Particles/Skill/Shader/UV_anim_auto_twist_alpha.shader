// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33339,y:32712,varname:node_9361,prsc:2|emission-381-OUT,alpha-5819-OUT;n:type:ShaderForge.SFN_Tex2d,id:2038,x:31889,y:32571,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5889-OUT;n:type:ShaderForge.SFN_Multiply,id:2985,x:32392,y:32624,varname:node_2985,prsc:2|A-2038-RGB,B-7287-RGB,C-2516-RGB,D-1230-OUT;n:type:ShaderForge.SFN_VertexColor,id:7287,x:31772,y:32735,varname:node_7287,prsc:2;n:type:ShaderForge.SFN_Color,id:2516,x:31871,y:32925,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:1230,x:32046,y:32762,varname:node_1230,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:7795,x:32513,y:33014,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_5890,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_DepthBlend,id:8194,x:32742,y:32582,varname:node_8194,prsc:2;n:type:ShaderForge.SFN_Multiply,id:381,x:33022,y:32783,varname:node_381,prsc:2|A-8194-OUT,B-2985-OUT,C-7795-A;n:type:ShaderForge.SFN_Tex2d,id:1741,x:31090,y:32515,ptovrint:False,ptlb:Noise1,ptin:_Noise1,varname:node_5105,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9493-OUT;n:type:ShaderForge.SFN_Tex2d,id:3244,x:31110,y:32766,ptovrint:False,ptlb:Noise2,ptin:_Noise2,varname:node_7759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8671-OUT;n:type:ShaderForge.SFN_Append,id:6153,x:30502,y:32483,varname:node_6153,prsc:2|A-5284-OUT,B-9993-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5284,x:30309,y:32483,ptovrint:False,ptlb:1 U speed,ptin:_1Uspeed,varname:node_1205,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9993,x:30309,y:32546,ptovrint:False,ptlb:1 V speed,ptin:_1Vspeed,varname:node_8234,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:9332,x:30502,y:32645,varname:node_9332,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1624,x:30707,y:32523,varname:node_1624,prsc:2|A-6153-OUT,B-9332-T;n:type:ShaderForge.SFN_TexCoord,id:5405,x:30731,y:32655,varname:node_5405,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:9493,x:30905,y:32523,varname:node_9493,prsc:2|A-1624-OUT,B-5405-UVOUT;n:type:ShaderForge.SFN_Append,id:2143,x:30517,y:32804,varname:node_2143,prsc:2|A-3881-OUT,B-4548-OUT;n:type:ShaderForge.SFN_Add,id:8671,x:30920,y:32766,varname:node_8671,prsc:2|A-5405-UVOUT,B-569-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3881,x:30294,y:32772,ptovrint:False,ptlb:2 U speed,ptin:_2Uspeed,varname:node_4913,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:4548,x:30294,y:32867,ptovrint:False,ptlb:2 V speed,ptin:_2Vspeed,varname:node_1874,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:569,x:30709,y:32804,varname:node_569,prsc:2|A-9332-T,B-2143-OUT;n:type:ShaderForge.SFN_Add,id:5709,x:31310,y:32649,varname:node_5709,prsc:2|A-1741-R,B-3244-R;n:type:ShaderForge.SFN_Multiply,id:2488,x:31472,y:32649,varname:node_2488,prsc:2|A-5709-OUT,B-4037-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4037,x:31304,y:32859,ptovrint:False,ptlb:Twist,ptin:_Twist,varname:node_305,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:5224,x:31472,y:32491,varname:node_5224,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5889,x:31710,y:32571,varname:node_5889,prsc:2|A-5224-UVOUT,B-2488-OUT;n:type:ShaderForge.SFN_Multiply,id:6345,x:31312,y:32260,varname:node_6345,prsc:2|A-1741-RGB,B-3244-RGB;n:type:ShaderForge.SFN_Multiply,id:5819,x:33095,y:32952,varname:node_5819,prsc:2|A-2038-A,B-7287-A,C-7795-A,D-7661-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7661,x:31582,y:32271,varname:node_7661,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6345-OUT;proporder:2038-2516-7795-1741-3244-5284-9993-3881-4548-4037;pass:END;sub:END;*/

Shader "ME/UV_anim_auto_twist_alpha" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "white" {}
        _Noise1 ("Noise1", 2D) = "white" {}
        _Noise2 ("Noise2", 2D) = "white" {}
        _1Uspeed ("1 U speed", Float ) = 0
        _1Vspeed ("1 V speed", Float ) = 0
        _2Uspeed ("2 U speed", Float ) = 0
        _2Vspeed ("2 V speed", Float ) = 0
        _Twist ("Twist", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Noise1; uniform float4 _Noise1_ST;
            uniform sampler2D _Noise2; uniform float4 _Noise2_ST;
            uniform float _1Uspeed;
            uniform float _1Vspeed;
            uniform float _2Uspeed;
            uniform float _2Vspeed;
            uniform float _Twist;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 node_9332 = _Time + _TimeEditor;
                float2 node_9493 = ((float2(_1Uspeed,_1Vspeed)*node_9332.g)+i.uv0);
                float4 _Noise1_var = tex2D(_Noise1,TRANSFORM_TEX(node_9493, _Noise1));
                float2 node_8671 = (i.uv0+(node_9332.g*float2(_2Uspeed,_2Vspeed)));
                float4 _Noise2_var = tex2D(_Noise2,TRANSFORM_TEX(node_8671, _Noise2));
                float2 node_5889 = (i.uv0+((_Noise1_var.r+_Noise2_var.r)*_Twist));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5889, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (saturate((sceneZ-partZ))*(_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0)*_Mask_var.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a*_Mask_var.a*(_Noise1_var.rgb*_Noise2_var.rgb).r));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
