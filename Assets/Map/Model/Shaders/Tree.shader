// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5315204,fgcg:0.7193959,fgcb:0.8308824,fgca:1,fgde:0.03,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:5053,x:32857,y:32704,varname:node_5053,prsc:2|diff-1248-OUT,spec-8057-OUT,normal-3796-RGB,transm-4452-RGB,lwrap-6790-RGB,clip-2858-OUT,voffset-959-OUT;n:type:ShaderForge.SFN_Tex2d,id:2956,x:32112,y:32397,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_2956,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:86c328402008318459d7f79694df1eba,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3796,x:32238,y:32865,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_3796,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:07dddf1a41c32cc439410467e9fe5f19,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:1248,x:32565,y:32567,varname:node_1248,prsc:2|A-32-OUT,B-1505-RGB;n:type:ShaderForge.SFN_Color,id:1505,x:32112,y:32635,ptovrint:False,ptlb:Diffuse color,ptin:_Diffusecolor,varname:node_1505,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7270221,c2:0.8308824,c3:0.7499826,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:8057,x:32325,y:32733,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_8057,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Sin,id:8140,x:31716,y:33061,varname:node_8140,prsc:2|IN-9353-OUT;n:type:ShaderForge.SFN_Multiply,id:959,x:32459,y:33148,varname:node_959,prsc:2|A-9663-OUT,B-4787-RGB,C-9909-OUT;n:type:ShaderForge.SFN_Time,id:5457,x:31299,y:32996,varname:node_5457,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:4787,x:31739,y:33427,ptovrint:False,ptlb:WindMask,ptin:_WindMask,varname:node_4787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2c88edb777a39c442b0085c08ed553dd,ntxv:3,isnm:False|UVIN-393-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:393,x:31453,y:33482,varname:node_393,prsc:2,uv:2;n:type:ShaderForge.SFN_Add,id:9353,x:31528,y:33061,varname:node_9353,prsc:2|A-4545-OUT,B-5457-T;n:type:ShaderForge.SFN_Pi,id:4545,x:31332,y:33119,varname:node_4545,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:2004,x:31518,y:32868,prsc:2,pt:False;n:type:ShaderForge.SFN_ValueProperty,id:8610,x:31683,y:33228,ptovrint:False,ptlb:Main Wind Str,ptin:_MainWindStr,varname:node_8610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_Color,id:6790,x:32565,y:32733,ptovrint:False,ptlb:Light Wrap,ptin:_LightWrap,varname:_Diffusecolor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7270221,c2:0.8308824,c3:0.7499826,c4:1;n:type:ShaderForge.SFN_Panner,id:7043,x:30620,y:32551,varname:node_7043,prsc:2,spu:0.1,spv:0;n:type:ShaderForge.SFN_ValueProperty,id:6293,x:30927,y:32806,ptovrint:False,ptlb:Additional wind str,ptin:_Additionalwindstr,varname:_BulgeShape,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:246,x:31896,y:33061,varname:node_246,prsc:2|A-8140-OUT,B-8610-OUT;n:type:ShaderForge.SFN_Vector1,id:9909,x:32063,y:33554,varname:node_9909,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:9862,x:31900,y:32810,varname:node_9862,prsc:2|A-7767-OUT,B-2004-OUT;n:type:ShaderForge.SFN_Add,id:9663,x:32259,y:33053,varname:node_9663,prsc:2|A-9862-OUT,B-6303-OUT;n:type:ShaderForge.SFN_Tex2d,id:7752,x:30831,y:32551,ptovrint:False,ptlb:Additional wind Gradient,ptin:_AdditionalwindGradient,varname:node_7752,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:74a8e43e9fe997c4a83c89a1b8f2301d,ntxv:0,isnm:False|UVIN-7043-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7767,x:31190,y:32752,varname:node_7767,prsc:2|A-7752-RGB,B-6293-OUT;n:type:ShaderForge.SFN_Multiply,id:6303,x:32079,y:33081,varname:node_6303,prsc:2|A-246-OUT,B-5857-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:5857,x:31855,y:33274,ptovrint:False,ptlb:Main Wind vector,ptin:_MainWindvector,varname:node_5857,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Desaturate,id:32,x:32405,y:32476,varname:node_32,prsc:2|COL-2956-RGB,DES-589-OUT;n:type:ShaderForge.SFN_ValueProperty,id:589,x:32232,y:32558,ptovrint:False,ptlb:Desaturation,ptin:_Desaturation,varname:_Specular_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8658,x:32453,y:32861,ptovrint:False,ptlb:Specular_copy,ptin:_Specular_copy,varname:_Specular_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2858,x:32684,y:32951,varname:node_2858,prsc:2|A-2956-A,B-2714-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2714,x:32498,y:33015,ptovrint:False,ptlb:Alpha Cutoff,ptin:_AlphaCutoff,varname:_Specular_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:4452,x:32736,y:32427,ptovrint:False,ptlb:Transmission,ptin:_Transmission,varname:node_4452,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:2956-2714-3796-589-1505-6790-8057-4787-8610-5857-7752-6293-4452;pass:END;sub:END;*/

Shader "Custom/FloraBase" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "black" {}
        _AlphaCutoff ("Alpha Cutoff", Float ) = 1
        _Normal ("Normal", 2D) = "bump" {}
        _Desaturation ("Desaturation", Float ) = 0
        _Diffusecolor ("Diffuse color", Color) = (0.7270221,0.8308824,0.7499826,1)
        _LightWrap ("Light Wrap", Color) = (0.7270221,0.8308824,0.7499826,1)
        _Specular ("Specular", Float ) = 0
        _WindMask ("WindMask", 2D) = "bump" {}
        _MainWindStr ("Main Wind Str", Float ) = 0.3
        _MainWindvector ("Main Wind vector", Vector) = (0,0,0,0)
        _AdditionalwindGradient ("Additional wind Gradient", 2D) = "white" {}
        _Additionalwindstr ("Additional wind str", Float ) = 0.01
        _Transmission ("Transmission", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float4 _Diffusecolor;
            uniform float _Specular;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float4 _LightWrap;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _Desaturation;
            uniform float _AlphaCutoff;
            uniform float4 _Transmission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_4726 = _Time + _TimeEditor;
                float2 node_7043 = (o.uv0+node_4726.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7043, _AdditionalwindGradient),0.0,0));
                float4 node_5457 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_5457.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip((_Diffuse_var.a*_AlphaCutoff) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrap.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _Transmission.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = (lerp(_Diffuse_var.rgb,dot(_Diffuse_var.rgb,float3(0.3,0.59,0.11)),_Desaturation)*_Diffusecolor.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float4 _Diffusecolor;
            uniform float _Specular;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float4 _LightWrap;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _Desaturation;
            uniform float _AlphaCutoff;
            uniform float4 _Transmission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_1680 = _Time + _TimeEditor;
                float2 node_7043 = (o.uv0+node_1680.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7043, _AdditionalwindGradient),0.0,0));
                float4 node_5457 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_5457.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip((_Diffuse_var.a*_AlphaCutoff) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrap.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _Transmission.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 diffuseColor = (lerp(_Diffuse_var.rgb,dot(_Diffuse_var.rgb,float3(0.3,0.59,0.11)),_Desaturation)*_Diffusecolor.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
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
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _AlphaCutoff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_766 = _Time + _TimeEditor;
                float2 node_7043 = (o.uv0+node_766.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7043, _AdditionalwindGradient),0.0,0));
                float4 node_5457 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_5457.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip((_Diffuse_var.a*_AlphaCutoff) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
