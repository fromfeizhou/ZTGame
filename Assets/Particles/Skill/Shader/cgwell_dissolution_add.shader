// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33066,y:32639,varname:node_3138,prsc:2|emission-6151-OUT,custl-117-OUT,clip-6644-OUT;n:type:ShaderForge.SFN_Tex2d,id:3755,x:30864,y:32999,ptovrint:False,ptlb:Mask Texture,ptin:_MaskTexture,varname:node_3755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_If,id:5746,x:31505,y:32951,varname:node_5746,prsc:2|A-654-OUT,B-3755-R,GT-6272-OUT,EQ-6272-OUT,LT-3073-OUT;n:type:ShaderForge.SFN_Vector1,id:6272,x:31244,y:33096,varname:node_6272,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:3073,x:31244,y:33221,varname:node_3073,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:5620,x:31791,y:32531,ptovrint:False,ptlb:Diffuse Texture,ptin:_DiffuseTexture,varname:_node_3755_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:6731,x:32178,y:32826,varname:node_6731,prsc:2|A-5620-A,B-6450-OUT;n:type:ShaderForge.SFN_Color,id:7519,x:31787,y:32262,ptovrint:False,ptlb:Diffuse Color,ptin:_DiffuseColor,varname:node_7519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6985294,c2:0.6985294,c3:0.6985294,c4:1;n:type:ShaderForge.SFN_Multiply,id:3808,x:32215,y:32419,varname:node_3808,prsc:2|A-7519-RGB,B-5620-RGB;n:type:ShaderForge.SFN_Multiply,id:6644,x:32804,y:32912,varname:node_6644,prsc:2|A-7519-A,B-6731-OUT;n:type:ShaderForge.SFN_ValueProperty,id:329,x:30993,y:32887,ptovrint:False,ptlb:N_mask,ptin:_N_mask,varname:node_329,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_If,id:5217,x:31525,y:33205,varname:node_5217,prsc:2|A-654-OUT,B-2022-OUT,GT-6272-OUT,EQ-6272-OUT,LT-3073-OUT;n:type:ShaderForge.SFN_Add,id:2022,x:31106,y:33159,varname:node_2022,prsc:2|A-3755-R,B-5828-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5828,x:30875,y:33257,ptovrint:False,ptlb:N_BY_KD,ptin:_N_BY_KD,varname:node_5828,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Subtract,id:1274,x:31735,y:33102,varname:node_1274,prsc:2|A-5746-OUT,B-5217-OUT;n:type:ShaderForge.SFN_Color,id:9508,x:31735,y:33271,ptovrint:False,ptlb:C_BYcolor,ptin:_C_BYcolor,varname:node_9508,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7346,x:31929,y:33178,varname:node_7346,prsc:2|A-1274-OUT,B-9508-RGB;n:type:ShaderForge.SFN_ValueProperty,id:8447,x:31735,y:33450,ptovrint:False,ptlb:N_BY_QD,ptin:_N_BY_QD,varname:node_8447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:7666,x:32284,y:33152,varname:node_7666,prsc:2|A-7346-OUT,B-8447-OUT;n:type:ShaderForge.SFN_Add,id:6450,x:31917,y:33009,varname:node_6450,prsc:2|A-5746-OUT,B-1274-OUT;n:type:ShaderForge.SFN_Multiply,id:4059,x:32439,y:32519,varname:node_4059,prsc:2|A-3808-OUT,B-6450-OUT;n:type:ShaderForge.SFN_Add,id:1014,x:32619,y:32663,varname:node_1014,prsc:2|A-4059-OUT,B-7666-OUT;n:type:ShaderForge.SFN_VertexColor,id:8945,x:30993,y:32699,varname:node_8945,prsc:2;n:type:ShaderForge.SFN_Multiply,id:654,x:31277,y:32890,varname:node_654,prsc:2|A-8945-A,B-329-OUT;n:type:ShaderForge.SFN_Multiply,id:6151,x:32806,y:32593,varname:node_6151,prsc:2|A-7519-A,B-1014-OUT,C-8945-RGB;n:type:ShaderForge.SFN_Multiply,id:117,x:32818,y:32780,varname:node_117,prsc:2|A-7519-A,B-7666-OUT;proporder:7519-5620-329-3755-9508-8447-5828;pass:END;sub:END;*/

Shader "cgwell/Dissolution_Add" {
    Properties {
        _DiffuseColor ("Diffuse Color", Color) = (0.6985294,0.6985294,0.6985294,1)
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
        _N_mask ("N_mask", Float ) = 0.3
        _MaskTexture ("Mask Texture", 2D) = "white" {}
        _C_BYcolor ("C_BYcolor", Color) = (0.5,0.5,0.5,1)
        _N_BY_QD ("N_BY_QD", Float ) = 3
        _N_BY_KD ("N_BY_KD", Float ) = 0.01
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MaskTexture; uniform float4 _MaskTexture_ST;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _DiffuseColor;
            uniform float _N_mask;
            uniform float _N_BY_KD;
            uniform float4 _C_BYcolor;
            uniform float _N_BY_QD;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float node_654 = (i.vertexColor.a*_N_mask);
                float4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv0, _MaskTexture));
                float node_5746_if_leA = step(node_654,_MaskTexture_var.r);
                float node_5746_if_leB = step(_MaskTexture_var.r,node_654);
                float node_3073 = 0.0;
                float node_6272 = 1.0;
                float node_5746 = lerp((node_5746_if_leA*node_3073)+(node_5746_if_leB*node_6272),node_6272,node_5746_if_leA*node_5746_if_leB);
                float node_5217_if_leA = step(node_654,(_MaskTexture_var.r+_N_BY_KD));
                float node_5217_if_leB = step((_MaskTexture_var.r+_N_BY_KD),node_654);
                float node_1274 = (node_5746-lerp((node_5217_if_leA*node_3073)+(node_5217_if_leB*node_6272),node_6272,node_5217_if_leA*node_5217_if_leB));
                float node_6450 = (node_5746+node_1274);
                clip((_DiffuseColor.a*(_DiffuseTexture_var.a*node_6450)) - 0.5);
////// Lighting:
////// Emissive:
                float3 node_7666 = ((node_1274*_C_BYcolor.rgb)*_N_BY_QD);
                float3 emissive = (_DiffuseColor.a*(((_DiffuseColor.rgb*_DiffuseTexture_var.rgb)*node_6450)+node_7666)*i.vertexColor.rgb);
                float3 finalColor = emissive + (_DiffuseColor.a*node_7666);
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
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MaskTexture; uniform float4 _MaskTexture_ST;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _DiffuseColor;
            uniform float _N_mask;
            uniform float _N_BY_KD;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float node_654 = (i.vertexColor.a*_N_mask);
                float4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv0, _MaskTexture));
                float node_5746_if_leA = step(node_654,_MaskTexture_var.r);
                float node_5746_if_leB = step(_MaskTexture_var.r,node_654);
                float node_3073 = 0.0;
                float node_6272 = 1.0;
                float node_5746 = lerp((node_5746_if_leA*node_3073)+(node_5746_if_leB*node_6272),node_6272,node_5746_if_leA*node_5746_if_leB);
                float node_5217_if_leA = step(node_654,(_MaskTexture_var.r+_N_BY_KD));
                float node_5217_if_leB = step((_MaskTexture_var.r+_N_BY_KD),node_654);
                float node_1274 = (node_5746-lerp((node_5217_if_leA*node_3073)+(node_5217_if_leB*node_6272),node_6272,node_5217_if_leA*node_5217_if_leB));
                float node_6450 = (node_5746+node_1274);
                clip((_DiffuseColor.a*(_DiffuseTexture_var.a*node_6450)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
