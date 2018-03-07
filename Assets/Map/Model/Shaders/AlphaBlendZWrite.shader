// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/AlphaBlendZWrite"
{
	Properties {
		_Color ("Main Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
	}
	SubShader {
		//RenderType标签通常被用于着色器替换功能。
		Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
		LOD 200
		
		Pass {
			//这个新添加的Pass的目的仅仅是为了把模型的深度信息写入深度缓冲中，
			//从而剔除模型中被自身遮挡的片元。
		
			//开启深度缓冲写入
			ZWrite On
			//关闭颜色缓冲写入
			ColorMask 0
		}
		
		Pass {
			Tags {"LightMode"="ForwardBase"}
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			//用于在透明纹理的基础上控制整体的透明度。
			fixed _AlphaScale;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				//Unity会将模型的第一组纹理坐标存储到该变量中
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				//Unity5中可用UnityObjectToWorldNormal()函数得到o.worldNormal
				//o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.texcoord.xy*_MainTex_ST.xy+_MainTex_ST.zw;
				//或者调用Unity内建的函数计算o.uv
				//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR {
				fixed3 worldNormal = normalize(i.worldNormal);
				//fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);//只适合场景中仅有一个平行光
				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
				//只有使用Blend命令打开混合后，我们在这里设置透明通道才有意义，
				//否则，这些透明度并不会对片元的透明效果有任何影响。
				//透明通道: 纹理像素的透明通道和材质参数_AlphaScale的乘积。
				return fixed4(ambient + diffuse, texColor.a * _AlphaScale);
			}
			ENDCG
		}
	} 
	//确保我们编写的SubShader无法在当前显卡上工作时可以有合适的代替Shader,
	//还可以保证使用透明度测试的物体可以正确地向其他物体投射阴影
	FallBack "Transparent/Cutout/VertexLit"
}
