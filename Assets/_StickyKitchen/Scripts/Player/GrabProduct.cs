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

    [Header("Referencias del Sistema")]
    public OrderManager orderManager; // Referencia al OrderManager de la escena
    public Score score;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (productGrabbed == null && nearbyProduct != null)
            {
                Grab(nearbyProduct);
            }
            else if (productGrabbed != null)
            {
                Drop();
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
        if (rb != null) rb.isKinematic = true;

        Collider[] colliders = productGrabbed.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) col.enabled = false;

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
        foreach (Collider col in colliders) col.enabled = true;

        Rigidbody rb = productGrabbed.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

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

        // Obtenemos la orden activa actual
        Order currentOrder = orderManager.GetCurrentOrder();

        if (currentOrder != null)
        {
            // Intentamos entregar el ingrediente a la orden actual
            bool wasNeeded = currentOrder.TryDeliverIngredient(idGrabbedProduct);

            if (wasNeeded)
            {
                if (score != null) score.AddPoints(productPoints);
            }
        }

        // Destruimos o deshabilitamos el objeto entregado
        Destroy(productGrabbed);
        productGrabbed = null;

        if (dropText != null) dropText.gameObject.SetActive(false);
    }
}
