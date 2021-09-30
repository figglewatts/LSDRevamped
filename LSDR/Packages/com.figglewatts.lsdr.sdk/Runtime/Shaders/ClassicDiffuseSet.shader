Shader "LSDR/ClassicDiffuseSet"
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
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "LSDR.cginc"

            v2g vert(appdata v)
            {
                return classicVert(v);
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                classicGeom(IN, triStream);
            }

            sampler2D _MainTexA;
            sampler2D _MainTexB;
            sampler2D _MainTexC;
            sampler2D _MainTexD;
            fixed4 _Tint;

            fragOut frag(g2f input)
            {
                return classicFragSetCutout(input, _MainTexA, _MainTexB, _MainTexC, _MainTexD, _Tint);
            }
            ENDCG
        }
    }
}