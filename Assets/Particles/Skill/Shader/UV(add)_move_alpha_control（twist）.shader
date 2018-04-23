// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|emission-1150-OUT;n:type:ShaderForge.SFN_Tex2d,id:7825,x:32299,y:32665,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-825-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7901,x:32559,y:32775,varname:node_7901,prsc:2|A-7825-RGB,B-347-RGB,C-8230-RGB,D-7481-OUT;n:type:ShaderForge.SFN_VertexColor,id:347,x:31728,y:32770,varname:node_347,prsc:2;n:type:ShaderForge.SFN_Color,id:8230,x:32287,y:32940,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:7481,x:32297,y:33126,varname:node_7481,prsc:2,v1:2;n:type:ShaderForge.SFN_Panner,id:825,x:32096,y:32609,varname:node_825,prsc:2,spu:0,spv:1|UVIN-3271-OUT,DIST-5682-OUT;n:type:ShaderForge.SFN_Clamp01,id:5727,x:31923,y:32931,varname:node_5727,prsc:2|IN-347-A;n:type:ShaderForge.SFN_RemapRange,id:5682,x:32092,y:32914,varname:node_5682,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-5727-OUT;n:type:ShaderForge.SFN_Multiply,id:8979,x:32776,y:32950,varname:node_8979,prsc:2|A-7901-OUT,B-154-A;n:type:ShaderForge.SFN_Tex2d,id:154,x:32537,y:33075,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_5890,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_DepthBlend,id:2060,x:32757,y:32670,varname:node_2060,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1150,x:32971,y:32801,varname:node_1150,prsc:2|A-2060-OUT,B-8979-OUT,C-347-A;n:type:ShaderForge.SFN_Tex2d,id:9393,x:31026,y:32451,ptovrint:False,ptlb:Noise1,ptin:_Noise1,varname:node_5105,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8394-OUT;n:type:ShaderForge.SFN_Tex2d,id:9302,x:31046,y:32702,ptovrint:False,ptlb:Noise2,ptin:_Noise2,varname:node_7759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7619-OUT;n:type:ShaderForge.SFN_Append,id:76,x:30438,y:32419,varname:node_76,prsc:2|A-5163-OUT,B-7364-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5163,x:30245,y:32419,ptovrint:False,ptlb:1 U speed,ptin:_1Uspeed,varname:node_1205,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7364,x:30245,y:32482,ptovrint:False,ptlb:1 V speed,ptin:_1Vspeed,varname:node_8234,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:456,x:30438,y:32581,varname:node_456,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7834,x:30643,y:32459,varname:node_7834,prsc:2|A-76-OUT,B-456-T;n:type:ShaderForge.SFN_TexCoord,id:5661,x:30667,y:32591,varname:node_5661,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:8394,x:30841,y:32459,varname:node_8394,prsc:2|A-7834-OUT,B-5661-UVOUT;n:type:ShaderForge.SFN_Append,id:6767,x:30453,y:32740,varname:node_6767,prsc:2|A-6966-OUT,B-8390-OUT;n:type:ShaderForge.SFN_Add,id:7619,x:30856,y:32702,varname:node_7619,prsc:2|A-5661-UVOUT,B-2536-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6966,x:30230,y:32708,ptovrint:False,ptlb:2 U speed,ptin:_2Uspeed,varname:node_4913,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8390,x:30230,y:32803,ptovrint:False,ptlb:2 V speed,ptin:_2Vspeed,varname:node_1874,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2536,x:30645,y:32740,varname:node_2536,prsc:2|A-456-T,B-6767-OUT;n:type:ShaderForge.SFN_Add,id:1748,x:31246,y:32585,varname:node_1748,prsc:2|A-9393-R,B-9302-R;n:type:ShaderForge.SFN_Multiply,id:4660,x:31408,y:32585,varname:node_4660,prsc:2|A-1748-OUT,B-9380-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9380,x:31240,y:32795,ptovrint:False,ptlb:Twist,ptin:_Twist,varname:node_305,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:7408,x:31408,y:32427,varname:node_7408,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3271,x:31562,y:32555,varname:node_3271,prsc:2|A-7408-UVOUT,B-4660-OUT;proporder:7825-8230-154-9393-9302-9380-5163-7364-6966-8390;pass:END;sub:END;*/

Shader "ME/UV(add)_move_alpha_controltwist" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "white" {}
        _Noise1 ("Noise1", 2D) = "white" {}
        _Noise2 ("Noise2", 2D) = "white" {}
        _Twist ("Twist", Float ) = 0
        _1Uspeed ("1 U speed", Float ) = 0
        _1Vspeed ("1 V speed", Float ) = 0
        _2Uspeed ("2 U speed", Float ) = 0
        _2Vspeed ("2 V speed", Float ) = 0
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
            Blend One One
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
                float4 node_456 = _Time + _TimeEditor;
                float2 node_8394 = ((float2(_1Uspeed,_1Vspeed)*node_456.g)+i.uv0);
                float4 _Noise1_var = tex2D(_Noise1,TRANSFORM_TEX(node_8394, _Noise1));
                float2 node_7619 = (i.uv0+(node_456.g*float2(_2Uspeed,_2Vspeed)));
                float4 _Noise2_var = tex2D(_Noise2,TRANSFORM_TEX(node_7619, _Noise2));
                float2 node_825 = ((i.uv0+((_Noise1_var.r+_Noise2_var.r)*_Twist))+(saturate(i.vertexColor.a)*2.0+-1.0)*float2(0,1));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_825, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (saturate((sceneZ-partZ))*((_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0)*_Mask_var.a)*i.vertexColor.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
