Shader "LSDR/RevampedDiffuseSetAlphaBlend" {
    Properties {
        _MainTexA ("Albedo A (RGB)", 2D) = "white" {}
        _MainTexB ("Albedo B (RGB)", 2D) = "white" {}
        _MainTexC ("Albedo C (RGB)", 2D) = "white" {}
        _MainTexD ("Albedo D (RGB)", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
        [PerRendererData]_FogAddition ("FogAddition", Color) = (0, 0, 0)
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // incoming vertices
            struct appdata
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            
            // data from vert to frag shader
            struct revampedV2F
            {
                fixed4 pos : SV_POSITION;
                half4 color : COLOR0;
                float2 uv_MainTex : TEXCOORD0;
            };

            revampedV2F vert(appdata v)
            {
                revampedV2F output;
                output.pos = UnityObjectToClipPos(v.position);
                output.color = v.color;
                output.uv_MainTex = v.uv;
                
                return output;
            }
            
            sampler2D _MainTexA;
            sampler2D _MainTexB;
            sampler2D _MainTexC;
            sampler2D _MainTexD;
            fixed4 _Tint;
            uniform int _TextureSet;
            
            float4 frag(revampedV2F input) : COLOR
            {
                half4 output;
                
                // choose texture
                if (_TextureSet == 2) output = tex2D(_MainTexB, input.uv_MainTex);
                else if (_TextureSet == 3) output = tex2D(_MainTexC, input.uv_MainTex);
                else if (_TextureSet == 4) output = tex2D(_MainTexD, input.uv_MainTex);
                else output = tex2D(_MainTexA, input.uv_MainTex);
                
                // apply vertex color
                output *= input.color;
                
                // apply tint
                output *= _Tint;
                
                return output;
            }
            ENDCG
        }
    }
}