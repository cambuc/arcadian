Shader "Custom/GPUInstancedBillboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.3
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200
        Cull Off
        ZWrite On

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float _Cutoff;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;   // Needed for crossfade
                UNITY_FOG_COORDS(3)
            };

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;

                float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                float3 objectScale;
                objectScale.x = length(unity_ObjectToWorld._m00_m10_m20);
                objectScale.y = length(unity_ObjectToWorld._m01_m11_m21);
                objectScale.z = length(unity_ObjectToWorld._m02_m12_m22);

                float3 camPos = _WorldSpaceCameraPos;

                float3 forward = normalize(camPos - worldCenter);
                forward.y = 0;

                float3 right = normalize(float3(forward.z, 0, -forward.x));
                float3 up = float3(0,1,0);

                float3 local = v.vertex.xyz;

                float3 worldPos =
                    worldCenter +
                    right * (local.x * objectScale.x) +
                    up    * (local.y * objectScale.y);

                o.pos = UnityWorldToClipPos(worldPos);
                o.uv = v.uv;

                o.worldNormal = forward;

                // Required for LOD crossfade
                o.screenPos = ComputeScreenPos(o.pos);

                UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;

                clip(tex.a - _Cutoff);

                #ifdef LOD_FADE_CROSSFADE
                    UNITY_APPLY_DITHER_CROSSFADE(i.screenPos);
                #endif

                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float NdotL = max(0, dot(normal, lightDir));
                fixed3 diffuse = tex.rgb * _LightColor0.rgb * NdotL;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * tex.rgb;

                fixed4 finalColor = fixed4(diffuse + ambient, tex.a);

                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                return finalColor;
            }

            ENDCG
        }
    }
}
