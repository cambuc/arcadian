Shader "Custom/FlowingWater"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed (Y)", Float) = 0.2
        _StepInterval ("Step Interval (Seconds)", Float) = 0.1

        _ShallowColor ("Shallow Color", Color) = (0.7,0.9,1,0.4)
        _DeepColor ("Deep Color", Color) = (0.0,0.3,0.6,0.9)
        _DepthMax ("Max Depth For Full Opaque", Float) = 2.0

        _FoamColor ("Foam Color (RGBA)", Color) = (1,1,1,1)
        _FoamDistance ("Foam Distance", Float) = 0.3
        _FoamStrength ("Foam Strength", Float) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma require depthtexture
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float4 _MainTex_ST;
            float _ScrollSpeed;
            float _StepInterval;

            float4 _ShallowColor;
            float4 _DeepColor;
            float _DepthMax;

            float4 _FoamColor;
            float _FoamDistance;
            float _FoamStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);

                float steppedTime = floor(_Time.y / _StepInterval) * _StepInterval;
                uv.y += steppedTime * _ScrollSpeed;

                o.uv = uv;
                o.screenPos = ComputeScreenPos(o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float sceneDepth = LinearEyeDepth(
                    SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos))
                );

                float waterDepth = LinearEyeDepth(i.screenPos.z / i.screenPos.w);
                float depthDiff = max(sceneDepth - waterDepth, 0);

                float depth01 = saturate(depthDiff / _DepthMax);

                // Depth-based water color
                fixed4 waterColor = lerp(_ShallowColor, _DeepColor, depth01);
                waterColor.rgb *= tex.rgb;

                // -------- Foam --------
                float foamMask = saturate(1 - depthDiff / _FoamDistance);
                foamMask = pow(foamMask, _FoamStrength);

                // Apply foam color using its own alpha
                fixed4 foam = _FoamColor;
                foam.a *= foamMask;

                // Combine water + foam correctly
                fixed4 finalColor;

                // RGB blend
                finalColor.rgb = lerp(waterColor.rgb, foam.rgb, foam.a);

                // Alpha blend (foam overrides water alpha)
                finalColor.a = saturate(waterColor.a + foam.a);

                return finalColor;
            }

            ENDCG
        }
    }
}
