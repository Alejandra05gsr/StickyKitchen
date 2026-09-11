using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public enum FluidType { Water, Syrup }

    [Header("UI & Aim Settings")]
    [SerializeField] private GameObject crosshairUI;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bulletSpawnPoint; 

    [Header("Projectile Prefabs (Balas Voladoras)")]
    [SerializeField] private GameObject waterProjectilePrefab; // Esfera de agua voladora
    [SerializeField] private GameObject syrupProjectilePrefab; // Esfera de jarabe voladora

    [Header("Fluid Mesh Prefabs (Mallas/Charcos)")]
    [SerializeField] private GameObject waterPrefab; // Malla/Charco de agua
    [SerializeField] private GameObject syrupPrefab; // Malla/Charco de jarabe
    [SerializeField] private GameObject waterShader;
    [SerializeField] private GameObject syrupShader;

    [Header("Water Ammo")]
    private int currentWaterAmmo;
    [SerializeField] private int maxWaterAmmo = 10;
    private float currentWaterShader;
    [SerializeField] public float maxWaterShader = 1f;

    [Header("Syrup Ammo")]
    private int currentSyrupAmmo;
    [SerializeField] private int maxSyrupAmmo = 10;
    private float currentSyrupShader;
    [SerializeField] public float maxSyrupShader = 1f;

    [Header("Shader Range Adjustments")]
    [SerializeField] private float minFillValue = -1f;
    [SerializeField] private float maxFillValue = 1f;

    [Header("State")]
    [SerializeField] private FluidType currentFluid = FluidType.Water;
    private bool isAiming = false;

    // Propiedades públicas para UI u otros componentes
    public int CurrentWaterAmmo => currentWaterAmmo;
    public int CurrentSyrupAmmo => currentSyrupAmmo;
    public FluidType CurrentFluid => currentFluid;

    private void Start()
    {
        ReloadAllAmmo();
        if (crosshairUI != null)
        {
            crosshairUI.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Verificar si se mantiene presionado el clic derecho (1)
        isAiming = Input.GetMouseButton(1);

        // Controlar la visibilidad de la cruz según el estado de apuntado
        if (crosshairUI != null)
        {
            crosshairUI.SetActive(isAiming);
        }

        // Solo permite disparar con clic izquierdo (0) SI se está apuntando con clic derecho
        if (isAiming && Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFluidType();
        }
    }

    private void TryShoot()
    {
        if (!HasAmmoForCurrentFluid())
        {
            Debug.Log("Sin munición para " + currentFluid);
            return;
        }

        if (CheckRaycast(out RaycastHit hit))
        {
            // Interacciones directas por Raycast
            if (currentFluid == FluidType.Syrup && hit.collider.CompareTag("Interactuable"))
            {
                hit.collider.gameObject.GetComponent<InteractableDoors>()?.InteractFurniture();
                hit.collider.gameObject.GetComponent<Oven>()?.InteractOven();
            }

            if (currentFluid == FluidType.Syrup && hit.collider.CompareTag("Oven"))
            {
                hit.collider.gameObject.GetComponent<Oven>()?.InteractOven();
            }

            if (currentFluid == FluidType.Water && hit.collider.CompareTag("Syrup"))
            {
                hit.collider.gameObject.GetComponent<FluidArea>()?.DisapearFluid();
                //Debug.Log("Desaparece Jarabe");
            }

            // 2. Seleccionar prefabs
            GameObject projectileToInstantiate = (currentFluid == FluidType.Water) ? waterProjectilePrefab : syrupProjectilePrefab;
            GameObject meshToInstantiate = (currentFluid == FluidType.Water) ? waterPrefab : syrupPrefab;

            // 3. ¡INSTANTÁNEO! Crear el charco y pegarlo al mueble INMEDIATAMENTE
            SpawnFluidMeshInstantly(meshToInstantiate, hit);

            // 4. (Opcional) Instanciar la bala voladora solo como efecto estético
            if (projectileToInstantiate != null)
            {
                CreateVisualFXProjectile(projectileToInstantiate, hit.point);
            }

            ConsumeAmmo();
        }
    }
    // Método que pega el charco AL INSTANTE en el mueble
    private void SpawnFluidMeshInstantly(GameObject meshPrefab, RaycastHit hit)
    {
        if (meshPrefab == null) return;

        // Si es agua, limpiar jarabes cercanos de inmediato
        if (currentFluid == FluidType.Water)
        {
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, 0.5f);
            foreach (var col in hitColliders)
            {
                if (col.CompareTag("Syrup") || col.GetComponent<FluidArea>()?.areaType == FluidArea.FluidAreaType.Syrup)
                {
                    col.GetComponent<FluidArea>()?.DisapearFluid();
                }
            }
        }

        // Calcular posición y rotación pegada a la superficie
        Vector3 spawnPosition = hit.point + hit.normal * 0.02f;
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform != null ? cameraTransform.forward : transform.forward, hit.normal).normalized;
        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(hit.normal, Vector3.up);
        }

        Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);

        // Instanciar charco en el frame exacto del clic
        GameObject spawnedFluid = Instantiate(meshPrefab, spawnPosition, rotation);

        // EMPARENTAR INMEDIATAMENTE si es un mueble/objeto móvil
        if (hit.collider != null && (hit.collider.CompareTag("Interactuable") || hit.collider.CompareTag("Oven")))
        {
            spawnedFluid.transform.SetParent(hit.collider.transform, true);
            Debug.Log("Charco emparentado instantáneamente a: " + hit.collider.name);
        }
    }

    // Método simple para lanzar la bala como puro adorno visual (opcional)
    private void CreateVisualFXProjectile(GameObject projectilePrefab, Vector3 targetPoint)
    {
        Vector3 spawnPosition = bulletSpawnPoint != null ? bulletSpawnPoint.position :
                               (gunTransform != null ? gunTransform.position : transform.position);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        // Un script simple en el proyectil visual que solo lo mueva rápido al punto y se destruya
        projectile.transform.LookAt(targetPoint);
        Destroy(projectile, 0.2f); // Se borra a los 0.2 segundos
    }
    private bool CheckRaycast(out RaycastHit hit)
    {
        Ray ray = Camera.main != null ?
            Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)) :
            new Ray(cameraTransform != null ? cameraTransform.position : transform.position,
                    cameraTransform != null ? cameraTransform.forward : transform.forward);

        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask finalMask = layerMask & ~(1 << playerLayer);

        bool hasHit = Physics.Raycast(ray, out hit, raycastDistance, finalMask);

        if (hasHit)
        {

            Color rayColor = Color.green;

            if (hit.collider.CompareTag("Syrup"))
            {
                rayColor = new Color(0.6f, 0.3f, 0f); // Marrón / Naranja para Jarabe
            }
            else if (hit.collider.CompareTag("Water"))
            {
                rayColor = Color.cyan; // Azul para Agua
            }
            else if (hit.collider.CompareTag("Interactuable") || hit.collider.CompareTag("Oven"))
            {
                rayColor = Color.yellow; // Amarillo para objetos interactuables
            }

            // Dibuja la línea desde la cámara/arma hasta el punto exacto de impacto
            Debug.DrawLine(ray.origin, hit.point, rayColor, 1.5f);

            // Opcional: Pinta una pequeña cruz roja en el punto exacto donde va a spawnear el charco
            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.red, 1.5f);
        }
        else
        {
            // Línea roja hasta la distancia máxima si no apunta a nada
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.red, 0.5f);
        }

        return hasHit;
    }

    private void CreateFluid(GameObject projectilePrefab, GameObject meshPrefab, RaycastHit hit)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = bulletSpawnPoint != null ? bulletSpawnPoint.position :
                               (gunTransform != null ? gunTransform.position : transform.position);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<Projectile>();
        }

        // Le pasamos hit.collider.transform como el objeto impactado
        projectileScript.SetupProjectile(hit.point, hit.normal, gunTransform, meshPrefab, currentFluid, hit.collider.transform);
    }

    private bool HasAmmoForCurrentFluid()
    {
        return currentFluid switch
        {
            FluidType.Water => currentWaterAmmo > 0,
            FluidType.Syrup => currentSyrupAmmo > 0,
            _ => false
        };
    }

    private void ConsumeAmmo()
    {
        if (currentFluid == FluidType.Water)
        {
            currentWaterAmmo -= 1;
            float pct = (float)currentWaterAmmo / maxWaterAmmo;
            currentWaterShader = Mathf.Lerp(minFillValue, maxFillValue, pct);
        }
        else if (currentFluid == FluidType.Syrup)
        {
            currentSyrupAmmo -= 1;
            float pct = (float)currentSyrupAmmo / maxSyrupAmmo;
            currentSyrupShader = Mathf.Lerp(minFillValue, maxFillValue, pct);
        }

        UpdateShaders();
    }

    public void ToggleFluidType()
    {
        currentFluid = (currentFluid == FluidType.Water) ? FluidType.Syrup : FluidType.Water;
        Debug.Log("Fluido cambiado a: " + currentFluid);
    }

    public void ReloadAllAmmo()
    {
        currentWaterAmmo = maxWaterAmmo;
        currentSyrupAmmo = maxSyrupAmmo;

        currentWaterShader = maxFillValue;
        currentSyrupShader = maxFillValue;

        UpdateShaders();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterAmmo"))
        {
            currentWaterAmmo = maxWaterAmmo;
            currentWaterShader = maxFillValue;
            UpdateShaders();
        }
        else if (other.CompareTag("SyrupAmmo"))
        {
            currentSyrupAmmo = maxSyrupAmmo;
            currentSyrupShader = maxFillValue;
            UpdateShaders();
        }
    }

    void UpdateShaders()
    {
        if (waterShader != null) waterShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentWaterShader);
        if (syrupShader != null) syrupShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentSyrupShader);
    }
}