Shader "Unlit/NoteShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleSpeed("RippleSpeed", Float) = 10.0
        _CeilDensity("CeilDensity", Float) = 10.0
        [HDR]_BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        [HDR]_RippleColor("RippleColor", Color) = (1, 1, 1, 1)
        _Slimness("Slimness", Float) = 10.0
        _Alpha("Alpha", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "LightMode" = "ForwardBase"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        // Reference
        // Shadows
        // https://docs.unity3d.com/550/Documentation/Manual/SL-VertexFragmentShaderExamples.html
        // Water 
        // https://youtu.be/Vg0L9aCRWPE?si=_rwHhV7GcoM-TbRZ
        // https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Voronoi-Node.html

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" 

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL; 
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                fixed4 diff : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RippleSpeed;
            float _CeilDensity;
            float _Slimness;
            float4 _RippleColor;
            float4 _BaseColor;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

                return o;
            }

            inline float2 unity_voronoi_noise_randomVector(float2 UV, float offset)
            {
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                UV = frac(sin(mul(UV, m)) * 46839.32);
                return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
            }
            void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                float t = 8.0;
                float3 res = float3(8.0, 0.0, 0.0);

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 lattice = float2(x, y);
                        float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                        float d = distance(lattice + offset, f);
                        if (d < res.x)
                        {
                            res = float3(d, offset.x, offset.y);
                            Out = res.x;
                            Cells = res.y;
                        }
                    }
                }
            }
            void Unity_RadialShear_float(float2 UV, float2 Center, float Strength, float2 Offset, out float2 Out)
            {
                float2 delta = UV - Center;
                float delta2 = dot(delta.xy, delta.xy);
                float2 delta_offset = delta2 * Strength;
                Out = UV + float2(delta.y, -delta.x) * delta_offset + Offset;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 newUV;
                Unity_RadialShear_float(i.uv, float2(0.5,0.5), 5, float2(0,0), newUV);
                // Procedural Voronoi
                float angleOffset = _RippleSpeed * _Time.x;
                float cellDensity = _CeilDensity;
                float val;
                float cells;
                Unity_Voronoi_float(newUV + float2(0, _Time.x), angleOffset, cellDensity, val, cells);

                // Black part slimness
                val = pow(val, _Slimness);
                fixed4 col = lerp(_BaseColor, _RippleColor, val); 
                col *= i.diff;
                col.a = _Alpha;
                return col;
            }
            ENDCG
        }
        // shadow caster rendering pass, implemented manually
        // using macros from UnityCG.cginc
        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
               SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
