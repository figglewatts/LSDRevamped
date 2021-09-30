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
}