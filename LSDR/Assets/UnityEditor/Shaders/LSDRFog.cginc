#include "HSL.cginc"

uniform int _SubtractiveFog;

float4 ApplyFog(half4 color, float amount)
{
    float3 hslFogCol = rgb2hsl(unity_FogColor.rgb);
    
    // modify lightness based on fog amount
    hslFogCol.z = lerp(hslFogCol.z, 0, amount);
  
    // convert back into RGB
    float3 modifiedFogCol = hsl2rgb(hslFogCol);
    
    // perform addition/subtraction based on the fog mode
    float3 finalFogCol = ((color.rgb - (1 - modifiedFogCol)) * _SubtractiveFog) + ((color.rgb + modifiedFogCol) * !_SubtractiveFog);
    
    color.rgb = lerp(finalFogCol, color.rgb, amount);
    return color;
}