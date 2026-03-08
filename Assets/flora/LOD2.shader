Shader "Custom/LOD2"
{
    Properties
    {
        _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow alphatest:_Cutoff vertex:vert
        #pragma target 3.0
        #pragma multi_compile_instancing
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;        // Needed for dithering
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        half _Metallic;
        half _Glossiness;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            UNITY_SETUP_INSTANCE_ID(v);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            UNITY_SETUP_INSTANCE_ID(IN);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            #ifdef LOD_FADE_CROSSFADE
                UNITY_APPLY_DITHER_CROSSFADE(IN.screenPos);
            #endif
        }
        ENDCG
    }

    FallBack "Standard"
}
