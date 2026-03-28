Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalRenderPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _OutlineColor;
            float _OutlineSize;
            float4 _MainTex_TexelSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            float SampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float alpha = SampleAlpha(i.uv);

                // If pixel exists, draw normally
                if (alpha > 0.01)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                }

                // Outline detection
                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float outline = 0;
                outline += SampleAlpha(i.uv + float2( offset.x, 0));
                outline += SampleAlpha(i.uv + float2(-offset.x, 0));
                outline += SampleAlpha(i.uv + float2(0,  offset.y));
                outline += SampleAlpha(i.uv + float2(0, -offset.y));

                if (outline > 0)
                {
                    return _OutlineColor;
                }

                return float4(0,0,0,0);
            }

            ENDHLSL
        }
    }
}