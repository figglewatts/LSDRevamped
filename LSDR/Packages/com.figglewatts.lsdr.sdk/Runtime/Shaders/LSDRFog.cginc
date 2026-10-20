#include <UnityShaderVariables.cginc>

#include "HSL.cginc"

uniform int _SubtractiveFog;

float GetFogEnd()
{
    const float der = -1 / unity_FogParams.z;
    return unity_FogParams.w * der;
}

float GetFogStart()
{
    return GetFogEnd() + 1 / unity_FogParams.z;
}

// how clear something is at a distance, 1 is no fog and 0 is fully fogged. fog goes with 1/distance so it
// thickens quickly past the start, and classic fogs in a few coarse steps
float GetFogDepth(float distance)
{
    const float fogStart = max(GetFogStart(), 0.001);
    const float fogEnd = max(GetFogEnd(), fogStart + 0.001);
    const float fog = (1 - fogStart / max(distance, fogStart)) / (1 - fogStart / fogEnd);

    float clear = (1 - saturate(fog)) * (1 - saturate(fog));

    #if defined(LSDR_CLASSIC)
    clear = 1 - floor((1 - clear) * 8) / 8;
    #endif

    return clear;
}

float3 Complement(float3 color)
{
    return float3(1 - color.r, 1 - color.g, 1 - color.b);
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

float3 SubtractiveFog(float3 color, float4 fogColor, float factor)
{
    return lerp(color, lerp(color, fogColor.rgb, 1 - fogColor.a), factor);
}

float4 ApplyClassicFog(float4 color, float4 fogColor)
{
    // perform addition/subtraction based on the fog mode
    float3 finalFogCol = color.rgb;

    #if defined(FOG_LINEAR)
    finalFogCol = AdditiveFog(finalFogCol, fogColor.rgb, 1.0 - _SubtractiveFog);
    finalFogCol = SubtractiveFog(finalFogCol, fogColor.rgba, _SubtractiveFog);
    #endif

    return float4(finalFogCol.rgb, color.a);
}

float4 ApplyRevampedFog(float4 color, float4 fogColor)
{
    float3 finalFogCol = color.rgb;

    #if defined(FOG_LINEAR)
    finalFogCol = AdditiveFog(finalFogCol, fogColor.rgb, 1.0 - _SubtractiveFog);
    finalFogCol = SubtractiveFog(finalFogCol, fogColor.rgba, _SubtractiveFog);
    #endif

    return float4(finalFogCol.rgb, color.a);
}
