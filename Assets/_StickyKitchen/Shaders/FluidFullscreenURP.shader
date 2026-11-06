Shader "StickyKitchen/FluidFullscreenURP"
{
    Properties
    {
        [HideInInspector] _ParticleCount("Particle Count", Integer) = 0
        [HideInInspector] _ParticleRadius("Particle Radius", Float) = 0.13
        [HideInInspector] _BlendStrength("Blend Strength", Float) = 0.1
        [HideInInspector] _WaterColor("Water Color", Color) = (0.02, 0.45, 0.85, 0.85)
        [HideInInspector] _AmbientLight("Ambient Light", Color) = (0.2, 0.25, 0.3, 1)
        [HideInInspector] _MaxSteps("Max Steps", Integer) = 96
        [HideInInspector] _MaxDistance("Max Distance", Float) = 100
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "FluidFullscreen"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM

            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct FluidParticle
            {
                float pressure;
                float density;
                float3 currentForce;
                float3 velocity;
                float3 position;
            };

            StructuredBuffer<FluidParticle> _FluidParticles;

            CBUFFER_START(UnityPerMaterial)
                int _ParticleCount;
                float _ParticleRadius;
                float _BlendStrength;
                int _MaxSteps;
                float4 _WaterColor;
                float4 _AmbientLight;
                float4 _FluidBoundsMin;
                float4 _FluidBoundsMax;
                float _MaxDistance;
                float _UseCustomLight;
                float4 _FluidLightDirection;
                float4 _FluidLightColor;
            CBUFFER_END

            float SmoothUnion(float a, float b)
            {
                if (_BlendStrength <= 0.00001) return min(a, b);

                float h = saturate(0.5 + 0.5 * (b - a) / _BlendStrength);
                return lerp(b, a, h) - _BlendStrength * h * (1.0 - h);
            }

            float FluidDistance(float3 pointWS)
            {
                float result = 1e10;

                [loop]
                for (int i = 0; i < _ParticleCount; i++)
                {
                    float distanceToParticle = length(pointWS - _FluidParticles[i].position) - _ParticleRadius;
                    result = SmoothUnion(result, distanceToParticle);
                }

                return result;
            }

            float3 FluidNormal(float3 pointWS)
            {
                float epsilon = max(0.0001, _ParticleRadius * 0.01);
                float2 e = float2(1, -1) * epsilon;

                float3 gradient = e.xyy * FluidDistance(pointWS + e.xyy)
                    + e.yyx * FluidDistance(pointWS + e.yyx)
                    + e.yxy * FluidDistance(pointWS + e.yxy)
                    + e.xxx * FluidDistance(pointWS + e.xxx);

                return gradient * rsqrt(max(dot(gradient, gradient), 1e-20));
            }

            bool IntersectBounds(float3 origin, float3 direction, out float entry, out float exit)
            {
                entry = 0;
                exit = _MaxDistance;

                [unroll]
                for (int axis = 0; axis < 3; axis++)
                {
                    if (abs(direction[axis]) < 0.000001)
                    {
                        if (origin[axis] < _FluidBoundsMin[axis] || origin[axis] > _FluidBoundsMax[axis]) return false;
                    }
                    else
                    {
                        float a = (_FluidBoundsMin[axis] - origin[axis]) / direction[axis];
                        float b = (_FluidBoundsMax[axis] - origin[axis]) / direction[axis];

                        entry = max(entry, min(a, b));
                        exit = min(exit, max(a, b));
                    }
                }

                return exit > entry;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 source = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                if (_ParticleCount <= 0) return source;

                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
                float rawDepth = SampleSceneDepth(screenUV);

                #if UNITY_REVERSED_Z
                    float nearDepth = 1.0;
                    float farDepth = 0.00001;
                    bool hasGeometry = rawDepth > 0.000001;
                #else
                    float nearDepth = UNITY_NEAR_CLIP_VALUE;
                    float farDepth = 0.99999;
                    bool hasGeometry = rawDepth < 0.999999;
                    rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawDepth);
                #endif

                float3 nearWS = ComputeWorldSpacePosition(screenUV, nearDepth, UNITY_MATRIX_I_VP);
                float3 farWS = ComputeWorldSpacePosition(screenUV, farDepth, UNITY_MATRIX_I_VP);
                float3 origin = unity_OrthoParams.w > 0.5 ? nearWS : _WorldSpaceCameraPos;
                float3 direction = normalize(farWS - origin);
                float maxDepth = _MaxDistance;

                if (hasGeometry)
                {
                    float3 sceneWS = ComputeWorldSpacePosition(screenUV, rawDepth, UNITY_MATRIX_I_VP);
                    maxDepth = max(0, dot(sceneWS - origin, direction));
                }

                float entry, exit;
                if (!IntersectBounds(origin, direction, entry, exit)) return source;

                entry = max(entry, length(nearWS - origin));
                exit = min(exit, maxDepth);

                float traveled = entry;
                float epsilon = max(0.0001, _ParticleRadius * 0.01);

                [loop]
                for (int step = 0; step < _MaxSteps && traveled < exit; step++)
                {
                    float3 pointWS = origin + direction * traveled;
                    float distanceToFluid = FluidDistance(pointWS);

                    if (distanceToFluid <= epsilon)
                    {
                        float3 normalWS = FluidNormal(pointWS);
                        if (dot(normalWS, direction) > 0) normalWS = -normalWS;

                        Light mainLight = GetMainLight();

                        float3 lightDirection = _UseCustomLight > 0.5 ? normalize(_FluidLightDirection.xyz) : mainLight.direction;
                        half3 lightColor = _UseCustomLight > 0.5 ? _FluidLightColor.rgb : mainLight.color;

                        float3 viewDirection = -direction;
                        float3 halfDirection = SafeNormalize(lightDirection + viewDirection);

                        float diffuse = saturate(dot(normalWS, lightDirection));
                        float specular = pow(saturate(dot(normalWS, halfDirection)), 96.0) * diffuse;
                        float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirection)), 5.0);

                        half3 water = _WaterColor.rgb * (_AmbientLight.rgb + lightColor * diffuse * 0.65);
                        water += lightColor * specular * 0.8;
                        water += half3(0.25, 0.55, 0.8) * fresnel * 0.35;

                        half opacity = saturate(_WaterColor.a + fresnel * 0.15);
                        return half4(lerp(source.rgb, water, opacity), source.a);
                    }

                    traveled += max(distanceToFluid, epsilon * 0.5);
                }

                return source;
            }

            ENDHLSL
        }
    }
}