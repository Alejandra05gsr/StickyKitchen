Shader "Custom/GridParticle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

        struct Particle
        {
            float3 position;
            float3 velocity;
            float3 force;
            float density;
            float pressure;
        };

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            StructuredBuffer<Particle> _Particles;
            float _Size;
        #endif

        void setup()
        {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                Particle p = _Particles[unity_InstanceID];
                unity_ObjectToWorld._11_21_31_41 = float4(_Size, 0, 0, 0);
                unity_ObjectToWorld._12_22_32_42 = float4(0, _Size, 0, 0);
                unity_ObjectToWorld._13_23_33_43 = float4(0, 0, _Size, 0);
                unity_ObjectToWorld._14_24_34_44 = float4(p.position, 1);
            #endif
        }

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}