using UnityEngine;
using UnityEngine.SceneManagement; // Necesario si la pantalla de victoria es otra escena

public class StartOrder : MonoBehaviour
{
    [Header("Órdenes de la UI")]
    public GameObject muffinOrder;
    public GameObject cookiesOrder;

    [Header("Progreso y Victoria")]
    public int ordersToWin = 3;         
    public GameObject victoryScreenUI;   
    private int completedOrdersCount = 0;

    public bool startMuffins;
    private bool startCookies;

    [Header("Prefaba de entrega")]
    public GameObject plate;
    public GameObject muffin;
    public GameObject cookies;


    private bool hasPlate = false;
    private bool hasMuffinFood = false;
    private bool hasCookiesFood = false;

    void Update()
    {
        CheckOrder();
        VerifyDelivery();
    }

    public void CheckOrder()
    {
        if (muffinOrder.activeInHierarchy)
        {
            startMuffins = true;
            startCookies = false;
        }
        else if (cookiesOrder.activeInHierarchy)
        {
            startCookies = true;
            startMuffins = false;
        }
        else
        {
            startMuffins = false;
            startCookies = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VerifyDelivery();
        }
    }


    private void VerifyDelivery()
    {
        if (muffin.activeInHierarchy && plate.activeInHierarchy && startMuffins)
        {
            CompletedOrder(muffinOrder);
        }
        else if (cookies.activeInHierarchy && plate.activeInHierarchy && startCookies)
        {
            CompletedOrder(cookiesOrder);
        }
    }

    public void CompletedOrder(GameObject activeOrderUI)
    {
        activeOrderUI.SetActive(false);
        plate.SetActive(false);
        muffin.SetActive(false);
        cookies.SetActive(false);

        completedOrdersCount++;
        Debug.Log("Pedidos completados: " + completedOrdersCount);

        if (completedOrdersCount >= ordersToWin)
        {
            WinGame();
        }
    }

    public void RemoveOrder()
    {
        plate.SetActive(false);
        muffin.SetActive(false);
        cookies.SetActive(false);
    }

    private void WinGame()
    {
        SceneManager.LoadScene(3);
    }
}