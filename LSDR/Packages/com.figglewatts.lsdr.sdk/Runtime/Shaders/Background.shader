Shader "LSDR/Background" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 0, 0, 1)
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
        _RotationAngle("Angle", Range(0, 360)) = 0
        _VertexColorIntensity ("Vertex Color Intensity", Float) = 1.5
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Background" }
        LOD 100
        Fog
        {
            Mode Off
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "LSDR.cginc"
            ENDCG
        }
    }
}
