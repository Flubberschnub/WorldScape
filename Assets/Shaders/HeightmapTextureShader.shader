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
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _LowTex;
            sampler2D _MidTex;
            sampler2D _HighTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.height = v.color.r; // Use red channel as height
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 lowTex = tex2D(_LowTex, i.uv);
                fixed4 midTex = tex2D(_MidTex, i.uv);
                fixed4 highTex = tex2D(_HighTex, i.uv);

                fixed4 color = lerp(lowTex, midTex, smoothstep(0.35, 0.55, i.height));
                color = lerp(color, highTex, smoothstep(0.65, 0.85, i.height));

                return color;
            }
            ENDCG
        }
    }
}