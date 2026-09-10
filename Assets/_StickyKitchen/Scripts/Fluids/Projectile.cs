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
    private Transform hitTransform;
    private bool initialized;

    public void SetupProjectile(Vector3 target, Vector3 normal, Transform gun, GameObject meshPrefab, Shoot.FluidType type, Transform hitObjTransform = null)
    {
        targetPoint = target;
        hitNormal = normal;
        gunTransform = gun;
        finalMeshPrefab = meshPrefab;
        fluidType = type;
        hitTransform = hitObjTransform;
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

        Vector3 spawnPosition = targetPoint + hitNormal * 0.02f;
        Vector3 forward = Vector3.ProjectOnPlane(gunTransform != null ? gunTransform.forward : transform.forward, hitNormal).normalized;
        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(hitNormal, Vector3.up);
        }

        Quaternion rotation = Quaternion.LookRotation(forward, hitNormal);

        // Instanciamos y guardamos la referencia de la malla creada
        GameObject spawnedFluid = Instantiate(finalMeshPrefab, spawnPosition, rotation);

        // 3. Emparentamiento dinámico si golpea un objeto móvil o interactuable
        if (hitTransform != null && (hitTransform.CompareTag("Interactuable") || hitTransform.CompareTag("Oven")))
        {
            spawnedFluid.transform.SetParent(hitTransform, true);
        }
    }
}