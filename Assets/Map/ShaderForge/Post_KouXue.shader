// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:1,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32740,y:33254,varname:node_2865,prsc:2|emission-3053-OUT,alpha-3663-OUT,voffset-1053-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:31885,y:33882,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:1053,x:32235,y:33879,varname:node_1053,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-4219-UVOUT;n:type:ShaderForge.SFN_Color,id:6537,x:31844,y:32660,ptovrint:False,ptlb:node_6537,ptin:_node_6537,varname:node_6537,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6681915,c2:0.7794118,c3:0.6590614,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3387,x:31827,y:32919,varname:node_3387,prsc:2,tex:d3f9d9ff6f0f77d4692e00d7e4e534ce,ntxv:0,isnm:False|UVIN-352-UVOUT,TEX-6103-TEX;n:type:ShaderForge.SFN_Multiply,id:3053,x:32375,y:33120,varname:node_3053,prsc:2|A-6537-RGB,B-3387-RGB,C-2597-OUT;n:type:ShaderForge.SFN_TexCoord,id:1330,x:30456,y:32494,varname:node_1330,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:2574,x:30677,y:32494,varname:node_2574,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-1330-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:8546,x:30869,y:32502,varname:node_8546,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2574-OUT;n:type:ShaderForge.SFN_ArcTan2,id:8798,x:31071,y:32496,varname:node_8798,prsc:2,attp:3|A-8546-G,B-8546-R;n:type:ShaderForge.SFN_Append,id:55,x:31288,y:32538,varname:node_55,prsc:2|A-8798-OUT,B-8798-OUT;n:type:ShaderForge.SFN_Rotator,id:352,x:31534,y:32627,varname:node_352,prsc:2|UVIN-55-OUT;n:type:ShaderForge.SFN_TexCoord,id:1500,x:31356,y:33472,varname:node_1500,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector2,id:7139,x:31274,y:33649,varname:node_7139,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Distance,id:2024,x:31633,y:33518,varname:node_2024,prsc:2|A-1500-UVOUT,B-7139-OUT;n:type:ShaderForge.SFN_Power,id:6354,x:32145,y:33395,varname:node_6354,prsc:2|VAL-759-OUT,EXP-2793-OUT;n:type:ShaderForge.SFN_Exp,id:2793,x:32150,y:33544,varname:node_2793,prsc:2,et:0|IN-1697-OUT;n:type:ShaderForge.SFN_Slider,id:1697,x:31955,y:33765,ptovrint:False,ptlb:node_1697,ptin:_node_1697,varname:node_1697,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.411613,max:8;n:type:ShaderForge.SFN_RemapRange,id:759,x:31890,y:33510,varname:node_759,prsc:2,frmn:0,frmx:0.5,tomn:0,tomx:0.8|IN-2024-OUT;n:type:ShaderForge.SFN_Slider,id:1324,x:32304,y:33629,ptovrint:False,ptlb:node_1324,ptin:_node_1324,varname:node_1324,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:3663,x:32391,y:33423,varname:node_3663,prsc:2|A-6354-OUT,B-1324-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6103,x:31441,y:32954,ptovrint:False,ptlb:node_6103,ptin:_node_6103,varname:node_6103,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d3f9d9ff6f0f77d4692e00d7e4e534ce,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1179,x:31809,y:33087,varname:node_1179,prsc:2,tex:d3f9d9ff6f0f77d4692e00d7e4e534ce,ntxv:0,isnm:False|UVIN-6244-UVOUT,TEX-6103-TEX;n:type:ShaderForge.SFN_Rotator,id:6244,x:31520,y:32769,varname:node_6244,prsc:2|UVIN-55-OUT,SPD-8362-OUT;n:type:ShaderForge.SFN_Vector1,id:8362,x:31315,y:32846,varname:node_8362,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Color,id:8747,x:31792,y:33282,ptovrint:False,ptlb:node_6537_copy,ptin:_node_6537_copy,varname:_node_6537_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.1448276,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:2597,x:32105,y:33163,varname:node_2597,prsc:2|A-1179-RGB,B-8747-RGB;proporder:6537-1697-1324-6103-8747;pass:END;sub:END;*/

Shader "Shader Forge/Post" {
    Properties {
        _node_6537 ("node_6537", Color) = (0.6681915,0.7794118,0.6590614,1)
        _node_1697 ("node_1697", Range(0, 8)) = 1.411613
        _node_1324 ("node_1324", Range(0, 1)) = 0
        _node_6103 ("node_6103", 2D) = "white" {}
        _node_6537_copy ("node_6537_copy", Color) = (1,0.1448276,0,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _node_6537;
            uniform float _node_1697;
            uniform float _node_1324;
            uniform sampler2D _node_6103; uniform float4 _node_6103_ST;
            uniform float4 _node_6537_copy;
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
                v.vertex.xyz = float3((o.uv0*2.0+-1.0),0.0);
                o.pos = v.vertex;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_7969 = _Time + _TimeEditor;
                float node_352_ang = node_7969.g;
                float node_352_spd = 1.0;
                float node_352_cos = cos(node_352_spd*node_352_ang);
                float node_352_sin = sin(node_352_spd*node_352_ang);
                float2 node_352_piv = float2(0.5,0.5);
                float2 node_8546 = (i.uv0*2.0+-1.0).rg;
                float node_8798 = (1-abs(atan2(node_8546.g,node_8546.r)/3.14159265359));
                float2 node_55 = float2(node_8798,node_8798);
                float2 node_352 = (mul(node_55-node_352_piv,float2x2( node_352_cos, -node_352_sin, node_352_sin, node_352_cos))+node_352_piv);
                float4 node_3387 = tex2D(_node_6103,TRANSFORM_TEX(node_352, _node_6103));
                float node_6244_ang = node_7969.g;
                float node_6244_spd = 0.5;
                float node_6244_cos = cos(node_6244_spd*node_6244_ang);
                float node_6244_sin = sin(node_6244_spd*node_6244_ang);
                float2 node_6244_piv = float2(0.5,0.5);
                float2 node_6244 = (mul(node_55-node_6244_piv,float2x2( node_6244_cos, -node_6244_sin, node_6244_sin, node_6244_cos))+node_6244_piv);
                float4 node_1179 = tex2D(_node_6103,TRANSFORM_TEX(node_6244, _node_6103));
                float3 emissive = (_node_6537.rgb*node_3387.rgb*(node_1179.rgb*_node_6537_copy.rgb));
                float3 finalColor = emissive;
                float2 node_7139 = float2(0.5,0.5);
                float node_6354 = pow((distance(i.uv0,node_7139)*1.6+0.0),exp(_node_1697));
                return fixed4(finalColor,(node_6354*_node_1324));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
