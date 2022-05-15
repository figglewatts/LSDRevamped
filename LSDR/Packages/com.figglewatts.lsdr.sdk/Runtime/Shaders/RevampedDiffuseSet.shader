Shader "LSDR/RevampedDiffuseSet"
{
    Properties
    {
        _MainTexA ("Albedo A (RGB)", 2D) = "white" {}
        _MainTexB ("Albedo B (RGB)", 2D) = "white" {}
        _MainTexC ("Albedo C (RGB)", 2D) = "white" {}
        _MainTexD ("Albedo D (RGB)", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma exclude_renderers metal
            
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #define LSDR_TEXTURE_SET
            #define LSDR_CUTOUT_ALPHA
            #include "LSDR.cginc"
            ENDCG
        }
    }
    
    // subshader without geometry shader, for mac
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma only_renderers metal
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #define LSDR_TEXTURE_SET
            #define LSDR_CUTOUT_ALPHA
            #define LSDR_NO_GEOM
            #include "LSDR.cginc"
            ENDCG
        }
    }
}