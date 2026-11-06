using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class FluidRayMarching : MonoBehaviour
{
    public Camera cam;
    public SPH sph;
    public Material fluidMaterial;

    [Header("Superficie")]
    [Min(0.001f)] public float viewRadius = 0.15f;
    [Min(0f)] public float blendStrength = 0.12f;
    public Color waterColor = new Color(0.02f, 0.45f, 0.85f, 0.85f);
    public Color ambientLight = new Color(0.2f, 0.25f, 0.3f, 1f);
    public Light lightSource;

    [Header("Ray marching")]
    [Range(32, 256)] public int maxSteps = 96;
    [Min(0.1f)] public float maxDistance = 100f;

    private static readonly int ParticleCountId = Shader.PropertyToID("_ParticleCount");
    private static readonly int ParticlesId = Shader.PropertyToID("_FluidParticles");
    private static readonly int RadiusId = Shader.PropertyToID("_ParticleRadius");
    private static readonly int BlendId = Shader.PropertyToID("_BlendStrength");
    private static readonly int ColorId = Shader.PropertyToID("_WaterColor");
    private static readonly int AmbientId = Shader.PropertyToID("_AmbientLight");
    private static readonly int BoundsMinId = Shader.PropertyToID("_FluidBoundsMin");
    private static readonly int BoundsMaxId = Shader.PropertyToID("_FluidBoundsMax");
    private static readonly int MaxStepsId = Shader.PropertyToID("_MaxSteps");
    private static readonly int MaxDistanceId = Shader.PropertyToID("_MaxDistance");
    private static readonly int CustomLightId = Shader.PropertyToID("_UseCustomLight");
    private static readonly int LightDirectionId = Shader.PropertyToID("_FluidLightDirection");
    private static readonly int LightColorId = Shader.PropertyToID("_FluidLightColor");

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        RenderPipelineManager.beginCameraRendering += PrepareMaterial;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PrepareMaterial;

        if (fluidMaterial != null) fluidMaterial.SetInteger(ParticleCountId, 0);
    }

    private void PrepareMaterial(ScriptableRenderContext context, Camera renderingCamera)
    {
        if (fluidMaterial == null) return;

        if (!Application.isPlaying || renderingCamera != cam || sph == null || !sph.isActiveAndEnabled || sph._particlesBuffer == null || !sph._particlesBuffer.IsValid())
        {
            fluidMaterial.SetInteger(ParticleCountId, 0);
            return;
        }

        float radius = Mathf.Max(0.001f, viewRadius);
        float blend = Mathf.Max(0f, blendStrength);

        fluidMaterial.SetBuffer(ParticlesId, sph._particlesBuffer);
        fluidMaterial.SetInteger(ParticleCountId, sph._particlesBuffer.count);
        fluidMaterial.SetFloat(RadiusId, radius);
        fluidMaterial.SetFloat(BlendId, blend);
        fluidMaterial.SetColor(ColorId, waterColor);
        fluidMaterial.SetColor(AmbientId, ambientLight);
        fluidMaterial.SetInteger(MaxStepsId, Mathf.Clamp(maxSteps, 32, 256));
        fluidMaterial.SetFloat(MaxDistanceId, Mathf.Max(0.1f, maxDistance));

        Vector3 spawnEnd = sph.spawnCenter + new Vector3(Mathf.Max(0, sph.numToSpawn.x - 1), Mathf.Max(0, sph.numToSpawn.y - 1), Mathf.Max(0, sph.numToSpawn.z - 1)) * (2f * sph.particleRadius);
        float jitter = Mathf.Abs(sph.particleRadius * sph.spawnJitter);

        Vector3 boundsMin = Vector3.Min(-sph.boxSize * 0.5f, Vector3.Min(sph.spawnCenter, spawnEnd) - Vector3.one * jitter);
        Vector3 boundsMax = Vector3.Max(sph.boxSize * 0.5f, Vector3.Max(sph.spawnCenter, spawnEnd) + Vector3.one * jitter);
        Vector3 padding = Vector3.one * (radius + blend + 0.01f);

        fluidMaterial.SetVector(BoundsMinId, boundsMin - padding);
        fluidMaterial.SetVector(BoundsMaxId, boundsMax + padding);

        bool useCustomLight = lightSource != null && lightSource.isActiveAndEnabled;
        fluidMaterial.SetFloat(CustomLightId, useCustomLight ? 1f : 0f);

        if (useCustomLight)
        {
            fluidMaterial.SetVector(LightDirectionId, -lightSource.transform.forward);
            fluidMaterial.SetColor(LightColorId, lightSource.color * lightSource.intensity);
        }
    }
}