using UnityEngine;
using static UnityEngine.UI.Image;

public class Shoot : MonoBehaviour
{
    public float raycastDistance;
    public LayerMask layerMask;
    public Transform gunTransform;

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
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.DrawLine(origin, hit.point, Color.green);
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


}
