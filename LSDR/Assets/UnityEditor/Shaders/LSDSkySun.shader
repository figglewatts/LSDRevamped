Shader "LSD/Sky/Sun" {
	Properties{
		_MainTex("Sun Texture", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
		_Tint ("Tint", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
		Fog { Mode Off }
		Lighting On
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

	float4 _MainTex_ST;
	uniform float _Cutoff;

	v2f vert(appdata_full v)
	{
		v2f o;

		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);

		o.color = v.color;

		o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

		return o;
	}

	sampler2D _MainTex;

	fixed4 _Tint;

	float4 frag(v2f IN) : COLOR
	{
		half4 c = tex2D(_MainTex, IN.uv_MainTex)*IN.color*_Tint;
	if (c.a < _Cutoff)
	{
		discard;
	}
	return c;
	}
		ENDCG
	}
	}
}