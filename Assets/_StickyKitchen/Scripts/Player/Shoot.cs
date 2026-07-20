using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Shoot : MonoBehaviour
{
    public float raycastDistance;
    public LayerMask layerMask;
    public LayerMask layerMask1;
    public LayerMask layerMask2;
    public Transform gunTransform;

    public GameObject fluidObjectType1;
    public GameObject fluidObjectType2;

    // Update is called once per frame
    void Update()
    {
        ShootGun();
    }

    void CheckRaycast()
    {
        RaycastHit hit;
        Vector3 origin = gunTransform.transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance, layerMask))
        {
            Debug.Log("Disparo");
            if(hit.collider.gameObject.layer == 6) 
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                Debug.DrawLine(origin, hit.point, Color.green);
                AppearFluidType1(hit.point);
            }
            else if (hit.collider.gameObject.layer == 7)
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                Debug.DrawLine(origin, hit.point, Color.green);
                AppearFluidType2(hit.point);
            }


        }
        else
        {
            Debug.DrawLine(origin, origin + direction * raycastDistance, Color.red);
        }

    }

    void ShootGun()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRaycast();
        }
    }


    void AppearFluidType1(Vector3 hitPosition)
    {
        Instantiate(fluidObjectType1, hitPosition, fluidObjectType1.transform.rotation);
    }

    void AppearFluidType2(Vector3 hitPosition)
    {
        Instantiate(fluidObjectType2, hitPosition, fluidObjectType2.transform.rotation);
    }

}
