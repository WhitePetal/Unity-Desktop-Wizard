// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SkyeBox("Sky", Cube) = "_SkyBox"{}
        _Cubemap("Environment Map", Cube) = "_SkyBox"{}
        _RefractAmount("Refarcat Amount", Range(-1, 1)) = 1
        _ReflectAmount("Relfect Amount", Range(0, 1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        GrabPass{"_ScreenTex"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent: TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 offset : TEXCOORD1;
                float4 scrPos : TEXCOORD2;
                float3 reflectUV : TEXCOORD3;
                float4 worldPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cubemap;
            float _RefractAmount;
            float _ReflectAmount;
            sampler2D _ScreenTex;
            samplerCUBE _SkyeBox;

            v2f vert (appdata v)
            {
                v2f o;
                float4 pos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = pos;
                float3 wordlNormal = UnityObjectToWorldNormal(v.normal);
                wordlNormal = normalize(wordlNormal);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeGrabScreenPos(o.vertex);
                o.uv = v.uv;
                float3 viewDir = _WorldSpaceCameraPos - pos;
                o.reflectUV = -reflect(viewDir, wordlNormal);
                // o.reflectUV = unitobje
                o.offset = refract(viewDir, wordlNormal, _RefractAmount);
                o.offset = UnityWorldToObjectDir(o.offset);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 reflect = texCUBE(_Cubemap, i.reflectUV) * _ReflectAmount;
                fixed4 refract = tex2D(_ScreenTex, i.scrPos / i.scrPos.w + i.offset.xy) * 0.2;
                return reflect + refract;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
