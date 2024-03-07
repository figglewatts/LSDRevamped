Shader "LSDR/Sun" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 0, 0, 1)
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
        _RotationAngle("Angle", Range(0, 360)) = 0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Background" }
        LOD 100
        Fog
        {
            Mode Off
        }
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScaleX;
            float _ScaleY;
            float _Cutoff;
            float _RotationAngle;
            float4 _Tint;

            v2f vert (appdata v) {
                v2f o;

                // Compute rotation in radians
                float angle = _RotationAngle * (3.14159265359 / 180.0);
                float c = cos(angle);
                float s = sin(angle);

                // Create a rotation matrix for Z-axis rotation
                float4x4 rotationZ = float4x4(
                    c, -s, 0, 0,
                    s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                );

                // Apply rotation
                float4 rotatedPosition = mul(rotationZ, v.vertex * float3(_ScaleX, _ScaleY, 1));
                
                float4 camPos = float4(UnityObjectToViewPos(float3(0, 0, 0)).xyz, 1.0);    // UnityObjectToViewPos(pos) is equivalent to mul(UNITY_MATRIX_MV, float4(pos, 1.0)).xyz,
                                                                                    // This gives us the camera's origin in 3D space (the position (0,0,0) in Camera Space)
                
                float4 viewDir = float4(rotatedPosition.x, rotatedPosition.y, 0.0, 0.0);            // Since w is 0.0, in homogeneous coordinates this represents a vector direction instead of a position
                float4 outPos = mul(UNITY_MATRIX_P, camPos + viewDir);            // Add the camera position and direction, then multiply by UNITY_MATRIX_P to get the new projected vert position
                o.vertex = outPos;
                o.uv = v.uv;
 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Tint.rgb * 2;
                col.a *= _Tint.a;
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
}
