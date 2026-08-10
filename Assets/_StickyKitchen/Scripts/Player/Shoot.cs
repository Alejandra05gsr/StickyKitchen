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


    [Header("Object Pooling Settings")]
    [SerializeField] private int initialPoolSize = 15;
    private List<GameObject> waterPool = new List<GameObject>();
    private List<GameObject> syrupPool = new List<GameObject>();


    [Header("Water Ammo")]
    private int currentWaterAmmo;
    [SerializeField] private int maxWaterAmmo = 10;
    private float currentWaterShader;
    [SerializeField] public float maxWaterShader = 1f;
    //[SerializeField] public float waterShaderFillSpeed = 0.5f;



    [Header("Syrup Ammo")]
    private int currentSyrupAmmo;
    [SerializeField] private int maxSyrupAmmo = 10;
    private float currentSyrupShader;
    [SerializeField] public float maxSyrupShader = 1f;
    //[SerializeField] public float syrupShaderFillSpeed = 0.5f;



    [Header("State")]
    [SerializeField] private FluidType currentFluid = FluidType.Water;



    // Propiedades públicas útiles para UI o scripts externos

    public int CurrentWaterAmmo => currentWaterAmmo;

    public int CurrentSyrupAmmo => currentSyrupAmmo;

    public FluidType CurrentFluid => currentFluid;




    private void Start()
    {
        //LLenamos al máximo los shaders de agua y jarabe
        currentWaterShader = maxWaterShader;
        currentSyrupShader = maxSyrupShader;
        UpdateShaders();

        //Recargamos toda la munición al inicio del juego
        ReloadAllAmmo();
    }


    private void Update()
    {
        //Inputs de disparo y cambio de fluido
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
        //Checa si hay munición para el fluido actual antes de disparar
        if (!HasAmmoForCurrentFluid())
        {
            Debug.Log("Sin munición para " + currentFluid);
            return;
        }

        //Realiza un raycast para determinar dónde disparar el fluido
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
            currentWaterShader -= 0.2f;
        }
        else if (currentFluid == FluidType.Syrup)
        {
            currentSyrupAmmo -= 1;
            currentSyrupShader -= 0.2f;
        }
        //Update del ammo y shader 
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
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterAmmo"))
        {
            currentWaterAmmo = maxWaterAmmo;
            currentWaterShader = maxWaterShader;
            UpdateShaders();
        }
        else if (other.CompareTag("SyrupAmmo"))
        {
            currentSyrupAmmo = maxSyrupAmmo;
            currentSyrupShader = maxSyrupShader;
            UpdateShaders();
        }

    }


    void UpdateShaders()
    {
        waterShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentWaterShader);
        syrupShader.GetComponent<Renderer>().material.SetFloat("_Fill", currentSyrupShader);
    }

}



