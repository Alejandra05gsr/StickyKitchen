using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 44)]
public struct Particle
{
    public float pressure;
    public float density;
    public Vector3 currentForce;
    public Vector3 velocity;
    public Vector3 position;
}

public class SPH : MonoBehaviour
{
    [Header("Capacidad")]
    [Min(1)] public int maxParticles = 256;
    public bool startWithParticles;
    public Vector3Int numToSpawn = new Vector3Int(5, 5, 5);
    public Vector3 spawnCenter;

    [Header("Dominio en coordenadas de mundo")]
    public Vector3 boxCenter;
    public Vector3 boxSize = new Vector3(4, 10, 3);
    [Min(0.001f)] public float particleRadius = 0.1f;
    public Transform collisionSphere;

    [Header("Depuracion")]
    public bool showSpheres;
    public Mesh particleMesh;
    public Material material;
    public float particleRenderSize = 0.1f;

    [Header("Simulacion")]
    public ComputeShader shader;
    public float boundDamping = -0.3f;
    public float viscosity = -0.003f;
    public float particleMass = 1f;
    public float gasConstant = 2f;
    public float restingDensity = 1f;
    public float timestep = 0.001f;

    public ComputeBuffer _particlesBuffer;
    public int ActiveParticleCount { get; private set; }

    public Bounds SimulationBounds => new Bounds(boxCenter, boxSize);
    public bool IsReady => runtimeShader != null && _particlesBuffer != null && _particlesBuffer.IsValid();

    private ComputeShader runtimeShader;
    private ComputeBuffer argsBuffer;
    private Particle[] upload;
    private readonly uint[] args = new uint[5];
    private MaterialPropertyBlock properties;

    private int writeIndex;
    private int densityKernel;
    private int forceKernel;
    private int integrateKernel;

    private void Awake()
    {
        if (shader == null || !SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("SPH necesita un Compute Shader asignado y soporte de compute shaders.", this);
            enabled = false;
            return;
        }

        int capacity = Mathf.Max(1, maxParticles);
        upload = new Particle[capacity];

        _particlesBuffer = new ComputeBuffer(capacity, 44);
        _particlesBuffer.SetData(upload);

        // Cada liquido necesita su propia instancia del Compute Shader.
        runtimeShader = Instantiate(shader);

        densityKernel = runtimeShader.FindKernel("ComputeDensityPressure");
        forceKernel = runtimeShader.FindKernel("ComputeForces");
        integrateKernel = runtimeShader.FindKernel("Integrate");

        runtimeShader.SetBuffer(densityKernel, "_particles", _particlesBuffer);
        runtimeShader.SetBuffer(forceKernel, "_particles", _particlesBuffer);
        runtimeShader.SetBuffer(integrateKernel, "_particles", _particlesBuffer);

        if (particleMesh != null && material != null)
        {
            material.enableInstancing = true;
            properties = new MaterialPropertyBlock();

            args[0] = particleMesh.GetIndexCount(0);
            args[2] = particleMesh.GetIndexStart(0);
            args[3] = particleMesh.GetBaseVertex(0);

            argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBuffer.SetData(args);
        }

        if (startWithParticles)
        {
            Emit(spawnCenter, Vector3.up, Vector3.zero, numToSpawn.x * numToSpawn.y * numToSpawn.z);
        }
    }

    public int Emit(Vector3 point, Vector3 normal, Vector3 velocity, int amount)
    {
        if (!isActiveAndEnabled || !IsReady || amount <= 0) return 0;

        int requested = Mathf.Min(amount, upload.Length);
        int side = 1;

        while (side * side * side < requested) side++;

        Vector3 n = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.up;
        Vector3 tangent = Vector3.Cross(n, Mathf.Abs(n.y) < 0.95f ? Vector3.up : Vector3.right).normalized;
        Vector3 bitangent = Vector3.Cross(n, tangent);

        float radius = Mathf.Max(0.001f, particleRadius);
        float spacing = radius * 2f;

        Vector3 min = SimulationBounds.min + Vector3.one * radius;
        Vector3 max = SimulationBounds.max - Vector3.one * radius;

        int count = 0;

        for (int i = 0; i < requested; i++)
        {
            float x = i % side - (side - 1) * 0.5f;
            float z = i / side % side - (side - 1) * 0.5f;
            int y = i / (side * side);

            Vector3 position = point + n * (radius + 0.02f + y * spacing) + (tangent * x + bitangent * z) * spacing;

            if (position.x < min.x || position.y < min.y || position.z < min.z || position.x > max.x || position.y > max.y || position.z > max.z) continue;

            upload[count++] = new Particle
            {
                position = position,
                velocity = velocity
            };
        }

        if (count == 0)
        {
            Debug.LogWarning("El impacto o sus particulas quedan fuera de Box Center / Box Size.", this);
            return 0;
        }

        // Copiar solamente las particulas nuevas.
        int first = Mathf.Min(count, upload.Length - writeIndex);

        _particlesBuffer.SetData(upload, 0, writeIndex, first);

        if (count > first)
        {
            _particlesBuffer.SetData(upload, first, 0, count - first);
        }

        writeIndex = (writeIndex + count) % upload.Length;
        ActiveParticleCount = Mathf.Min(upload.Length, ActiveParticleCount + count);

        if (argsBuffer != null)
        {
            args[1] = (uint)ActiveParticleCount;
            argsBuffer.SetData(args);
        }

        return count;
    }

    private void FixedUpdate()
    {
        if (!IsReady || ActiveParticleCount == 0) return;

        float radius = Mathf.Max(0.001f, particleRadius);

        runtimeShader.SetInt("particleLength", ActiveParticleCount);
        runtimeShader.SetFloat("particleMass", Mathf.Max(0.0001f, particleMass));
        runtimeShader.SetFloat("viscosity", viscosity);
        runtimeShader.SetFloat("gasConstant", gasConstant);
        runtimeShader.SetFloat("restDensity", restingDensity);
        runtimeShader.SetFloat("boundDamping", boundDamping);
        runtimeShader.SetFloat("pi", Mathf.PI);

        runtimeShader.SetFloat("radius", radius);
        runtimeShader.SetFloat("radius2", radius * radius);
        runtimeShader.SetFloat("radius3", Mathf.Pow(radius, 3));
        runtimeShader.SetFloat("radius4", Mathf.Pow(radius, 4));
        runtimeShader.SetFloat("radius5", Mathf.Pow(radius, 5));

        runtimeShader.SetFloat("timestep", timestep);
        runtimeShader.SetVector("boxCenter", boxCenter);
        runtimeShader.SetVector("boxSize", boxSize);

        runtimeShader.SetVector("spherePos", collisionSphere != null ? collisionSphere.position : Vector3.zero);
        runtimeShader.SetFloat("sphereRadius", collisionSphere != null ? Mathf.Abs(collisionSphere.lossyScale.x) * 0.5f : 0f);

        int groups = Mathf.CeilToInt(ActiveParticleCount / 100f);

        runtimeShader.Dispatch(densityKernel, groups, 1, 1);
        runtimeShader.Dispatch(forceKernel, groups, 1, 1);
        runtimeShader.Dispatch(integrateKernel, groups, 1, 1);
    }

    private void Update()
    {
        if (!showSpheres || !IsReady || ActiveParticleCount == 0 || argsBuffer == null || material == null) return;

        properties.SetFloat("_Size", particleRenderSize);
        properties.SetBuffer("_particlesBuffer", _particlesBuffer);

        Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, SimulationBounds, argsBuffer, 0, properties, ShadowCastingMode.Off);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    private void OnDestroy()
    {
        argsBuffer?.Release();
        _particlesBuffer?.Release();
        _particlesBuffer = null;

        if (runtimeShader != null) Destroy(runtimeShader);
    }
}