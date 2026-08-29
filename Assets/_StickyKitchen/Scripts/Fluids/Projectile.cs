using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Velocidad de Vuelo")]
    public float speed = 50f;

    private Vector3 targetPoint;
    private Vector3 hitNormal;
    private Transform gunTransform;
    private GameObject finalMeshPrefab;
    private Shoot.FluidType fluidType;
    private bool initialized;

    public void SetupProjectile(Vector3 target, Vector3 normal, Transform gun, GameObject meshPrefab, Shoot.FluidType type)
    {
        targetPoint = target;
        hitNormal = normal;
        gunTransform = gun;
        finalMeshPrefab = meshPrefab;
        fluidType = type;
        initialized = true;

        transform.LookAt(targetPoint);
    }

    private void Update()
    {
        if (!initialized) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            SpawnFinalMesh();
            Destroy(gameObject);
        }
    }

    private void SpawnFinalMesh()
    {
        // 1. Limpieza de Jarabe si el proyectil es de Agua
        if (fluidType == Shoot.FluidType.Water)
        {
            Collider[] hitColliders = Physics.OverlapSphere(targetPoint, 0.5f);
            foreach (var col in hitColliders)
            {
                if (col.CompareTag("Syrup") || col.GetComponent<FluidArea>()?.areaType == FluidArea.FluidAreaType.Syrup)
                {
                    col.GetComponent<FluidArea>()?.DisapearFluid();
                }
            }
        }

        if (finalMeshPrefab == null) return;

        // 2. Posicionamiento de la malla sobre la superficie
        Vector3 spawnPosition = targetPoint + hitNormal * 0.02f;
        Vector3 forward = Vector3.ProjectOnPlane(gunTransform != null ? gunTransform.forward : transform.forward, hitNormal).normalized;
        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(hitNormal, Vector3.up);
        }

        Quaternion rotation = Quaternion.LookRotation(forward, hitNormal);
        Instantiate(finalMeshPrefab, spawnPosition, rotation);
    }
}