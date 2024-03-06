Shader "Custom/Scene4Shader"
{
    Properties
    {
        _GradientTexture("GradientTexture", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _TimeSpeed("TimeSpeed", Float) = 10.0
        [HDR]_MixColor("MixColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _GradientTexture;
            float4 _GradientTexture_ST;
            sampler2D _Noise;
            float _TimeSpeed;
            float4 _MixColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GradientTexture);
                return o;
            }

            // Reference
            // https://www.youtube.com/watch?v=IvOfx-kbqac

            fixed4 frag(v2f i) : SV_Target
            {
                // 0~1 -> 0~16/9 -> recenter
                float2 uv = i.uv ;
                //uv.x = uv.x * (16.0/9.0);
                float2 textureUV = uv;
                uv.x -= (16.0/9.0 - 1.0) / 2.0;

                // color channel different directions
                float2 rUV = uv;
                rUV.x += _Time.x * _TimeSpeed;
                rUV.y -= 5 * _Time.x * _TimeSpeed;
                float2 gUV = uv;
                gUV.x += -1 * _Time.x * _TimeSpeed;
                gUV.y -= 2 *  _Time.x * _TimeSpeed;
                float2 bUV = uv;
                bUV.y += _Time.x * _TimeSpeed;

                float r = tex2D(_Noise, rUV).r;
                float g = tex2D(_Noise, gUV).g;
                float b = tex2D(_Noise, bUV).b;
                float gradient = tex2D(_GradientTexture, textureUV).r;

                // use noise alpha & graident alpha
                fixed3 noiseColor = fixed3(r, g, b);
                float gradientExtraAlpha = gradient * 0.01;
                float noiseAlpha = clamp(r * g * b  * gradient + gradientExtraAlpha, 0.0, 1.0);

                fixed4 col = _MixColor;
                col.a = noiseAlpha;

                return col;


            }
            ENDCG
        }
    }
}
