Shader "Unlit/RefractionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // render texture
        _ShapeTex("ShapeTex", 2D) = "white" {} // affected area
        _Zoom("Zoom", Float) = 1.0
        _UVDisplacement("uvDisplacement", Vector) = (0.0,0.0,0.0,0.0)
        _TimeSpeed("TimeSpeed", Float) = 0.005
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
            // make fog work
            #pragma multi_compile_fog

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
            float4 _MainTex_ST;
            float _Zoom;
            float4 _UVDisplacement;
            sampler2D _ShapeTex;
            float _TimeSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float2 renderTexUV = (i.uv - 0.5) / _Zoom + 0.5 + _UVDisplacement.xy;
                fixed4 col = tex2D(_MainTex, renderTexUV);

                float2 shapeUV = i.uv;
                shapeUV.y += _Time.x * _TimeSpeed;

                col.a = tex2D(_ShapeTex, shapeUV).r ;
                //col.rgb = fixed3(1.0, 1.0, 1.0) - col.rgb;
                col.rgb -= fixed3(0.02, 0.02, 0.02);
                return col;
            }
            ENDCG
        }
    }
}
