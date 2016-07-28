Shader "LSD/DiffuseSet" {
	Properties{
		_MainTexA("Albedo A (RGB)", 2D) = "white" {}
		_MainTexB("Albedo B (RGB)", 2D) = "white" {}
		_MainTexC("Albedo C (RGB)", 2D) = "white" {}
		_MainTexD("Albedo D (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
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
			};

			float4 _MainTexA_ST;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);

				o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
				o.color *= v.color;

				float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTexA);

				//Fog
				float4 fogColor = unity_FogColor;

				float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
				//o.normal.g = fogDensity;
				//o.normal.b = 1;

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
				if (_TextureSet == 2)
				{
					c = tex2D(_MainTexB, IN.uv_MainTex)*IN.color;
				}
				else if (_TextureSet == 3)
				{
					c = tex2D(_MainTexC, IN.uv_MainTex)*IN.color;
				}
				else if (_TextureSet == 4)
				{
					c = tex2D(_MainTexD, IN.uv_MainTex)*IN.color;
				}
				else // default to A
				{
					c = tex2D(_MainTexA, IN.uv_MainTex)*IN.color;
				}
				half4 color = c*(IN.colorFog.a);
				color.rgb += IN.colorFog.rgb*(1 - IN.colorFog.a);

				return color;
			}
			ENDCG
		}
	}
}