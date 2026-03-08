Shader "Custom/SimpleSun"
{
    Properties
    {
        _DayColor ("Day Sky Color", Color) = (0.5,0.7,1,1)
        _SunsetColor ("Sunset Color", Color) = (1,0.4,0.2,1)
        _NightColor ("Night Color", Color) = (0.02,0.02,0.08,1)

        _SunColor ("Sun Color", Color) = (1,0.95,0.8,1)
        _SunSize ("Sun Size", Range(0.001,0.1)) = 0.02
        _SunSoftness ("Sun Softness", Range(0.0001,0.05)) = 0.01

        _SunPixelSize ("Sun Pixel Size", Range(0.0005,0.02)) = 0.003
        _SunGradientSteps ("Sun Gradient Steps", Range(2,16)) = 6
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _DayColor;
            float4 _SunsetColor;
            float4 _NightColor;

            float4 _SunColor;
            float _SunSize;
            float _SunSoftness;
            float _SunPixelSize;
            float _SunGradientSteps;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.dir = normalize(worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.dir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // Build tangent basis around the sun direction
                float3 up = abs(lightDir.y) > 0.99 ? float3(1,0,0) : float3(0,1,0);
                float3 right = normalize(cross(up, lightDir));
                float3 forward = cross(lightDir, right);

                // Project view direction into sun plane
                float2 sunUV;
                sunUV.x = dot(viewDir, right);
                sunUV.y = dot(viewDir, forward);

                // Pixelate
                sunUV = floor(sunUV / _SunPixelSize) * _SunPixelSize;

                float dist = length(sunUV);

                // Soft edge
                float sun = smoothstep(_SunSize + _SunSoftness, _SunSize, dist);

                // Retro stepped gradient
                sun = floor(sun * _SunGradientSteps) / _SunGradientSteps;

                float sunHeight = lightDir.y;

                float dayFactor = saturate(sunHeight * 0.5 + 0.5);

                float sunsetFactor = 1 - abs(sunHeight) * 5;
                sunsetFactor = saturate(sunsetFactor);

                float3 sky = lerp(_NightColor.rgb, _DayColor.rgb, dayFactor);
                sky = lerp(sky, _SunsetColor.rgb, sunsetFactor);
                
                float sunVisibility = saturate(sunHeight * 10); // fades near horizon
                float3 col = sky + _SunColor.rgb * sun * sunVisibility;

                return float4(col, 1);
            }
            ENDCG
        }
    }
}