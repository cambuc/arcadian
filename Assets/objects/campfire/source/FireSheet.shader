Shader "Custom/FireSheet"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Rows ("Rows", Float) = 4
        _Columns ("Columns", Float) = 4
        _Speed ("Animation Speed (FPS)", Float) = 12
        _Step ("Frame Step", Int) = 1
        _Color ("Tint Color (RGBA controls fade)", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Rows;
            float _Columns;
            float _Speed;
            int _Step;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float totalFrames = _Rows * _Columns;

                // Base frame index
                float baseFrame = floor(_Time.y * _Speed);

                // Apply stepping
                float steppedFrame = baseFrame * max(_Step, 1);

                // Wrap safely
                float currentFrame = fmod(steppedFrame, totalFrames);

                // Determine row and column
                float row = floor(currentFrame / _Columns);
                float col = fmod(currentFrame, _Columns);

                // Adjust UVs
                float2 uv = i.uv;
                uv.x = (uv.x + col) / _Columns;
                uv.y = (uv.y + (_Rows - 1 - row)) / _Rows;

                fixed4 tex = tex2D(_MainTex, uv);
                return tex * _Color;
            }
            ENDCG
        }
    }
}