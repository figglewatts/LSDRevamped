#include "UnityCG.cginc"
#include "LSDRFog.cginc"

uniform float _AffineIntensity;
uniform float _RenderCutoffAdjustment;
uniform int _TextureSet;
uniform float _VertexColorIntensity;
uniform float _Brightness;

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
    float4 clipPos : TEXCOORD1;
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
    output.data.clipPos = UnityObjectToClipPos(v.position);
    output.data.clipPos.w += 0.25;
    output.data.clipPos.w = lerp(1, output.data.clipPos.w, _AffineIntensity);
    output.data.uv_MainTex *= output.data.clipPos.w;
    #else
    output.data.pos = UnityObjectToClipPos(v.position);
    output.data.clipPos = 1;
    #endif

    if (output.distance > GetFogEnd() + _RenderCutoffAdjustment)
    {
        output.data.pos.z = -1000;
    }

    return output;
}

float applyIntensity(float v, float p)
{
    float adj = tanh((v - 0.5) * p);
    return (adj + 1) / 2;
}

half3 applyVertexColorIntensity(half3 color)
{
    return half3(applyIntensity(color.r, _VertexColorIntensity),
                 applyIntensity(color.g, _VertexColorIntensity),
                 applyIntensity(color.b, _VertexColorIntensity));
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

    #if defined(LSDR_CLASSIC)
    float2 uvs = input.data.uv_MainTex / input.data.clipPos.w;
    #else
    float2 uvs = input.data.uv_MainTex;
    #endif

    // handle reading texture colour
    #if defined(LSDR_TEXTURE_SET)
    if (_TextureSet == 2) output_col = tex2D(mainTexB, uvs);
    else if (_TextureSet == 3) output_col = tex2D(mainTexC, uvs);
    else if (_TextureSet == 4) output_col = tex2D(mainTexD, uvs);
    else output_col = tex2D(mainTexA, uvs);
    #else
    output_col = tex2D(mainTex, uvs);
    #endif

    #if defined(LSDR_CUTOUT_ALPHA)
    if (output_col.a <= 0.1) discard;
    #endif

    // apply vertex color and alpha
    output_col.rgb *= applyVertexColorIntensity(input.data.color);
    output_col.a *= input.data.color.a;

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

    output_col.rgb *= _Brightness;
    output.color = output_col;

    return output;
}

#if defined(LSDR_WATER)
sampler2D _MainTextureA;
sampler2D _MainTextureB;
sampler2D _MainTextureC;
sampler2D _MainTextureD;
sampler2D _WaterPaletteA;
sampler2D _WaterPaletteB;
sampler2D _WaterPaletteC;
sampler2D _WaterPaletteD;
fixed4 _Tint;
float _AnimationSpeed;
float _Alpha;

fragOut frag(fragdata input)
{
    fragOut output;

#if defined(LSDR_CLASSIC)
    float2 uvs = input.data.uv_MainTex / input.data.clipPos.w;
#else
    float2 uvs = input.data.uv_MainTex;
#endif


    half4 waterMap;
    if (_TextureSet == 2) waterMap = tex2D(_MainTextureB, uvs);
    else if (_TextureSet == 3) waterMap = tex2D(_MainTextureC, uvs);
    else if (_TextureSet == 4) waterMap = tex2D(_MainTextureD, uvs);
    else waterMap = tex2D(_MainTextureA, uvs);

    float paletteIdx = (waterMap.r - _Time[0] * _AnimationSpeed) % 1.0;

    half4 output_col;
    if (_TextureSet == 2) output_col = tex2D(_WaterPaletteB, float2(paletteIdx, 0.5));
    else if (_TextureSet == 3) output_col = tex2D(_WaterPaletteC, float2(paletteIdx, 0.5));
    else if (_TextureSet == 4) output_col = tex2D(_WaterPaletteD, float2(paletteIdx, 0.5));
    else output_col = tex2D(_WaterPaletteA, float2(paletteIdx, 0.5));

    // apply vertex color
    output_col *= (input.data.color * 1.3);

    // apply tint
    output_col *= _Tint;

    // apply fog
#if defined(LSDR_CLASSIC)
    // operate on floored versions of coords to produce grid effect
    float4 fogColor = FogColor(length(int3(input.worldPos) - int3(_WorldSpaceCameraPos)));
    output_col = ApplyClassicFog(output_col, fogColor);
#else
    const float4 fogColor = FogColor(input.distance);
    output_col = ApplyRevampedFog(output_col, fogColor);
#endif

    output.color = output_col * _Brightness;
    output.color.a = min(_Alpha, waterMap.a);

    return output;
}
#elif defined(LSDR_TEXTURE_SET)
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
