using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Shoot : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float raycastDistance = 20f;
    public LayerMask layerMask;
    public Transform gunTransform;

    [Header("Fluid Prefabs")]
    public GameObject fluidObjectType1;
    public GameObject fluidObjectType2;



    // Update is called once per frame
    void Update()
    {
        ShootGun();
    }


    void ShootGun()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRaycast();
        }
    }

    void CheckRaycast()
    {
        RaycastHit hit;

        Vector3 origin = gunTransform.transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance, layerMask))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.DrawLine(origin, hit.point, Color.green);

            switch (hit.collider.gameObject.layer)
            {
                case 6:
                    CreateFluid(fluidObjectType1, hit);
                    break;

                case 7:
                    CreateFluid(fluidObjectType2, hit);
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
        CreateFluid(fluidObjectType1, hit);
    }

    void AppearFluidType2(RaycastHit hit)
    {
        CreateFluid(fluidObjectType2, hit);
    }


    void CreateFluid(GameObject prefab, RaycastHit hit)
    {
        Vector3 spawnPosition = hit.point + hit.normal * 0.02f;

        Vector3 forward = Vector3.ProjectOnPlane(gunTransform.forward, hit.normal).normalized;

        Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);

        GameObject fluid = Instantiate(prefab, spawnPosition, rotation);
    }





}
