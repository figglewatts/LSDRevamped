#include "UnityCG.cginc"
#include "LSDRFog.cginc"

uniform float _AffineIntensity;
uniform int _TextureSet;

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
    half3 affine : TEXCOORD1;
    float distance : FOG;
};

struct v2g
{
    v2f data;
};

struct g2f
{
    v2f data;
    float4 fogColor : TEXCOORD2;
    float4 barycenter : TEXCOORD3;
};

struct fragOut
{
    fixed4 color : SV_Target;
};

v2g classicVert(appdata v)
{
    v2g output;
    output.data.color = v.color;

    UNITY_SETUP_INSTANCE_ID(v);

    // vertex snapping
    float4 snapToPixel = UnityObjectToClipPos(v.position);
    snapToPixel.xyz = snapToPixel.xyz / snapToPixel.w;
    snapToPixel.x = floor(160 * snapToPixel.x) / 160;
    snapToPixel.y = floor(120 * snapToPixel.y) / 120;
    snapToPixel.xyz *= snapToPixel.w;
    output.data.pos = snapToPixel;

    // affine texture mapping
    const float distance = length(UnityObjectToViewPos(v.position));
    output.data.uv_MainTex = v.uv;
    output.data.uv_MainTex *= distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    output.data.affine = distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    output.data.distance = distance;

    return output;
}

void classicGeom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
{
    half4 barycenter = (IN[0].data.pos + IN[1].data.pos + IN[2].data.pos) / 3;
    const float faceDistance = min(IN[0].data.distance, min(IN[1].data.distance, IN[2].data.distance));

    // render distance
    if (faceDistance > GetFogEnd() + 30)
    {
        return;
    }

    for (int i = 0; i < 3; i++)
    {
        g2f o;

        o.data = IN[i].data;
        o.barycenter = barycenter;

        // Flatten the triangle to the barycenter
        o.data.pos.z = o.data.pos.w * barycenter.z / barycenter.w;

        // handle fog color
        o.fogColor = FogColor(faceDistance);

        triStream.Append(o);
    }

    triStream.RestartStrip();
}

v2f revampedVert(appdata v)
{
    v2f output;

    UNITY_SETUP_INSTANCE_ID(v);

    output.pos = UnityObjectToClipPos(v.position);
    output.color = v.color;
    output.uv_MainTex = v.uv;
    output.affine = 0;

    const float distance = length(UnityObjectToViewPos(v.position));
    output.distance = distance;

    return output;
}

fragOut classicFragCutout(g2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;

    half4 output_col = tex2D(mainTex, input.data.uv_MainTex / input.data.affine.x);

    // alpha cutout
    if (output_col.a <= 0.1) discard;

    // apply vertex color
    output_col *= input.data.color;

    // apply tint
    output_col *= tint;

    // apply fog
    output_col = ApplyClassicFog(output_col, input.fogColor);

    output.color = output_col;

    return output;
}

fragOut classicFrag(g2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;

    half4 output_col = tex2D(mainTex, input.data.uv_MainTex / input.data.affine.x);

    if (output_col.a == 0) discard;

    // apply vertex color
    output_col *= input.data.color;

    // apply tint
    output_col *= tint;

    // apply fog
    output_col = ApplyClassicFog(output_col, input.fogColor);

    output.color = output_col;

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
    const float4 fogColor = FogColor(input.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);

    output.color = output_col;

    return output;
}

fragOut revampedFrag(v2f input, sampler2D mainTex, fixed4 tint)
{
    fragOut output;

    half4 output_col = tex2D(mainTex, input.uv_MainTex);

    if (output_col.a == 0) discard;

    // apply vertex color
    output_col *= input.color;

    // apply tint
    output_col *= tint;

    // apply fog
    const float4 fogColor = FogColor(input.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);

    output.color = output_col;

    return output;
}

fragOut revampedFragSetCutout(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD,
                              fixed4 tint)
{
    fragOut output;
    half4 output_col;

    // choose texture
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.uv_MainTex);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.uv_MainTex);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.uv_MainTex);
    else output_col = tex2D(mainTexA, input.uv_MainTex);

    // alpha cutout
    if (output_col.a <= 0.1) discard;

    // apply vertex color
    output_col *= input.color;

    // apply tint
    output_col *= tint;

    // apply fog
    const float4 fogColor = FogColor(input.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);

    output.color = output_col;

    return output;
}

fragOut revampedFragSet(v2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD,
                        fixed4 tint)
{
    fragOut output;
    half4 output_col;

    // choose texture
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.uv_MainTex);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.uv_MainTex);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.uv_MainTex);
    else output_col = tex2D(mainTexA, input.uv_MainTex);

    // apply vertex color
    output_col *= input.color;

    // apply tint
    output_col *= tint;

    // apply fog
    const float4 fogColor = FogColor(input.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);

    output.color = output_col;

    return output;
}

fragOut classicFragSetCutout(g2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD,
                             fixed4 tint)
{
    fragOut output;
    half4 output_col;

    // choose texture
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.data.uv_MainTex / input.data.affine.x);
    else output_col = tex2D(mainTexA, input.data.uv_MainTex / input.data.affine.x);

    // alpha cutout
    if (output_col.a <= 0.1) discard;

    // apply vertex color
    output_col *= input.data.color;

    // apply tint
    output_col *= tint;

    // apply fog
    output_col = ApplyClassicFog(output_col, input.fogColor);

    output.color = output_col;

    return output;
}

fragOut classicFragSet(g2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD,
                       fixed4 tint)
{
    fragOut output;
    half4 output_col;

    // choose texture
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.data.uv_MainTex / input.data.affine.x);
    else output_col = tex2D(mainTexA, input.data.uv_MainTex / input.data.affine.x);

    // apply vertex color
    output_col *= input.data.color;

    // apply tint
    output_col *= tint;

    // apply fog
    output_col = ApplyClassicFog(output_col, input.fogColor);

    output.color = output_col;

    return output;
}
