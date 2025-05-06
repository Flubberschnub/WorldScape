Shader "Custom/HeightmapTextureShader"
{
    Properties
    {
        _LowTex ("Low Texture", 2D) = "blue" {}
        _MidTex ("Mid Texture", 2D) = "green" {}
        _HighTex ("High Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_LowTex); SAMPLER(sampler_LowTex);
            TEXTURE2D(_MidTex); SAMPLER(sampler_MidTex);
            TEXTURE2D(_HighTex); SAMPLER(sampler_HighTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                float fogFactor : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(worldPos);
                output.uv = input.uv;
                output.height = input.color.r;
                output.fogFactor = ComputeFogFactor(output.positionHCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 lowTex = SAMPLE_TEXTURE2D(_LowTex, sampler_LowTex, input.uv);
                half4 midTex = SAMPLE_TEXTURE2D(_MidTex, sampler_MidTex, input.uv);
                half4 highTex = SAMPLE_TEXTURE2D(_HighTex, sampler_HighTex, input.uv);

                half4 color = lerp(lowTex, midTex, smoothstep(0.35, 0.55, input.height));
                color = lerp(color, highTex, smoothstep(0.65, 0.85, input.height));
                color.rgb = MixFog(color.rgb, input.fogFactor); // Apply URP fog

                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}
