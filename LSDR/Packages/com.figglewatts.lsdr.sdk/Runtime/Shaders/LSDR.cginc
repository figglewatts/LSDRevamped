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

struct vertdata
{
    float4 pos : SV_POSITION;
    half4 color : COLOR0;
    float2 uv_MainTex : TEXCOORD0;
    half3 affine : TEXCOORD1;
};

struct fragdata
{
    vertdata data;
    float distance : FOG;
    float3 worldPos : TEXCOORD2;
};

struct fragOut
{
    fixed4 color : SV_Target;
};

fragdata vert(appdata v)
{
    fragdata output;

    UNITY_SETUP_INSTANCE_ID(v);

    // color and UVs
    output.data.uv_MainTex = v.uv;
    output.data.color = v.color;


    output.worldPos = mul(unity_ObjectToWorld, float4(v.position.xyz, 1)).xyz;
    output.distance = length(UnityObjectToViewPos(v.position));

    #if defined(LSDR_CLASSIC)
    // vertex snapping
    float4 snapToPixel = UnityObjectToClipPos(v.position);
    snapToPixel.xyz = snapToPixel.xyz / snapToPixel.w;
    snapToPixel.x = floor(160 * snapToPixel.x) / 160;
    snapToPixel.y = floor(120 * snapToPixel.y) / 120;
    snapToPixel.xyz *= snapToPixel.w;
    output.data.pos = snapToPixel;

    // affine texture mapping
    output.data.uv_MainTex *= output.distance + (v.position.w * _AffineIntensity * 8) / output.distance / 2;
    output.data.affine = output.distance + (v.position.w * _AffineIntensity * 8) / output.distance / 2;
    #else
    output.data.pos = UnityObjectToClipPos(v.position);
    output.data.affine = 0;
    #endif

    if (output.distance > GetFogEnd() + _RenderCutoffAdjustment)
    {
        output.data.pos.z = -1000;
    }

    return output;
}

#if defined(LSDR_TEXTURE_SET)
fragOut lsdrFrag(fragdata input, sampler2D mainTexA, sampler2D mainTexB, sampler2D mainTexC, sampler2D mainTexD,
                 fixed4 tint)
#else
fragOut lsdrFrag(fragdata input, sampler2D mainTex, fixed4 tint)
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
    // operate on floored versions of coords to produce grid effect
    float4 fogColor = FogColor(length(int3(input.worldPos) - int3(_WorldSpaceCameraPos)));
    output_col = ApplyClassicFog(output_col, fogColor);
    #else
    const float4 fogColor = FogColor(input.distance);
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

fragOut frag(fragdata input)
{
    return lsdrFrag(input, _MainTexA, _MainTexB, _MainTexC, _MainTexD, _Tint);
}
#else
sampler2D _MainTex;
fixed4 _Tint;

fragOut frag(fragdata input)
{
    return lsdrFrag(input, _MainTex, _Tint);
}
#endif
