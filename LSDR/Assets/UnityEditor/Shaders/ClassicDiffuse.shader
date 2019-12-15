// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "LSDR/ClassicDiffuse" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
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
            
            sampler2D _MainTex;
            fixed4 _Tint;
            
            float4 frag(v2f input) : COLOR
            {
                return classicFragCutout(input, _MainTex, _Tint);
            }
            
            ENDCG
        }
    }
}