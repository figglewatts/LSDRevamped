Shader "LSDR/ClassicWater" {
	Properties {
		_MainTextureA ("Water Map A", 2D) = "white" {}
        _MainTextureB ("Water Map B", 2D) = "white" {}
        _MainTextureC ("Water Map C", 2D) = "white" {}
        _MainTextureD ("Water Map D", 2D) = "white" {}
	    _WaterPaletteA ("Water Palette A", 2D) = "white" {}
	    _WaterPaletteB ("Water Palette B", 2D) = "white" {}
	    _WaterPaletteC ("Water Palette C", 2D) = "white" {}
	    _WaterPaletteD ("Water Palette D", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
        _AnimationSpeed ("Speed", float) = 1.0
		_Alpha ("Transparency", float) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        ZTest LEqual
		LOD 200

	    Pass {
	        Cull [_Cull]
	    	
	    	CGPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

	        #define LSDR_WATER
            #define LSDR_CLASSIC
            #include "LSDR.cginc"
	        ENDCG
	    }
    }
	FallBack "Diffuse"
}