// MIT License
// 
// Copyright (c) 2019 jazz mickle
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
Shader "Jazz/PSX Dither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern("Dither", 2D) = "black" {}
        _Colors ("Color Depth", float) = 32
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DitherPattern;
            float4 _MainTex_ST;
            float _Colors;

            float4 _MainTex_TexelSize;
            float4 _DitherPattern_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

                /// Per channel dither functions

            float channelError(float col, float colMin, float colMax)
            {
                float range = abs(colMin - colMax);
                float aRange = abs(col - colMin);
                return aRange /range;
            }

            float ditheredChannel(float error, float2 ditherBlockUV, float ditherSteps)
            {
                error = floor(error * ditherSteps) / ditherSteps;
                float2 ditherUV = float2(error, 0);
                ditherUV.x += ditherBlockUV.x;
                ditherUV.y = ditherBlockUV.y;
                return tex2D(_DitherPattern, ditherUV).x;
            }

            float4 mix(float4 a, float4 b, float amt)
            {
                return ((1.0 - amt) * a) + (b * amt);
            }

                /// YUV/RGB color space calculations

            float4 RGBtoYUV(float4 rgba) {
                float4 yuva;
                yuva.r = rgba.r * 0.2126 + 0.7152 * rgba.g + 0.0722 * rgba.b;
                yuva.g = (rgba.b - yuva.r) / 1.8556;
                yuva.b = (rgba.r - yuva.r) / 1.5748;
                yuva.a = rgba.a;
                
                // Adjust to work on GPU
                yuva.gb += 0.5;
                
                return yuva;
            }

            float4 YUVtoRGB(float4 yuva) {
                yuva.gb -= 0.5;
                return float4(
                    yuva.r * 1 + yuva.g * 0 + yuva.b * 1.5748,
                    yuva.r * 1 + yuva.g * -0.187324 + yuva.b * -0.468124,
                    yuva.r * 1 + yuva.g * 1.8556 + yuva.b * 0,
                    yuva.a);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture and convert to YUV color space
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 yuv = RGBtoYUV(col);

                // Clamp the YUV color to specified color depth (default: 32, 5 bits per channel, as per playstation hardware)
                float4 col1 = floor(yuv * _Colors) / _Colors;
                float4 col2 = ceil(yuv * _Colors) / _Colors;

                // Calculate dither texture UV based on the input texture
                float ditherSize = _DitherPattern_TexelSize.w;
                float ditherSteps = _DitherPattern_TexelSize.z/ditherSize;

                float2 ditherBlockUV = i.uv;
                ditherBlockUV.x %= (ditherSize / _MainTex_TexelSize.z);
                ditherBlockUV.x /= (ditherSize / _MainTex_TexelSize.z);
                ditherBlockUV.y %= (ditherSize / _MainTex_TexelSize.w);
                ditherBlockUV.y /= (ditherSize / _MainTex_TexelSize.w);
                ditherBlockUV.x /= ditherSteps;

                // Dither each channel individually
                yuv.x = lerp(col1.x, col2.x, ditheredChannel(channelError(yuv.x, col1.x, col2.x), ditherBlockUV, ditherSteps));
                yuv.y = lerp(col1.y, col2.y, ditheredChannel(channelError(yuv.y, col1.y, col2.y), ditherBlockUV, ditherSteps));
                yuv.z = lerp(col1.z, col2.z, ditheredChannel(channelError(yuv.z, col1.z, col2.z), ditherBlockUV, ditherSteps));

                return YUVtoRGB(yuv);
            }
            ENDCG
        }
    }
}