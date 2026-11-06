using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class FluidRayMarching : MonoBehaviour
{
    [System.Serializable]
    public class FluidLayer
    {
        public SPH sph;
        public Material fluidMaterial;

        public Color color = new Color(0.02f, 0.45f, 0.85f, 0.85f);

        [Min(0.001f)] public float viewRadius = 0.18f;
        [Min(0f)] public float blendStrength = 0.5f;
    }

    public FluidLayer[] fluids = new FluidLayer[] { new FluidLayer(), new FluidLayer() };

    public Color ambientLight = new Color(0.2f, 0.25f, 0.3f, 1f);
    public Light lightSource;

    [Range(32, 256)] public int maxSteps = 96;
    [Min(0.1f)] public float maxDistance = 100f;

    private Camera cam;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        RenderPipelineManager.beginCameraRendering += PrepareMaterials;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PrepareMaterials;

        if (fluids == null) return;

        foreach (FluidLayer fluid in fluids)
        {
            if (fluid != null && fluid.fluidMaterial != null)
            {
                fluid.fluidMaterial.SetInteger("_ParticleCount", 0);
            }
        }
    }

    private void PrepareMaterials(ScriptableRenderContext context, Camera renderingCamera)
    {
        if (fluids == null) return;

        foreach (FluidLayer fluid in fluids)
        {
            if (fluid == null || fluid.fluidMaterial == null) continue;

            Material mat = fluid.fluidMaterial;
            SPH sim = fluid.sph;

            if (!Application.isPlaying || renderingCamera != cam || sim == null || !sim.isActiveAndEnabled || !sim.IsReady)
            {
                mat.SetInteger("_ParticleCount", 0);
                continue;
            }

            mat.SetBuffer("_FluidParticles", sim._particlesBuffer);
            mat.SetInteger("_ParticleCount", sim.ActiveParticleCount);

            mat.SetFloat("_ParticleRadius", Mathf.Max(0.001f, fluid.viewRadius));
            mat.SetFloat("_BlendStrength", Mathf.Max(0f, fluid.blendStrength));
            mat.SetColor("_WaterColor", fluid.color);
            mat.SetColor("_AmbientLight", ambientLight);

            mat.SetInteger("_MaxSteps", Mathf.Clamp(maxSteps, 32, 256));
            mat.SetFloat("_MaxDistance", Mathf.Max(0.1f, maxDistance));

            Vector3 padding = Vector3.one * (Mathf.Max(0.001f, fluid.viewRadius) + Mathf.Max(0f, fluid.blendStrength) + 0.01f);
            Bounds bounds = sim.SimulationBounds;

            mat.SetVector("_FluidBoundsMin", bounds.min - padding);
            mat.SetVector("_FluidBoundsMax", bounds.max + padding);

            bool customLight = lightSource != null && lightSource.isActiveAndEnabled;
            mat.SetFloat("_UseCustomLight", customLight ? 1f : 0f);

            if (customLight)
            {
                mat.SetVector("_FluidLightDirection", -lightSource.transform.forward);
                mat.SetColor("_FluidLightColor", lightSource.color * lightSource.intensity);
            }
        }
    }
}