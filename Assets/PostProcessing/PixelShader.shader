Shader "Hidden/PixelShader"
{
    HLSLINCLUDE
    // StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    uniform float _PixelationAmount; // Higher for more pixelation

    float4 Frag(VaryingsDefault i) : SV_Target
    {
         float pixelSize = 1.0 / _PixelationAmount;
         float2 uv = floor(i.texcoord / pixelSize) * pixelSize;
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