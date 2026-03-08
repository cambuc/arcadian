Shader "Custom/FoliageFade"
{
    Properties
    {
        _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        LOD 200

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade addshadow
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = c.rgb;
            o.Alpha = c.a;

            #ifdef LOD_FADE_CROSSFADE
                UNITY_APPLY_DITHER_CROSSFADE(IN.screenPos);
            #endif
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}