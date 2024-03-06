Shader "Custom/Scene1Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BoxWidth("BoxWidth", Float) = 0.1
        _Brightness("Brightness", Float) = 0.01
        _GroundHeight("GroundHeight", Float) = 0.01
        _BlendAmount("BlendAmount", Float) = 0.01
        _TimeSpeed("TimeSpeed", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
            float _BoxWidth;
            float _Brightness;
            float _BlendAmount;
            float _GroundHeight;
            float _TimeSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Reference
            // https://youtu.be/f4s1h2YETNY?si=13qMQ0uZDLQXBNbR
            // https://youtu.be/khblXafu7iA?si=z3qfUFAuzICoCr4W
            // https://iquilezles.org/articles/distfunctions/
            // https://www.shadertoy.com/view/Ml3Gz8

            const float width = 1.6;
            const float height = 0.9;
            float smin(float a, float b, float k) 
            {
                float h = clamp(0.5 + 0.5 * (a - b) / k, 0.0, 1.0);
                return lerp(a, b, h) - k * h * (1.0 - h);
            }
            float sdSphere(float3 p, float r)
            {
                return length(p) - r;
            }
            float sdBox(float3 p, float3 b)
            {
                float3 d = abs(p) - b;
                return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
            }

            float map(float3 pt)
            {
                pt.z += _Time.x * _TimeSpeed;

                float3 q = pt;
                q = frac(q) - 0.5; // Space repetition
                float box = sdBox(q, float3(_BoxWidth, _BoxWidth, _BoxWidth)); // Cube SDF
                float ground = pt.y + _GroundHeight; // Ground SDF
                return smin(ground, box, _BlendAmount);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 0~1 -> 0~16/9 -> recenter
                float2 uv = i.uv ;
                uv.x = uv.x * (16.0/9.0);
                uv.x -= (16.0/9.0 - 1.0) / 2.0;

                // Ray march
                float3 ro = float3(0, 0, -5); 
                float3 rd = normalize(float3(uv - 0.5, 1));
                fixed4 col = fixed4(1, 1, 1, 1);

                float distSoFar = 0;
                for (int i = 0; i < 100; ++i)
                {
                    float3 pt = ro + rd * distSoFar;
                    float dist = map(pt);
                    distSoFar += dist;
                    if (dist < 0.01 || distSoFar > 200) break;
                }
                col = fixed4(distSoFar * _Brightness, distSoFar * _Brightness, distSoFar * _Brightness, 1);
                return col;
            }
            ENDCG
        }
    }
}
