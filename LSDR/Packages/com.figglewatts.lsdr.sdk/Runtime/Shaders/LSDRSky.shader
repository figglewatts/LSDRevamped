Shader "LSDR/GradientSky"
{
    Properties
    {
        _FogHeight("Fog height", Range(0.0, 1.0)) = 0.0
        _FogGradient("Fog gradient", Range(0.0, 1.0)) = 0.2
        _SkyColor("Sky color", Color) = (0, 0.5, 0.5, 1)
        _FogColor("Fog color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Background" "Queue"="Background-1"
        }

        Pass
        {
            ZWrite Off
            Cull Off
            Fog
            {
                Mode Off
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct vertdata
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            vertdata vert(appdata v)
            {
                vertdata o;
                o.position = UnityObjectToClipPos(v.position);
                o.texcoord = v.texcoord;
                o.screenPos = ComputeScreenPos(o.position);
                return o;
            }

            uniform float _FogHeight;
            uniform float _FogGradient;
            uniform float4 _SkyColor;
            uniform float4 _FogColor;

            fixed4 skyGradient(float y, float heightOffset)
            {
                y += heightOffset;
                const float upper = saturate((y - (_FogHeight / 2.0) - 0.5) / _FogGradient);
                const float lower = saturate((0.5 - (y + (_FogHeight / 2.0))) / _FogGradient);
                const float combined = upper + lower;

                const fixed4 skyComponent = _SkyColor * combined;
                const fixed4 fogComponent = _FogColor * (1.0 - combined);
                return skyComponent + fogComponent;
            }

            fixed4 frag(vertdata i) : SV_Target
            {
                // sky is rendered in screenspace - so we need screenspace UVs
                float2 normalizedScreenUV = i.screenPos.xy / i.screenPos.w;

                // we need to 'pin' the fog to the horizon, so we can do this with the dot product of the camera's
                // forward vector and the 'up' vector
                const float3 cameraForward = unity_CameraToWorld._m02_m12_m22;
                const float3 cameraUp = unity_CameraToWorld._m01_m11_m21;
                const float upDirection = sign(cameraUp.y);
                const float heightOffset = dot(cameraForward, float3(0, upDirection, 0));

                // calculate sky gradient
                fixed4 finalCol = skyGradient(normalizedScreenUV.y, heightOffset);

                return finalCol;
            }
            ENDCG
        }
    }
}