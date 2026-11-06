using UnityEngine;

public class FluidImpactEmitter : MonoBehaviour
{
    [SerializeField] private SPH waterSimulation;
    [SerializeField] private SPH syrupSimulation;

    [SerializeField, Min(1)] private int particlesPerImpact = 27;
    [SerializeField, Min(0f)] private float surfaceSpeed = 1f;
    [SerializeField, Min(0f)] private float outwardSpeed = 0.5f;

    public int Emit(Shoot.FluidType type, RaycastHit hit, Vector3 shotOrigin)
    {
        SPH simulation = type == Shoot.FluidType.Water ? waterSimulation : syrupSimulation;

        if (simulation == null || !simulation.isActiveAndEnabled || !simulation.IsReady)
        {
            Debug.LogWarning("Falta una simulacion SPH activa para " + type, this);
            return 0;
        }

        Vector3 incoming = (hit.point - shotOrigin).normalized;
        Vector3 alongSurface = Vector3.ProjectOnPlane(incoming, hit.normal);
        Vector3 velocity = alongSurface * surfaceSpeed + hit.normal * outwardSpeed;

        return simulation.Emit(hit.point, hit.normal, velocity, particlesPerImpact);
    }
}