using UnityEngine;

public class GrabProduct : MonoBehaviour
{
    bool isShooting;
    bool haveProduct;
    bool canGrab;

    public Transform playerGrabTransform;

    //public GameObject productGrabbed;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            canGrab = true;

        }

    }

    public void GrabbingProduct(Collision collision)
    {
        if (!canGrab) return;
        if (Input.GetKeyDown(KeyCode.R)) //Input.GetMouseButtonDown(0) mejor con click para poder moverse libremente
        {
            //Si no hay collision entonces es nulo
            collision.gameObject.transform.position = playerGrabTransform.transform.position;
            collision.gameObject.transform.SetParent(this.transform);

        }

    }


    //void DropProduct()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ToggleFluidType();
    //    }
    //}



}
