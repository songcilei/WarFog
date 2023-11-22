Shader "Unlit/KawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

            CGINCLUDE
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _BlurOffset;
            float _LerpSpeed;

            sampler2D _OldMainTex;
            float4 _OldMainTex_ST;
            

            sampler2D _SourTex;
            float4 _SourTex_ST;
            
/*         URP==============================================>
            half4 KawaseBlur(TEXTURE2D_PARAM(tex, samplerTex), float2 uv, float2 texelSize, half pixelOffset)
	        {
		        half4 o = 0;
		        o += SAMPLE_TEXTURE2D(tex, samplerTex, uv + float2(pixelOffset +0.5, pixelOffset +0.5) * texelSize); 
		        o += SAMPLE_TEXTURE2D(tex, samplerTex, uv + float2(-pixelOffset -0.5, pixelOffset +0.5) * texelSize); 
		        o += SAMPLE_TEXTURE2D(tex, samplerTex, uv + float2(-pixelOffset -0.5, -pixelOffset -0.5) * texelSize); 
		        o += SAMPLE_TEXTURE2D(tex, samplerTex, uv + float2(pixelOffset +0.5, -pixelOffset -0.5) * texelSize); 
		        return o * 0.25;
	        }
*/
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 FragBoxBlur (v2f i) : SV_Target
            {
                half4 o = 0;
		        o += tex2D(_MainTex,i.uv + float2(_BlurOffset +0.5, _BlurOffset +0.5) * _MainTex_TexelSize.xy); 
		        o += tex2D(_MainTex,i.uv + float2(-_BlurOffset -0.5, _BlurOffset +0.5) * _MainTex_TexelSize.xy); 
		        o += tex2D(_MainTex,i.uv + float2(-_BlurOffset -0.5, -_BlurOffset -0.5) * _MainTex_TexelSize.xy); 
		        o += tex2D(_MainTex,i.uv + float2(_BlurOffset +0.5, -_BlurOffset -0.5) * _MainTex_TexelSize.xy); 
		        return o * 0.25;
                
            }

            fixed4 FragCombine(v2f i):SV_Target
            {
                float blurTex = tex2D(_SourTex,i.uv);
                float oldTex = tex2D(_OldMainTex,i.uv);
                return lerp(oldTex,blurTex,_LerpSpeed);
            }
        
        
        
            ENDCG
        
        Pass
        {
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragBoxBlur

            ENDCG
        }
        
        Pass
        {
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragCombine

            ENDCG
        }
    }
}
