Shader "LSDR/ClassicDiffuse"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
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
            ZTest LEqual


            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
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

            sampler2D _MainTex;
            fixed4 _Tint;

            fragOut frag(g2f input)
            {
                return classicFragCutout(input, _MainTex, _Tint);
            }
            ENDCG
        }
    }
}