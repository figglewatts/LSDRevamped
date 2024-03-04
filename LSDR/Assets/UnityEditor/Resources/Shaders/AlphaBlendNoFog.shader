// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LSD/Sky/AlphaBlend" {
	Properties{
		_MainTex("Sun Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
	}
	SubShader{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		// draw after all opaque geometry has been drawn

		Pass{
			Fog { Mode Off }

			Cull Back // second pass renders only front faces 
						// (the "outside")
			ZWrite Off // don't write to depth buffer 
						// in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc" 

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				half4 color : COLOR0;
				float2 uv_MainTex : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _Tint;

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;

				return o;
			}

			float4 frag(v2f IN) : COLOR
			{
				float4 c = IN.color * tex2D(_MainTex, IN.uv_MainTex);
				c.rgb *= IN.color.a;
				c.rgb *= _Tint.rgb;
				return c;
			}

			ENDCG
		}
	}
}