// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sky"
{
	Properties
	{
		_SkyColor1("Top Color", Color) = (0.37, 0.52, 0.73, 0)
		_SkyExponent1("Top Exponent", Float) = 8.5

		_SkyColor2("Horizon Color", Color) = (0.89, 0.96, 1, 0)

		_SkyColor3("Bottom Color", Color) = (0.89, 0.89, 0.89, 0)
		_SkyExponent2("Bottom Exponent", Float) = 3.0

		_SkyIntensity("Sky Intensity", Float) = 1.0

		_SunColor("Sun Color", Color) = (1, 0.99, 0.87, 1)
		_SunIntensity("Sun Intensity", float) = 2.0

		_SunAlpha("Sun Alpha", float) = 550
		_SunBeta("Sun Beta", float) = 1

		_SunVector("Sun Vector", Vector) = (0.269, 0.615, 0.740, 0)

		_SunAzimuth("Sun Azimuth (editor only)", float) = 20
		_SunAltitude("Sun Altitude (editor only)", float) = 38

		_SunTex("Sun Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc" 

			struct v2f
			{
				float4 position : SV_POSITION;
				float3 texcoord : TEXCOORD0;
				float2 suntexcoord : TEXCOORD1;
			};

			half3 _SkyColor1;
			half _SkyExponent1;

			half3 _SkyColor2;

			half3 _SkyColor3;
			half _SkyExponent2;

			half _SkyIntensity;

			half4 _SunColor;
			half _SunIntensity;

			half _SunAlpha;
			half _SunBeta;

			half3 _SunVector;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);

				o.texcoord = v.texcoord;

				o.suntexcoord = v.texcoord;
				return o;
			}

			sampler2D _SunTex;
			float4 _SunTex_ST;

			half4 frag(v2f i) : COLOR
			{
				float p = normalize(i.texcoord).y;
				float p1 = 1.0f - pow(min(1.0f, 1.0f - p), _SkyExponent1);
				float p3 = 1.0f - pow(min(1.0f, 1.0f + p), _SkyExponent2);
				float p2 = 1.0f - p1 - p3;

				half3 c_sky = _SkyColor1 * p1 + _SkyColor2 * p2 + _SkyColor3 * p3;

				//half3 c_sky = _SkyColor1 * p1 + _SkyColor2 * p2 + _SkyColor3 * p3;
				//half3 c_sun = tex2D(_SunTex, i.texcoord) * min(pow(max(0, dot(v, _SunVector)), _SunAlpha) * _SunBeta, 1);
				//c_sun = tex2D(_SunTex, i.texcoord) * max(0, dot(v, _SunVector));
				//c_sun = tex2D(_SunTex, float4(i.texcoord.xy * _SunTex_ST, 0, 0));

				//return half4(c_sky * _SkyIntensity + c_sun * _SunIntensity, 0);

				return half4(c_sky, 0);
			}

			ENDCG
		}
	}
	FallBack off
}