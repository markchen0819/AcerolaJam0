Shader "Custom/Scene5Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TimeSpeed("TimeSpeed", Float) = 0.005
        _ParticleSpeed("ParticleSpeed", Float) = 0.0008
        _Alpha("Alpha", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ParticleSpeed;
            float _TimeSpeed;
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Reference
            // https://www.shadertoy.com/view/Msl3WH

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = 2.0 * i.uv - 1.0;

                float time =  _Time.x* _TimeSpeed;
            

                // 2D rotation matrix
                float s = sin(time);
                float c = cos(time);
                uv = mul(float2x2(c, s, -s, c), uv);

                float3 col = float3(0.0, 0.0, 0.0);
                float3 init = float3(0.0, 0.0, time * _ParticleSpeed);

                float dragAmount = 0.0;
                float v = 0.0;
                for (int r = 0; r < 100; r++) // ray march
                {
                    float3 dir = float3(uv, 0.15);
                    float3 p = init + dragAmount * dir;

                    p.z = fmod(p.z, 0.1); // fractal for cycle

                    float dirScale = 2.;
                    float amount = 0.75; // ensure in fractal space, affect shape

                    for (int i = 0; i < 10; i++)
                    {
                        p = abs(p * dirScale) / dot(p, p) - amount;
                    }

                    // fade out
                    v += length(p * p) * smoothstep(0.0, 0.5, 0.9 - dragAmount) * .002;

                    // Colorize
                    float3 adjustedColor = float3(v * 0.8, 1.1 - dragAmount * 0.5, .7 + v * 0.5);
                    col += adjustedColor * v * 0.013;

                    dragAmount += .01;
                }
                return float4(col, _Alpha);
            }
            ENDCG
        }
    }
}
