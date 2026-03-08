Shader "Custom/FoliageCutout"
{
    Properties
    {
        _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        [Toggle] _UseWind ("Enable Wind", Float) = 1
        _WindStrength ("Wind Strength", Range(0,1)) = 0.2
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindScale ("Wind Scale", Float) = 1.0

        _WindHeightOffset ("Wind Height Offset", Float) = 0.0
        _WindHeightRange ("Wind Height Range", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }

        LOD 200
        Cull Off
        ZWrite On
        AlphaToMask On

        CGPROGRAM
        #pragma surface surf Lambert alphatest:_Cutoff addshadow vertex:vert
        #pragma target 3.0
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        sampler2D _MainTex;
        fixed4 _Color;

        float _UseWind;
        float _WindStrength;
        float _WindSpeed;
        float _WindScale;
        float _WindHeightOffset;
        float _WindHeightRange;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            if (_UseWind > 0.5)
            {
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float wave = sin(worldPos.x * _WindScale + _Time.y * _WindSpeed)
                           + cos(worldPos.z * _WindScale + _Time.y * _WindSpeed);

                // Shift pivot upward by offset
                float adjustedHeight = (v.vertex.y - _WindHeightOffset);

                // Remap height into 0–1 range
                float heightMask = saturate(adjustedHeight / max(_WindHeightRange, 0.0001));

                float windOffset = wave * _WindStrength * heightMask;

                v.vertex.x += windOffset;
                v.vertex.z += windOffset * 0.5;
            }
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = tex.rgb;
            o.Alpha = tex.a;

            #ifdef LOD_FADE_CROSSFADE
                UNITY_APPLY_DITHER_CROSSFADE(IN.screenPos);
            #endif
        }
        ENDCG
    }

    FallBack "Transparent/Cutout/VertexLit"
}
