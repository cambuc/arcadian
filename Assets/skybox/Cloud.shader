Shader "Custom/Cloud"
{
    Properties
    {
        _MainTex0 ("Cloud Texture 0", 2D) = "white" {}
        _MainTex1 ("Cloud Texture 1", 2D) = "white" {}
        _MainTex2 ("Cloud Texture 2", 2D) = "white" {}
        _MainTex3 ("Cloud Texture 3", 2D) = "white" {}

        _Weight0 ("Texture 0 Weight", Range(0,1)) = 0.25
        _Weight1 ("Texture 1 Weight", Range(0,1)) = 0.25
        _Weight2 ("Texture 2 Weight", Range(0,1)) = 0.25
        _Weight3 ("Texture 3 Weight", Range(0,1)) = 0.25

        _Color ("Tint Color", Color) = (1,1,1,1)
        _MaskTex ("Mask Texture (Alpha)", 2D) = "white" {}

        _ScrollX ("Main Scroll Speed X", Float) = 0.1
        _ScrollY ("Main Scroll Speed Y", Float) = 0.0

        _TileSize ("Tile Size", Float) = 1.0
        _EdgeFadeWidth ("Edge Fade Width", Range(0.001, 0.2)) = 0.03

        _NearFadeDistance ("Near Fade Distance", Float) = 1.0
        _FarFadeDistance ("Far Fade Distance", Float) = 10.0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        LOD 100
        Cull Back
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling

            #include "UnityCG.cginc"

            sampler2D _MainTex0;
            sampler2D _MainTex1;
            sampler2D _MainTex2;
            sampler2D _MainTex3;
            sampler2D _MaskTex;

            float4 _MainTex0_ST;
            float4 _MaskTex_ST;

            float _Weight0;
            float _Weight1;
            float _Weight2;
            float _Weight3;

            float _ScrollX;
            float _ScrollY;
            float _TileSize;
            float _EdgeFadeWidth;

            float _NearFadeDistance;
            float _FarFadeDistance;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uvMain : TEXCOORD0;
                float2 uvMask : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float viewDepth : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 78.233);
                return frac(p.x * p.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 viewPos = mul(UNITY_MATRIX_V, worldPos);

                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                o.viewDepth = -viewPos.z;

                float2 scroll = float2(_ScrollX, _ScrollY) * _Time.y;

                float2 uvMain = TRANSFORM_TEX(v.uv, _MainTex0);
                o.uvMain = uvMain + scroll;

                float2 uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
                o.uvMask = uvMask + scroll;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float2 scaledUV = i.uvMain / _TileSize;

                float2 tileCoord = floor(scaledUV);
                float2 localUV = frac(scaledUV);

                float rand = Hash21(tileCoord);

                // Normalize weights
                float total = _Weight0 + _Weight1 + _Weight2 + _Weight3;
                float w0 = _Weight0 / total;
                float w1 = _Weight1 / total;
                float w2 = _Weight2 / total;
                float w3 = _Weight3 / total;

                fixed4 mainTex;

                if (rand < w0)
                    mainTex = tex2D(_MainTex0, localUV);
                else if (rand < w0 + w1)
                    mainTex = tex2D(_MainTex1, localUV);
                else if (rand < w0 + w1 + w2)
                    mainTex = tex2D(_MainTex2, localUV);
                else
                    mainTex = tex2D(_MainTex3, localUV);

                // Edge fade
                float2 edgeDist = min(localUV, 1.0 - localUV);
                float edge = min(edgeDist.x, edgeDist.y);
                float edgeFade = smoothstep(0.0, _EdgeFadeWidth, edge);
                mainTex.a *= edgeFade;

                fixed maskTex = tex2D(_MaskTex, i.uvMask).r;

                fixed4 color = mainTex * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                color.a *= maskTex;

                float nearClip = _ProjectionParams.y;
                float farClip  = _ProjectionParams.z;
                float depth = i.viewDepth;

                float nearFade = saturate((depth - nearClip) / _NearFadeDistance);
                float farFade  = saturate((farClip - depth) / _FarFadeDistance);

                color.a *= nearFade * farFade;

                return color;
            }
            ENDCG
        }
    }
}