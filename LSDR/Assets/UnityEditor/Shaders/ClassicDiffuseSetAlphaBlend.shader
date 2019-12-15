Shader "LSDR/ClassicDiffuseSetAlphaBlend" {
    Properties {
        _MainTexA ("Albedo A (RGB)", 2D) = "white" {}
        _MainTexB ("Albedo B (RGB)", 2D) = "white" {}
        _MainTexC ("Albedo C (RGB)", 2D) = "white" {}
        _MainTexD ("Albedo D (RGB)", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "LSDR.cginc"
            
            v2f vert(appdata v)
            {
                return classicVert(v);
            }
            
            sampler2D _MainTexA;
            sampler2D _MainTexB;
            sampler2D _MainTexC;
            sampler2D _MainTexD;
            fixed4 _Tint;
            
            float4 frag(v2f input) : COLOR
            {
                return classicFragSet(input, _MainTexA, _MainTexB, _MainTexC, _MainTexD, _Tint);
            }
            
            ENDCG
        }
    }
}