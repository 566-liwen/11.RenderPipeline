Shader "Unlit/ColorTestShader"
{
    Properties
    {
    }
    SubShader
    {   
        

        //Name "DEFERRED"

        Pass
        {
            //Name "ObjectA"
            Tags { "LightMode"="ObjectA" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2ff
            {
                float4 vertex : SV_POSITION;
            };

            v2ff vert (appdata v)
            {
                v2ff o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

			void frag (v2ff i,
            out half4 GRT0:SV_Target0,
            out half4 GRT1:SV_Target1,
            out float GRTDepth:SV_Depth
            )
			{                
				GRT0 = float4(1,0,0,0);
				GRT1 = float4(0,1,0,0);
                GRTDepth = 0.5;
			}
            ENDCG
        }

        Pass
        {
            //Name "ObjectB"
            Tags { "LightMode"="ObjectB" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2ff
            {
                float4 vertex : SV_POSITION;
            };

            v2ff vert (appdata v)
            {
                v2ff o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

			void frag (v2ff i,
            out half4 GRT2:SV_Target2,
            out float GRTDepth:SV_Depth
            )
			{                
                GRT2 = float4(0,0,0.5,0);
                GRTDepth = 0.5;
			}
            ENDCG
        }

        Pass
        {
            //Name "ObjectB"
            Tags { "LightMode"="ObjectC" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2ff
            {
                float4 vertex : SV_POSITION;
            };

            v2ff vert (appdata v)
            {
                v2ff o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

			void frag (v2ff i,
            out half4 GRT3:SV_Target3,
            out float GRTDepth:SV_Depth
            )
			{                
                GRT3 = float4(0,0,0.5,0);
                GRTDepth = 0.5;
			}
            ENDCG
        }
    }
}