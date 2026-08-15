using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Shoot : MonoBehaviour
{
    public enum FluidType { Water, Syrup }


    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform cameraTransform;



    [Header("Fluid Prefabs")]
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject syrupPrefab;
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
    [SerializeField] private float minFillValue = -1f; // Si 0 es la mitad, pon aquí el valor cuando está vacío (ej. -0.5 o -1)
    [SerializeField] private float maxFillValue = 1f;  // El valor cuando está completamente lleno (ej. 0.5 o 1)


    [Header("State")]
    [SerializeField] private FluidType currentFluid = FluidType.Water;



    // Propiedades públicas útiles para UI o scripts externos

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
            GameObject prefabToInstantiate = (currentFluid == FluidType.Water) ? waterPrefab : syrupPrefab;
            CreateFluid(prefabToInstantiate, hit);
            ConsumeAmmo();
        }
    }

    private bool CheckRaycast(out RaycastHit hit)
    {
        Vector3 origin = gunTransform != null ? gunTransform.position : transform.position;
        Vector3 direction = cameraTransform != null ? cameraTransform.forward : transform.forward;

        bool hasHit = Physics.Raycast(origin, direction, out hit, raycastDistance, layerMask);

        if (hasHit)
        {
            Debug.DrawLine(origin, hit.point, Color.green, 2f);
        }
        else
        {
            Debug.DrawLine(origin, origin + direction * raycastDistance, Color.red, 0.5f);
        }

        return hasHit;
    }

    private void CreateFluid(GameObject prefab, RaycastHit hit)
    {
        if (prefab == null) return;

        Vector3 spawnPosition = hit.point + hit.normal * 0.02f;
        Vector3 forward = Vector3.ProjectOnPlane(gunTransform != null ? gunTransform.forward : transform.forward, hit.normal).normalized;

        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(hit.normal, Vector3.up);
        }


        Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);
        Instantiate(prefab, spawnPosition, rotation);
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
            // Calculamos el porcentaje (0.0 a 1.0)
            float pct = (float)currentWaterAmmo / maxWaterAmmo;
            // Lo mapeamos al rango real del shader (minFillValue a maxFillValue)
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

        // Al recargar, asignamos el valor máximo del shader
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
        waterShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentWaterShader);
        syrupShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentSyrupShader);
    }

}



