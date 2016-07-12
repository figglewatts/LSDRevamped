Shader "psx/transparent/vertexlit" {
	Properties{
		_MainTexA("Albedo A (RGB)", 2D) = "white" {}
		_MainTexB("Albedo B (RGB)", 2D) = "white" {}
		_MainTexC("Albedo C (RGB)", 2D) = "white" {}
		_MainTexD("Albedo D (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
			Lighting On
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				half4 color : COLOR0;
				half4 colorFog : COLOR1;
				float2 uv_MainTex : TEXCOORD0;
				half3 normal: TEXCOORD1;
			};

			float4 _MainTexA_ST;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			v2f vert(appdata_full v)
			{
				v2f o;

				//Vertex snapping
				float4 snapToPixel = mul(UNITY_MATRIX_MVP,v.vertex);
				float4 vertex = snapToPixel;
				vertex.xyz = snapToPixel.xyz / snapToPixel.w;
				vertex.x = floor(160 * vertex.x) / 160;
				vertex.y = floor(120 * vertex.y) / 120;
				vertex.xyz *= snapToPixel.w;
				o.pos = vertex;


				//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//Vertex lighting 
				//o.color = float4(ShadeVertexLights(v.vertex, v.normal), 1.0);
				o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
				o.color *= v.color;

				float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

				//Affine Texture Mapping
				float4 affinePos = vertex;//vertex;				
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTexA);
				o.uv_MainTex *= distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
				o.normal = distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

				//Fog
				float4 fogColor = unity_FogColor;

				float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
				o.normal.g = fogDensity;
				o.normal.b = 1;

				o.colorFog = fogColor;
				o.colorFog.a = clamp(fogDensity,0,1);

				//Cut out polygons
				if (distance > unity_FogStart.z + unity_FogColor.a * 255)
				{
					o.pos.w = 0;
				}


				return o;
			}

			sampler2D _MainTexA;
			sampler2D _MainTexB;
			sampler2D _MainTexC;
			sampler2D _MainTexD;

			uniform int _TextureSet;

			float4 frag(v2f IN) : COLOR
			{
				half4 c;
				if (_TextureSet == 1)
				{
					c = tex2D(_MainTexA, IN.uv_MainTex / IN.normal.r)*IN.color;
				}
				else if (_TextureSet == 2)
				{
					c = tex2D(_MainTexB, IN.uv_MainTex / IN.normal.r)*IN.color;
				}
				else if (_TextureSet == 3)
				{
					c = tex2D(_MainTexC, IN.uv_MainTex / IN.normal.r)*IN.color;
				}
				else if (_TextureSet == 4)
				{
					c = tex2D(_MainTexD, IN.uv_MainTex / IN.normal.r)*IN.color;
				}
				else // default to A
				{
					c = tex2D(_MainTexA, IN.uv_MainTex / IN.normal.r)*IN.color;
				}
				half4 color = c*(IN.colorFog.a);
				color.rgb += IN.colorFog.rgb*(1 - IN.colorFog.a);
				color.a = c.a;
				return color;
			}
			ENDCG
		}
	}
}