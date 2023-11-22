Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_WarFogMap("_warFogMap",2D) = "black"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            
            Cull back
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint vertexID :SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            sampler2D _WarFogMap;
            float4 _WarFogMap_ST;

            float4 GetFullScreenTriangleVertexPosition(uint vertexID, float z = UNITY_NEAR_CLIP_VALUE)
            {
                // note: the triangle vertex position coordinates are x2 so the returned UV coordinates are in range -1, 1 on the screen.
                float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
                return float4(uv * 2.0 - 1.0, z, 1.0);
            }


            float2 GetFullScreenTriangleTexCoord(uint vertexID)
            {
                #if UNITY_UV_STARTS_AT_TOP
                return float2((vertexID << 1) & 2, 1.0 - (vertexID & 2));
                #else
                return float2((vertexID << 1) & 2, vertexID & 2);
                #endif
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = GetFullScreenTriangleVertexPosition(v.vertexID);

                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 fogMap = (tex2D(_WarFogMap,i.uv.xy));
                return fixed4(0,0,0,1-fogMap.r);
            }
            ENDCG
        }
    }
}
