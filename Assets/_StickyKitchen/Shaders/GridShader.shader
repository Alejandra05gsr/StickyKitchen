
Shader "Instanced/GridTestParticleShader_URP" {
    Properties {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Color("Color", Color) = (1, 0, 0.5, 1)
        _Size("Particle Size", Float) = 1.0
        _DensityRange("Density Range", Range(0,500000)) = 1.0
    }
    SubShader {
        Tags { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                uint instanceID   : SV_InstanceID;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Smoothness;
                float _Metallic;
                float _Size;
                float _DensityRange;
            CBUFFER_END

            struct Particle {
                float pressure;
                float density;
                float3 currentForce;
                float3 velocity;
                float3 position;
            };

            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                StructuredBuffer<Particle> _particlesBuffer;
            #endif

            void setup() {
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                    float3 pos = _particlesBuffer[unity_InstanceID].position;
                    float size = _Size;

                    // Matriz de transformación Object -> World
                    unity_ObjectToWorld = float4x4(
                        size, 0,    0,    pos.x,
                        0,    size, 0,    pos.y,
                        0,    0,    size, pos.z,
                        0,    0,    0,    1
                    );

                    // Matriz inversa World -> Object
                    unity_WorldToObject = float4x4(
                        1.0f / size, 0,           0,           -pos.x / size,
                        0,           1.0f / size, 0,           -pos.y / size,
                        0,           0,           1.0f / size, -pos.z / size,
                        0,           0,           0,            1
                    );
                #endif
            }

            Varyings vert(Attributes input) {
                Varyings output;
                
                UNITY_SETUP_INSTANCE_ID(input);

                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                    setup();
                #endif

                // Transformaciones de posición y normales a World Space / Clip Space
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                // Cálculo básico de iluminación PBR (URP Lighting)
                InputData inputData = (InputData)0;
                inputData.normalWS = normalize(input.normalWS);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = _Color.rgb * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.alpha = _Color.a;

                // Iluminación estándar de URP
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}