#include "HSL.cginc"
#include "UnityCG.cginc"

uniform int _SubtractiveFog;
uniform float _AffineIntensity;
uniform int _TextureSet;

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
    float3 modifiedFogCol = hsl2rgb(hslFogCol);
    
    // perform addition/subtraction based on the fog mode
    float3 finalFogCol = ((color.rgb - (1 - modifiedFogCol)) * _SubtractiveFog) + ((color.rgb + modifiedFogCol) * !_SubtractiveFog);
    
    color.rgb = lerp(finalFogCol, color.rgb, amount);
    return color;
}


// incoming vertices
struct appdata
{
    float4 position : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 pos : SV_POSITION;
    half4 color : COLOR0;
    float2 uv_MainTex : TEXCOORD0;
    half3 normal : TEXCOORD1;
    float fogAmount : FOG;
    float depth : SV_Depth;
};

struct fragOut
{
    fixed4 color : SV_Target;
    float depth : SV_Depth;
};

v2f classicVert(appdata v)
{
    v2f output;
                
    UNITY_SETUP_INSTANCE_ID(v);
    
    // vertex snapping
    float4 snapToPixel = UnityObjectToClipPos(v.position);
    snapToPixel.xyz = snapToPixel.xyz / snapToPixel.w;
    snapToPixel.x = floor(160 * snapToPixel.x) / 160;
    snapToPixel.y = floor(120 * snapToPixel.y) / 120;
    snapToPixel.xyz *= snapToPixel.w;
    output.pos = snapToPixel;
    
    // vertex color
    output.color = v.color;
    
    float distance = length(UnityObjectToViewPos(v.position));
    
    // affine texture mapping
    output.uv_MainTex = v.uv;
    output.uv_MainTex *= distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    output.normal = distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    
    // fog amount
    float3 objPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
    float cameraDist = length(_WorldSpaceCameraPos - objPos);
    output.fogAmount = GetFogAmount(cameraDist);
    
    // depth
    // bit of a hack, we want the depth to be kinda different between tiles but also to have a
    // gradient to prevent Z-fighting on self-occlusions, so we derive depth from both the distance
    // from the camera as well as the vertex's viewspace position
    float depth = ((cameraDist + distance) - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
    output.depth = 1 - depth;
    
    if (cameraDist > GetFogEnd())
    {
        output.pos = float4(0, 0, 0, 1);
    }
    
    return output;
}

v2f revampedVert(appdata v)
{
    v2f output;
    
    UNITY_SETUP_INSTANCE_ID(v);
    
    output.pos = UnityObjectToClipPos(v.position);
    output.color = v.color;
    output.uv_MainTex = v.uv;
    output.normal = 0;
    
    float distance = length(UnityObjectToViewPos(v.position));
    
    // fog amount
    float3 objPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
    float cameraDist = length(_WorldSpaceCameraPos - objPos);
    output.fogAmount = GetFogAmount(cameraDist);
    
    // depth
    // bit of a hack, we want the depth to be kinda different between tiles but also to have a
    // gradient to prevent Z-fighting on self-occlusions, so we derive depth from both the distance
    // from the camera as well as the vertex's viewspace position
    float depth = ((cameraDist + distance) - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
    output.depth = 1 - depth;
    
    if (cameraDist > GetFogEnd())
    {
        output.pos = float4(0, 0, 0, 1);
    }
    
    return output;
}

fragOut classicFragCutout(v2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;
    
    half4 output_col = tex2D(mainTex, input.uv_MainTex / input.normal.x);
    
    // alpha cutout
    if (output_col.a <= 0.1) discard;
    
    // apply vertex color
    output_col *= input.color;
    
    // apply tint
    output_col *= tint;
    
    // apply fog
    output_col = ApplyFog(output_col, input.fogAmount);
    
    output.color = output_col;
    output.depth = input.depth;
    
    return output;
}

fragOut classicFrag(v2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;
    
    half4 output_col = tex2D(mainTex, input.uv_MainTex / input.normal.x);
    
    if (output_col.a == 0) discard;
    
    // apply vertex color
    output_col *= input.color;
    
    // apply tint
    output_col *= tint;
    
    // apply fog
    output_col = ApplyFog(output_col, input.fogAmount);
    
    output.color = output_col;
    output.depth = input.depth;
    
    return output;
}

fragOut revampedFragCutout(v2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;
    
    half4 output_col = tex2D(mainTex, input.uv_MainTex);
                
    // alpha cutout
    if (output_col.a <= 0.1) discard;
    
    // apply vertex color
    output_col *= input.color;
    
    // apply tint
    output_col *= tint;
    
    // apply fog
    output_col = ApplyFog(output_col, input.fogAmount);
    
    output.color = output_col;
    output.depth = input.depth;
    
    return output;
}

fragOut revampedFrag(v2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;
    
    half4 output_col = tex2D(mainTex, input.uv_MainTex);
    
    // apply vertex color
    output_col *= input.color;
    
    // apply tint
    output_col *= tint;
    
    // apply fog
    output_col = ApplyFog(output_col, input.fogAmount);
    
    output.color = output_col;
    output.depth = input.depth;
    
    return output;
}

float4 revampedFragSetCutout(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD, fixed4 tint)
{
    half4 output;
                
    // choose texture
    if (_TextureSet == 2) output = tex2D(mainTexB, input.uv_MainTex);
    else if (_TextureSet == 3) output = tex2D(mainTexC, input.uv_MainTex);
    else if (_TextureSet == 4) output = tex2D(mainTexD, input.uv_MainTex);
    else output = tex2D(mainTexA, input.uv_MainTex);
    
    // alpha cutout
    if (output.a <= 0.1) discard;
    
    // apply vertex color
    output *= input.color;
    
    // apply tint
    output *= tint;
    
    // apply fog
    output = ApplyFog(output, input.fogAmount);
    
    return output;
}

float4 revampedFragSet(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD, fixed4 tint)
{
    half4 output;
                
    // choose texture
    if (_TextureSet == 2) output = tex2D(mainTexB, input.uv_MainTex);
    else if (_TextureSet == 3) output = tex2D(mainTexC, input.uv_MainTex);
    else if (_TextureSet == 4) output = tex2D(mainTexD, input.uv_MainTex);
    else output = tex2D(mainTexA, input.uv_MainTex);
    
    // apply vertex color
    output *= input.color;
    
    // apply tint
    output *= tint;
    
    // apply fog
    output = ApplyFog(output, input.fogAmount);
    
    return output;
}

float4 classicFragSetCutout(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD, fixed4 tint)
{
    half4 output;
                
    // choose texture
    if (_TextureSet == 2) output = tex2D(mainTexB, input.uv_MainTex / input.normal.x);
    else if (_TextureSet == 3) output = tex2D(mainTexC, input.uv_MainTex / input.normal.x);
    else if (_TextureSet == 4) output = tex2D(mainTexD, input.uv_MainTex / input.normal.x);
    else output = tex2D(mainTexA, input.uv_MainTex / input.normal.x);
    
    // alpha cutout
    if (output.a <= 0.1) discard;
    
    // apply vertex color
    output *= input.color;
    
    // apply tint
    output *= tint;
    
    // apply fog
    output = ApplyFog(output, input.fogAmount);
    
    return output;
}

float4 classicFragSet(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD, fixed4 tint)
{
    half4 output;
                
    // choose texture
    if (_TextureSet == 2) output = tex2D(mainTexB, input.uv_MainTex / input.normal.x);
    else if (_TextureSet == 3) output = tex2D(mainTexC, input.uv_MainTex / input.normal.x);
    else if (_TextureSet == 4) output = tex2D(mainTexD, input.uv_MainTex / input.normal.x);
    else output = tex2D(mainTexA, input.uv_MainTex / input.normal.x);
    
    // apply vertex color
    output *= input.color;
    
    // apply tint
    output *= tint;
    
    // apply fog
    output = ApplyFog(output, input.fogAmount);
    
    return output;
}