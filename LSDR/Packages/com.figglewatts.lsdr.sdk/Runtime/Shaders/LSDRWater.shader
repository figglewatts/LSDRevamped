Shader "LSDR/Water" {
	Properties {
		_MainTex ("Water Map", 2D) = "white" {}
		_PaletteTexture ("Water Palette", 2D) = "white" {}
		_AnimationSpeed ("Speed", float) = 1.0
		_Alpha ("Transparency", float) = 0.5
    }
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        ZTest LEqual
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _PaletteTexture;
		float _AnimationSpeed;
		float _Alpha;
		
		struct Input {
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
		    float4 mapSample = tex2D(_MainTex, IN.uv_MainTex);
		    float paletteIdx = (mapSample.r - _Time[0] * _AnimationSpeed) % 1.0;
			o.Albedo = tex2D(_PaletteTexture, float2(paletteIdx, 0.5));
			o.Alpha = min(_Alpha, mapSample.a);
		}
		ENDCG
	}
	FallBack "Diffuse"
}