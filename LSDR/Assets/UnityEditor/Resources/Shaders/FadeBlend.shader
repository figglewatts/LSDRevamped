Shader "Hidden/FadeBlend"
{
    Properties
    {
    _MainTex ("Texture", 2D) = "white" {}
    _BlendOp ("Blend Op", Float) = 0
    _Subtract ("Subtract", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }

        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        BlendOp [_BlendOp]
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            float _Subtract;

            fixed4 frag(v2f i) : SV_Target
            {
                // adding brightens towards the colour, subtracting darkens towards it
                fixed3 col = _Subtract > 0.5 ? 1 - i.color.rgb : i.color.rgb;
                return fixed4(col * i.color.a, 1);
            }
            ENDCG
        }
    }
}