Shader "LSD/Sky/Sunburst Opaque" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }

		Cull off

		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Color;

		struct Input {
			half dummy;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}