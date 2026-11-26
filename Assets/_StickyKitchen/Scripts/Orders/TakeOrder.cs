using UnityEngine;

public class TakeOrder : MonoBehaviour
{
    [Header("Textos de UI")]
    public GameObject takeOrder; 
    public GameObject grabText; 
    public GameObject dropText; 

    [Header("Órdenes del Canvas")]
    public GameObject muffinCanvasUI;  
    public GameObject cookiesCanvasUI;

    private string currentOrderTag = "";

    public bool canTakeOrder = true; // Variable para controlar si se puede tomar la orden


    void Update()
    {
        // Si estamos dentro de una orden y presionamos Q
        if (currentOrderTag != "" && Input.GetKeyDown(KeyCode.Q))
        {
            canTakeOrder = false; // Desactivamos la posibilidad de tomar otra orden hasta que se reinicie
            TakingOrder();
        }
    }



    public void TakingOrder()
    {
        // Solo toma la orden si el texto de aviso está activo
        if (takeOrder.activeInHierarchy)
        {
            if (currentOrderTag == "OrderMuffin")
            {
                muffinCanvasUI.SetActive(true);
            }
            else if (currentOrderTag == "OrderCookies")
            {
                cookiesCanvasUI.SetActive(true);
            }

            // Ocultamos el aviso y limpiamos la orden actual para evitar tomarla dos veces seguidas
            takeOrder.SetActive(false);
            currentOrderTag = "";
        }
    }

    public void AppearText() // Activa el texto de agarrar pedido
    {
        if (!grabText.activeInHierarchy && !dropText.activeInHierarchy)
        {
            takeOrder.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Guardamos el tag del objeto con el que chocamos
        if (other.CompareTag("OrderMuffin") || other.CompareTag("OrderCookies") && canTakeOrder)
        {
            currentOrderTag = other.tag;
            AppearText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si salimos del trigger de la orden actual, reiniciamos todo
        if (other.CompareTag("OrderMuffin") || other.CompareTag("OrderCookies"))
        {
            currentOrderTag = "";
            takeOrder.SetActive(false);
        }
    }

    public void ResetOrderTaking()
    {
        canTakeOrder = true;
    }

}
