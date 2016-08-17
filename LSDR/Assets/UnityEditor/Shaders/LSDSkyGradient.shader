Shader "LSD/Sky/Gradient" {
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_ColorTop("Top Color", Color) = (1,1,1,1)
		_ColorMid("Mid Color", Color) = (1,1,1,1)
		_ColorBot("Bot Color", Color) = (1,1,1,1)
		_Middle("Middle", Range(0.001, 0.999)) = 1
		_OffsetOuter("Outer Offset", Range(0, 1)) = 0.9
		_OffsetInner("Inner Offset", Range(0, 1)) = 0.1
		_GradientStep("Gradient Step", Range(0.001, 0.5)) = 0.1
	}

	SubShader{
		Tags{ "Queue" = "Background"  "IgnoreProjector" = "True" }
		LOD 100

		ZWrite On

		Pass{
			Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _ColorTop;
			fixed4 _ColorMid;
			fixed4 _ColorBot;
			float  _Middle;

			struct v2f {
				float4 pos : SV_POSITION;
				float4 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			float _GradientStep;
			float _OffsetOuter;
			float _OffsetInner;

			fixed4 frag(v2f i) : COLOR{
				//fixed4 c = lerp (_ColorBot, _ColorMid, _Offset + (i.texcoord.y / (_Middle - _Offset))) * step(i.texcoord.y, _Middle))

				float mb = (1 / (_Middle - _OffsetInner - _OffsetOuter));

				fixed4 c = lerp (_ColorBot, _ColorMid, (mb * i.texcoord.y) - mb * _OffsetOuter) * step(i.texcoord.y, _Middle);

				float mt = (1 / (1 - _Middle - _OffsetOuter - _OffsetInner));

				c += lerp (_ColorMid, _ColorTop, (mt * (i.texcoord.y - _Middle))) * step(_Middle, i.texcoord.y);

				//fixed4 c = lerp(_ColorBot, _ColorMid, i.texcoord.y / (_Middle - _Offset)) * step(i.texcoord.y, _Middle - _Offset);
				//c += lerp(_ColorMid, _ColorTop, (i.texcoord.y - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.y);
				c = round(c / _GradientStep) * _GradientStep;
				c.a = 1;
				return c;
			}
			ENDCG
		}
	}
}