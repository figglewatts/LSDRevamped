#include <UnityShaderVariables.cginc>

#include "HSL.cginc"

uniform int _SubtractiveFog;

float GetFogEnd()
{
    float der = -1 / unity_FogParams.z;
    return unity_FogParams.w * der;
}

float GetFogAmount(float distance)
{
    #if defined(FOG_LINEAR)
    float baseFogAmt = saturate(distance * unity_FogParams.z + unity_FogParams.w);
    float quantized = round(baseFogAmt / 0.1) * 0.1;
    return quantized;
    #else
    return 1;
    #endif
}

float4 ApplyFog(half4 color, float amount)
{
    float3 hslFogCol = rgb2hsl(unity_FogColor.rgb);

    // modify lightness based on fog amount
    hslFogCol.z = lerp(hslFogCol.z, _SubtractiveFog, amount);

    // convert back into RGB
    const float3 modifiedFogCol = hsl2rgb(hslFogCol);

    // perform addition/subtraction based on the fog mode
    const float3 finalFogCol = ((color.rgb - (1 - modifiedFogCol)) * _SubtractiveFog) + ((color.rgb + modifiedFogCol) *
        !_SubtractiveFog);

    color.rgb = lerp(finalFogCol, color.rgb, amount);
    return color;
}
