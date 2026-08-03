using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Shoot : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float raycastDistance = 20f;
    public LayerMask layerMask;
    public Transform gunTransform;
    public Transform cameraTransform;

    [Header("Fluid Prefabs")]
    public GameObject Range01;
    public GameObject Range02;

    [Header("Ammo")]
    public int maxWaterAmmo;
    public int actualWaterAmmo;
    public int maxSyrupAmmo;
    public int actualSyrupAmmo;

    public bool canShoot;

    public int actualFluid;
    public int idWater;
    public int idSyrup;


    private void Start()
    {
        ReloadWaterAmmo();
        ReloadSyrupAmmo();
       
    }


    // Update is called once per frame
    void Update()
    {
        ShootGun();
    }


    void ShootGun()
    {
        //Si se aprieta el boton y can shoot es true se dispara
        if (Input.GetMouseButtonDown(0))
        {
            //Checar arma
            //Checar municion
            CheckRaycast();

            actualWaterAmmo -= 1;
        }
    }

    void CheckRaycast()
    {
        RaycastHit hit;

        Vector3 origin = gunTransform.position;
        Vector3 direction = cameraTransform.forward;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance, layerMask))
        {
            //Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.DrawLine(origin, hit.point, Color.green, 2f);

            switch (hit.collider.gameObject.layer)
            {
                case 6:
                    CreateFluid(Range01, hit);
                    break;

                case 7:
                    CreateFluid(Range02, hit);
                    break;
            }
        }
        else
        {
            Debug.DrawLine(origin, origin + direction * raycastDistance, Color.red);
        }

    }



    void AppearFluidType1(RaycastHit hit)
    {
        CreateFluid(Range01, hit);
    }

    void AppearFluidType2(RaycastHit hit)
    {
        CreateFluid(Range02, hit);
    }


    void CreateFluid(GameObject prefab, RaycastHit hit)
    {
        Vector3 spawnPosition = hit.point + hit.normal * 0.02f;

        Vector3 forward = Vector3.ProjectOnPlane(gunTransform.forward, hit.normal).normalized;

        Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);

        GameObject fluid = Instantiate(prefab, spawnPosition, rotation);
    }


    void ReloadWaterAmmo()
    {
        actualWaterAmmo = maxWaterAmmo;
    }

    void ReloadSyrupAmmo()
    {
        actualSyrupAmmo = maxSyrupAmmo;
    }

    public void CanShoot()
    {
        //Primero checa el tipo de arma

        //Checa si hay munición suficiente



    }

    public void ChangeTypeOfFluid()
    {
        //Si se le da click a la E entonces cambia 
    }


}
