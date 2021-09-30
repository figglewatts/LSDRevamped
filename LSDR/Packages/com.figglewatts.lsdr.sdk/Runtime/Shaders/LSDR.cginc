#include "UnityCG.cginc"
#include "LSDRFog.cginc"

uniform float _AffineIntensity;
uniform float _RenderCutoffAdjustment;
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

v2g vert(appdata v)
{
    v2g output;

    UNITY_SETUP_INSTANCE_ID(v);

    // vertex distance
    const float distance = length(UnityObjectToViewPos(v.position));
    output.data.distance = distance;

    // color and UVs
    output.data.uv_MainTex = v.uv;
    output.data.color = v.color;

    #if defined(LSDR_CLASSIC)
    // vertex snapping
    float4 snapToPixel = UnityObjectToClipPos(v.position);
    snapToPixel.xyz = snapToPixel.xyz / snapToPixel.w;
    snapToPixel.x = floor(160 * snapToPixel.x) / 160;
    snapToPixel.y = floor(120 * snapToPixel.y) / 120;
    snapToPixel.xyz *= snapToPixel.w;
    output.data.pos = snapToPixel;

    // affine texture mapping
    output.data.uv_MainTex *= distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    output.data.affine = distance + (v.position.w * _AffineIntensity * 8) / distance / 2;
    #else
    output.data.pos = UnityObjectToClipPos(v.position);
    output.data.affine = 0;
    #endif

    return output;
}

[maxvertexcount(3)]
void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
{
    #if defined(LSDR_CLASSIC)
    const float faceDistance = min(IN[0].data.distance, min(IN[1].data.distance, IN[2].data.distance));
    half4 barycenter = (IN[0].data.pos + IN[1].data.pos + IN[2].data.pos) / 3;
    
    // render distance
    if (faceDistance > GetFogEnd() + _RenderCutoffAdjustment)
    {
        return;
    }
    #endif

    for (int i = 0; i < 3; i++)
    {
        g2f o;

        o.data = IN[i].data;

        #if defined(LSDR_CLASSIC)
        // Flatten the triangle to the barycenter
        o.barycenter = barycenter;
        o.data.pos.z = o.data.pos.w * barycenter.z / barycenter.w;

        // handle fog color
        o.fogColor = FogColor(faceDistance);
        #else
        o.barycenter = float4(0, 0, 0, 0);
        o.fogColor = float4(0, 0, 0, 1);
        #endif

        triStream.Append(o);
    }

    triStream.RestartStrip();
}

#if defined(LSDR_TEXTURE_SET)
fragOut lsdrFrag(g2f input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD, fixed4 tint)
#else
fragOut lsdrFrag(g2f input, sampler2D mainTex, fixed4 tint)
#endif
{
    fragOut output;
    half4 output_col;

    // handle reading texture colour
    #if defined(LSDR_CLASSIC)
    #if defined(LSDR_TEXTURE_SET)
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.data.uv_MainTex / input.data.affine.x);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.data.uv_MainTex / input.data.affine.x);
    else output_col = tex2D(mainTexA, input.data.uv_MainTex / input.data.affine.x);
    #else
    output_col = tex2D(mainTex, input.data.uv_MainTex / input.data.affine.x);
    #endif
    #else
    #if defined(LSDR_TEXTURE_SET)
    if (_TextureSet == 2) output_col = tex2D(mainTexB, input.data.uv_MainTex);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, input.data.uv_MainTex);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, input.data.uv_MainTex);
    else output_col = tex2D(mainTexA, input.data.uv_MainTex);
    #else
    output_col = tex2D(mainTex, input.data.uv_MainTex);
    #endif
    #endif

    #if defined(LSDR_CUTOUT_ALPHA)
    if (output_col.a <= 0.1) discard;
    #endif

    // apply vertex color
    output_col *= input.data.color;

    // apply tint
    output_col *= tint;

    // apply fog
    #if defined(LSDR_CLASSIC)
    output_col = ApplyClassicFog(output_col, input.fogColor);
    #else
    const float4 fogColor = FogColor(input.data.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);
    #endif

    output.color = output_col;

    return output;
}

#if defined(LSDR_TEXTURE_SET)
sampler2D _MainTexA;
sampler2D _MainTexB;
sampler2D _MainTexC;
sampler2D _MainTexD;
fixed4 _Tint;

fragOut frag(g2f input)
{
    return lsdrFrag(input, _MainTexA, _MainTexB, _MainTexC, _MainTexD, _Tint);
}
#else
sampler2D _MainTex;
fixed4 _Tint;

fragOut frag(g2f input)
{
    return lsdrFrag(input, _MainTex, _Tint);
}
#endif
