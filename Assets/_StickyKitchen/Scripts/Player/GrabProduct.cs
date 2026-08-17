using TMPro;
using UnityEngine;

public class GrabProduct : MonoBehaviour
{
    public Transform playerGrabTransform;

    public GameObject productGrabbed;
    private GameObject nearbyProduct;

    public TextMeshProUGUI grabText;
    public TextMeshProUGUI dropText;

    private int idGrabbedProduct;
    private int productPoints;

    public GameObject order;

    public Score score;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (productGrabbed == null && nearbyProduct != null)
            {
                Grab(nearbyProduct);
                grabText.gameObject.SetActive(false);
                dropText.gameObject.SetActive(true);
            }
            else if (productGrabbed != null)
            {
                Drop();
                dropText.gameObject.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11 && productGrabbed == null)
        {
            if (grabText != null) grabText.gameObject.SetActive(true);
            nearbyProduct = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == nearbyProduct)
        {
            if (grabText != null) grabText.gameObject.SetActive(false);
            nearbyProduct = null;
        }
    }


    private void Grab(GameObject targetProduct)
    {
        productGrabbed = targetProduct;

        Product prodScript = productGrabbed.GetComponent<Product>();
        if (prodScript != null)
        {
            idGrabbedProduct = prodScript.idProduct;
            productPoints = prodScript.points;
        }


        Rigidbody rb = productGrabbed.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders = productGrabbed.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }


        productGrabbed.transform.position = playerGrabTransform.position;
        productGrabbed.transform.rotation = playerGrabTransform.rotation;
        productGrabbed.transform.SetParent(playerGrabTransform);

        if (grabText != null) grabText.gameObject.SetActive(false);
        if (dropText != null) dropText.gameObject.SetActive(true);

        nearbyProduct = null;
    }


    public void Drop()
    {
        if (productGrabbed == null) return;

        productGrabbed.transform.SetParent(null);

        Collider[] colliders = productGrabbed.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        Rigidbody rb = productGrabbed.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        productGrabbed = null;
        if (dropText != null) dropText.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bowl") && productGrabbed != null)
        {
            DeliverProduct();
        }
    }

    public void DeliverProduct()
    {
        if (productGrabbed == null) return;
        
        productGrabbed.GetComponent<Product>().TypeOfProduct(idGrabbedProduct, productPoints);
        Debug.Log("Producto entregado: " + productGrabbed.name + " con ID: " + idGrabbedProduct + ", Puntos: " + productPoints);

        //Visualización de interfaz de pedidos
        switch (idGrabbedProduct)
        {
            case 0:
                order.gameObject.GetComponent<Order>().TurnOffProduct01();
                break;
            case 1:
                order.gameObject.GetComponent<Order>().TurnOffProduct02();
                break;
            case 2:
                order.gameObject.GetComponent<Order>().TurnOffProduct03();
                break;
        }

        
        //Se llama a la función de Score para ir sumando los puntos
        score.GetComponent<Score>().AddPoints(productPoints);


        //Se quita el objeto del jugador
        Rigidbody rb = productGrabbed.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        productGrabbed.transform.SetParent(null);
        productGrabbed.SetActive(false);
        productGrabbed = null;
        dropText.gameObject.SetActive(false);
    }

}
