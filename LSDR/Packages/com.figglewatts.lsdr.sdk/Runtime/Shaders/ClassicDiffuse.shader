Shader "LSDR/ClassicDiffuse"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass
        {
            ZTest LEqual
            Cull [_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #define LSDR_CLASSIC
            #define LSDR_CUTOUT_ALPHA
            #include "LSDR.cginc"
            ENDCG
        }
    }
}