#include <UnityShaderVariables.cginc>

#include "HSL.cginc"

uniform int _SubtractiveFog;

float GetFogEnd()
{
    const float der = -1 / unity_FogParams.z;
    return unity_FogParams.w * der;
}

float GetFogDepth(float distance)
{
    UNITY_CALC_FOG_FACTOR_RAW(distance);
    return saturate(unityFogFactor);
}

float4 FogColor(float distance)
{
    float fogDepth = GetFogDepth(distance);
    float3 fogColor = lerp(unity_FogColor.rgb, float3(0, 0, 0), fogDepth);
    return float4(fogColor, fogDepth);
}

float3 AdditiveFog(float3 color, float3 fogColor, float factor)
{
    return color + fogColor * factor;
}

float3 SubtractiveFog(float3 color, float3 fogColor, float factor)
{
    return max(float3(0, 0, 0), color - fogColor * factor);
}

float4 ApplyClassicFog(float4 color, float4 fogColor)
{
    // perform addition/subtraction based on the fog mode
    float3 finalFogCol = color.rgb;

    #if defined(FOG_LINEAR)
    finalFogCol = AdditiveFog(finalFogCol, fogColor.rgb, 1.0 - _SubtractiveFog);
    finalFogCol = SubtractiveFog(finalFogCol, fogColor.rgb, _SubtractiveFog);
    #endif

    return float4(finalFogCol.rgb, color.a);
}

float4 ApplyRevampedFog(float4 color, float4 fogColor)
{
    float3 finalFogCol = color.rgb;

    #if defined(FOG_LINEAR)
    finalFogCol = AdditiveFog(finalFogCol, fogColor.rgb, 1.0 - _SubtractiveFog);
    finalFogCol = SubtractiveFog(finalFogCol, fogColor.rgb, _SubtractiveFog);
    #endif

    return float4(finalFogCol.rgb, color.a);
}
