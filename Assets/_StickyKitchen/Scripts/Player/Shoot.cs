using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public enum FluidType { Water, Syrup }

    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bulletSpawnPoint; // Punta del cañón de la pistola

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

    // Propiedades públicas para UI u otros componentes
    public int CurrentWaterAmmo => currentWaterAmmo;
    public int CurrentSyrupAmmo => currentSyrupAmmo;
    public FluidType CurrentFluid => currentFluid;

    private void Start()
    {
        ReloadAllAmmo();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
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
                Debug.Log("Desaparece Jarabe");
            }

            // Seleccionar prefabs según el tipo de fluido cargado
            GameObject projectileToInstantiate;
            GameObject meshToInstantiate;

            if (currentFluid == FluidType.Water)
            {
                projectileToInstantiate = waterProjectilePrefab != null ? waterProjectilePrefab : waterPrefab;
                meshToInstantiate = waterPrefab;
            }
            else
            {
                projectileToInstantiate = syrupProjectilePrefab != null ? syrupProjectilePrefab : syrupPrefab;
                meshToInstantiate = syrupPrefab;
            }

            CreateFluid(projectileToInstantiate, meshToInstantiate, hit);
            ConsumeAmmo();
        }
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
            Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);
        }
        else
        {
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

        projectileScript.SetupProjectile(hit.point, hit.normal, gunTransform, meshPrefab, currentFluid);
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