// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:True,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:5055,x:33582,y:32680,varname:node_5055,prsc:2|emission-6386-OUT,alpha-4222-OUT,clip-2904-B,refract-2991-OUT;n:type:ShaderForge.SFN_Tex2d,id:2904,x:32480,y:32603,ptovrint:False,ptlb:Main,ptin:_Main,varname:node_2904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False|UVIN-1388-UVOUT,MIP-6079-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6055,x:32741,y:32825,varname:node_6055,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2904-RGB;n:type:ShaderForge.SFN_Multiply,id:2991,x:33033,y:33008,varname:node_2991,prsc:2|A-6055-OUT,B-9358-OUT;n:type:ShaderForge.SFN_VertexColor,id:9858,x:32291,y:32927,varname:node_9858,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9358,x:32817,y:33084,varname:node_9358,prsc:2|A-9858-A,B-3725-OUT;n:type:ShaderForge.SFN_TexCoord,id:1388,x:32289,y:32481,varname:node_1388,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:6079,x:32051,y:32732,ptovrint:False,ptlb:tex,ptin:_tex,varname:node_6079,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.205128,max:6;n:type:ShaderForge.SFN_Lerp,id:6386,x:33061,y:32552,varname:node_6386,prsc:2|A-8462-OUT,B-6055-OUT,T-9858-A;n:type:ShaderForge.SFN_Vector3,id:8462,x:32751,y:32442,varname:node_8462,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:3725,x:32381,y:33195,ptovrint:False,ptlb:amount,ptin:_amount,varname:node_3725,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1025641,max:1;n:type:ShaderForge.SFN_Vector1,id:4222,x:33261,y:32813,varname:node_4222,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9020,x:32703,y:32603,varname:node_9020,prsc:2,v1:0;proporder:2904-6079-3725;pass:END;sub:END;*/

Shader "ME/Wraping" {
    Properties {
        _Main ("Main", 2D) = "bump" {}
        _tex ("tex", Range(0, 6)) = 2.205128
        _amount ("amount", Range(0, 1)) = 0.1025641
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            AlphaToMask On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Main; uniform float4 _Main_ST;
            uniform float _tex;
            uniform float _amount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 _Main_var = tex2Dlod(_Main,float4(TRANSFORM_TEX(i.uv0, _Main),0.0,_tex));
                float2 node_6055 = _Main_var.rgb.rg;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_6055*(i.vertexColor.a*_amount));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                clip(_Main_var.b - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = lerp(float3(0,0,1),float3(node_6055,0.0),i.vertexColor.a);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _Main; uniform float4 _Main_ST;
            uniform float _tex;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _Main_var = tex2Dlod(_Main,float4(TRANSFORM_TEX(i.uv0, _Main),0.0,_tex));
                clip(_Main_var.b - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
