Shader "LSD/TransparentSet" {
	Properties {
		_MainTexA ("Albedo A (RGB)", 2D) = "white" {}
		_MainTexB ("Albedo B (RGB)", 2D) = "white" {}
		_MainTexC ("Albedo C (RGB)", 2D) = "white" {}
		_MainTexD ("Albedo D (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTexA;
			float2 uv_MainTexB;
			float2 uv_MainTexC;
			float2 uv_MainTexD;
		};

		sampler2D _MainTexA;
		sampler2D _MainTexB;
		sampler2D _MainTexC;
		sampler2D _MainTexD;

		uniform int _TextureSet;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c;
			if (_TextureSet == 1)
			{
				c = tex2D (_MainTexB, IN.uv_MainTexB);
			}
			else if (_TextureSet == 2)
			{
				c = tex2D (_MainTexC, IN.uv_MainTexC);
			}
			else if (_TextureSet == 3)
			{
				c = tex2D (_MainTexD, IN.uv_MainTexD);
			}
			else
			{
				// default to texture set A
				c = tex2D (_MainTexA, IN.uv_MainTexA);
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/Cutout/Diffuse"
}
