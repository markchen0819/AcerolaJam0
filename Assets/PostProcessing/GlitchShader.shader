Shader "Hidden/GlitchShader"
{
    HLSLINCLUDE
    // StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"


    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    uniform float _DisplacementAmount = 0.05f;
    uniform float _BlockSize = 200.0f;

    float nrand(float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
         float pixelSize = 1.0 / _BlockSize;
         float2 uv = floor(i.texcoord / pixelSize) * pixelSize;
         uv.x = i.texcoord.x + nrand(uv.y * _ScreenParams.xy * _Time.x) * _DisplacementAmount ;
         float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
         return col;
    }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}

// https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Writing-Custom-Effects.html