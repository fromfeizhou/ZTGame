// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-594-OUT,alpha-1133-A;n:type:ShaderForge.SFN_Tex2d,id:1133,x:32538,y:32854,ptovrint:False,ptlb:MaimTex,ptin:_MaimTex,varname:node_1133,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2499e5981d55d8245858d57c7d9e481e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:3628,x:29138,y:32849,varname:node_3628,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:63,x:29617,y:32873,varname:node_63,prsc:2,frmn:0,frmx:1,tomn:0,tomx:3.14|IN-1092-OUT;n:type:ShaderForge.SFN_Power,id:4789,x:31878,y:32967,varname:node_4789,prsc:2|VAL-2993-OUT,EXP-6070-OUT;n:type:ShaderForge.SFN_Slider,id:3148,x:30614,y:33262,ptovrint:False,ptlb:Width,ptin:_Width,varname:node_3148,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:12;n:type:ShaderForge.SFN_Exp,id:6070,x:31589,y:33172,varname:node_6070,prsc:2,et:0|IN-3056-OUT;n:type:ShaderForge.SFN_RemapRange,id:3056,x:31282,y:33157,varname:node_3056,prsc:2,frmn:0,frmx:10,tomn:10,tomx:1|IN-3148-OUT;n:type:ShaderForge.SFN_Color,id:4024,x:32344,y:33220,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4024,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Vector1,id:5537,x:32346,y:33402,varname:node_5537,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:7267,x:32608,y:33122,varname:node_7267,prsc:2|A-9433-OUT,B-4024-RGB,C-5537-OUT,D-4024-A;n:type:ShaderForge.SFN_Add,id:594,x:32944,y:32854,varname:node_594,prsc:2|A-1133-RGB,B-7267-OUT;n:type:ShaderForge.SFN_Slider,id:3333,x:29704,y:33108,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_3333,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:3.2,cur:0.6337174,max:-3.2;n:type:ShaderForge.SFN_Add,id:8716,x:29875,y:32867,varname:node_8716,prsc:2|A-63-OUT,B-3333-OUT;n:type:ShaderForge.SFN_Clamp,id:9433,x:32180,y:33017,varname:node_9433,prsc:2|IN-4789-OUT,MIN-7221-OUT,MAX-9850-OUT;n:type:ShaderForge.SFN_Vector1,id:7221,x:31956,y:33164,varname:node_7221,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Vector1,id:9850,x:31997,y:33316,varname:node_9850,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:1092,x:29425,y:32873,varname:node_1092,prsc:2|A-3628-U,B-3628-V,T-6748-OUT;n:type:ShaderForge.SFN_Slider,id:9479,x:28843,y:33043,ptovrint:False,ptlb:Angle,ptin:_Angle,varname:_Offset_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:90;n:type:ShaderForge.SFN_RemapRange,id:6748,x:29244,y:33068,varname:node_6748,prsc:2,frmn:0,frmx:90,tomn:0,tomx:1|IN-9479-OUT;n:type:ShaderForge.SFN_Tan,id:7084,x:30787,y:32572,varname:node_7084,prsc:2|IN-8716-OUT;n:type:ShaderForge.SFN_Multiply,id:2993,x:31260,y:32628,varname:node_2993,prsc:2|A-7084-OUT,B-7045-OUT,C-298-OUT;n:type:ShaderForge.SFN_Tan,id:7045,x:30793,y:32710,varname:node_7045,prsc:2|IN-8716-OUT;n:type:ShaderForge.SFN_Slider,id:298,x:30651,y:32961,ptovrint:False,ptlb:Size,ptin:_Size,varname:node_298,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.01;proporder:1133-3148-4024-3333-9479-298;pass:END;sub:END;*/

Shader "Shader Forge/UISaoGuang" {
    Properties {
        _MaimTex ("MaimTex", 2D) = "white" {}
        _Width ("Width", Range(1, 12)) = 1
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _Offset ("Offset", Range(3.2, -3.2)) = 0.6337174
        _Angle ("Angle", Range(0, 90)) = 0
        _Size ("Size", Range(0, 0.01)) = 0
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform sampler2D _MaimTex; uniform float4 _MaimTex_ST;
            uniform float _Width;
            uniform float4 _Color;
            uniform float _Offset;
            uniform float _Angle;
            uniform float _Size;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 _MaimTex_var = tex2D(_MaimTex,TRANSFORM_TEX(i.uv0, _MaimTex));
                float node_63 = (lerp(i.uv0.r,i.uv0.g,(_Angle*0.01111111+0.0))*3.14+0.0);
                float node_8716 = (node_63+_Offset);
                float node_2993 = (tan(node_8716)*tan(node_8716)*_Size);
                float3 finalColor = (_MaimTex_var.rgb+(clamp(pow(node_2993,exp((_Width*-0.9+10.0))),0.01,1.0)*_Color.rgb*5.0*_Color.a));
                fixed4 finalRGBA = fixed4(finalColor,_MaimTex_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
