// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-5832-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:31898,y:32551,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c1d86af22004f9f4990477ad53473b3c,ntxv:0,isnm:False|UVIN-9553-UVOUT;n:type:ShaderForge.SFN_Multiply,id:5085,x:32223,y:32577,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-851-RGB,B-851-A,C-4219-RGB;n:type:ShaderForge.SFN_TexCoord,id:8283,x:29701,y:32368,varname:node_8283,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:467,x:30059,y:32404,varname:node_467,prsc:2|UVIN-8283-UVOUT,ANG-3698-OUT;n:type:ShaderForge.SFN_Slider,id:9007,x:29136,y:32470,ptovrint:False,ptlb:Angle,ptin:_Angle,varname:node_9007,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:360;n:type:ShaderForge.SFN_Pi,id:7333,x:29501,y:32696,varname:node_7333,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3698,x:29679,y:32577,varname:node_3698,prsc:2|A-2232-OUT,B-7333-OUT;n:type:ShaderForge.SFN_RemapRange,id:2232,x:29489,y:32497,varname:node_2232,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-9007-OUT;n:type:ShaderForge.SFN_UVTile,id:9553,x:31385,y:32567,varname:node_9553,prsc:2|UVIN-8640-OUT,WDT-7273-X,HGT-7273-Y,TILE-7729-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9906,x:30303,y:32491,varname:node_9906,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-467-UVOUT;n:type:ShaderForge.SFN_Append,id:8640,x:30715,y:32530,varname:node_8640,prsc:2|A-9906-R,B-9624-OUT;n:type:ShaderForge.SFN_OneMinus,id:9624,x:30512,y:32660,varname:node_9624,prsc:2|IN-9906-G;n:type:ShaderForge.SFN_Slider,id:4853,x:29990,y:32889,ptovrint:False,ptlb:Uvtile,ptin:_Uvtile,varname:node_4853,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.487154,max:9;n:type:ShaderForge.SFN_Trunc,id:7729,x:30610,y:32962,varname:node_7729,prsc:2|IN-168-OUT;n:type:ShaderForge.SFN_Vector4Property,id:7273,x:30969,y:32656,ptovrint:False,ptlb:Uvcount,ptin:_Uvcount,varname:node_7273,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3,v2:3,v3:0,v4:0;n:type:ShaderForge.SFN_Time,id:1252,x:29894,y:33052,varname:node_1252,prsc:2;n:type:ShaderForge.SFN_Slider,id:7318,x:29692,y:33322,ptovrint:False,ptlb:AutoTileSpeed,ptin:_AutoTileSpeed,varname:_Uvtile_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:6494,x:30212,y:33161,varname:node_6494,prsc:2|A-1252-TDB,B-7318-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:168,x:30389,y:32856,ptovrint:False,ptlb:AutoTile,ptin:_AutoTile,varname:node_168,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-4853-OUT,B-6494-OUT;n:type:ShaderForge.SFN_Color,id:4219,x:31908,y:32781,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.1310346,c4:1;n:type:ShaderForge.SFN_FaceSign,id:2744,x:31815,y:32065,varname:node_2744,prsc:2,fstp:0;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:7000,x:32337,y:32164,varname:node_7000,prsc:2|IN-1444-OUT,IMIN-6202-OUT,IMAX-9143-OUT,OMIN-8269-OUT,OMAX-9143-OUT;n:type:ShaderForge.SFN_Vector1,id:6202,x:32131,y:32203,varname:node_6202,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9143,x:32130,y:32243,varname:node_9143,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8269,x:31959,y:32344,ptovrint:False,ptlb:BackFace,ptin:_BackFace,varname:node_8269,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_OneMinus,id:1444,x:32036,y:32073,varname:node_1444,prsc:2|IN-2744-VFACE;n:type:ShaderForge.SFN_Multiply,id:5832,x:32462,y:32288,varname:node_5832,prsc:2|A-7000-OUT,B-5085-OUT;proporder:851-168-7318-9007-4853-7273-4219-8269;pass:END;sub:END;*/

Shader "Shader Forge/test" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        [MaterialToggle] _AutoTile ("AutoTile", Float ) = 0
        _AutoTileSpeed ("AutoTileSpeed", Range(0, 10)) = 1
        _Angle ("Angle", Range(0, 360)) = 0.5
        _Uvtile ("Uvtile", Range(0, 9)) = 4.487154
        _Uvcount ("Uvcount", Vector) = (3,3,0,0)
        _Color ("Color", Color) = (0,1,0.1310346,1)
        _BackFace ("BackFace", Range(0, 1)) = 0
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Angle;
            uniform float _Uvtile;
            uniform float4 _Uvcount;
            uniform float _AutoTileSpeed;
            uniform fixed _AutoTile;
            uniform float4 _Color;
            uniform float _BackFace;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                float node_6202 = 0.0;
                float node_9143 = 1.0;
                float4 node_1252 = _Time + _TimeEditor;
                float node_7729 = trunc(lerp( _Uvtile, (node_1252.b*_AutoTileSpeed), _AutoTile ));
                float2 node_9553_tc_rcp = float2(1.0,1.0)/float2( _Uvcount.r, _Uvcount.g );
                float node_9553_ty = floor(node_7729 * node_9553_tc_rcp.x);
                float node_9553_tx = node_7729 - _Uvcount.r * node_9553_ty;
                float node_467_ang = ((_Angle*0.005555556+0.0)*3.141592654);
                float node_467_spd = 1.0;
                float node_467_cos = cos(node_467_spd*node_467_ang);
                float node_467_sin = sin(node_467_spd*node_467_ang);
                float2 node_467_piv = float2(0.5,0.5);
                float2 node_467 = (mul(i.uv0-node_467_piv,float2x2( node_467_cos, -node_467_sin, node_467_sin, node_467_cos))+node_467_piv);
                float2 node_9906 = node_467.rg;
                float2 node_9553 = (float2(node_9906.r,(1.0 - node_9906.g)) + float2(node_9553_tx, node_9553_ty)) * node_9553_tc_rcp;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_9553, _Diffuse));
                float3 node_5832 = ((_BackFace + ( ((1.0 - isFrontFace) - node_6202) * (node_9143 - _BackFace) ) / (node_9143 - node_6202))*(_Diffuse_var.rgb*_Diffuse_var.a*_Color.rgb));
                float3 finalColor = node_5832;
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
