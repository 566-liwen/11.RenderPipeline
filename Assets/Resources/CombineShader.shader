Shader "Unlit/CombineShader"
{
   Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {

            ZTest Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            Texture2D _GBuffer0;
            Texture2D _GBuffer1;
            Texture2D _GBuffer2;
            Texture2D _GBuffer3;

            SamplerState sampler_pointer_clamp;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2ff
            {
                float4 vertex : SV_POSITION;
                 float2 uv : TEXCOORD0;
            };

            struct GBufferOutput
            {
                half4 GBuffer4 : SV_Target4;
            };

            v2ff vert (appdata v)
            {
                v2ff o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            bool compareTwoColors(half4 x, half4 y) {
                if (x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a)  return true;
                return false;
            }

			half4 frag (v2ff i): SV_Target
			{            
                half4 g0 = _GBuffer0.Sample(sampler_pointer_clamp, i.uv);
                half4 g1 = _GBuffer1.Sample(sampler_pointer_clamp, i.uv);
                half4 g2 = _GBuffer2.Sample(sampler_pointer_clamp, i.uv);
                half4 g3 = _GBuffer3.Sample(sampler_pointer_clamp, i.uv);
               // half4 col = g0 + g1; // color from background (deep blue)
                //col.a = g0.a;
                //return lerp(g2, g1, g0);

                // "masking" background color by checking if the pixel is in same color
                if (compareTwoColors(g0, g1) && compareTwoColors(g0, g2) && compareTwoColors(g0, g3)) {
                    return g0;
                } else {
                    return g0 + g1 + g2 +g3;
                }
			}

            ENDCG
        }
    }
}
