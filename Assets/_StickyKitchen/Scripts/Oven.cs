using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Oven : MonoBehaviour
{
    public Vector3 initialRotation;
    public Vector3 finalRotation;

    [Header("Referencias del Horno")]
    public Tray ovenTray;
    public OrderManager orderManager;
    public Score score;

    private bool hasBaked = false;

    private float pendingPoints = 0f;

    public bool orderCompleted = false;
    public float removedPoints = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InteractOven()
    {
        Debug.Log("Horno");
        OrderCompleted();//Checa si se completó la orden
        transform.Rotate(initialRotation); //Cierra el horno
        Invoke("CloseOven", 4.0f);
    }

    void CloseOven()
    {
        //Checar si está bien el postre o no
        //MakeFood();
        if (orderCompleted)
        {
            MakeFood();
        }
        else if (!orderCompleted)
        {
            RemoveScorePoints();
            CleanOven();
            Debug.Log("Puntos restados" + removedPoints);
        }
    }

    public void OpenOven()
    {
        transform.Rotate(finalRotation);
    }



    public void AccumulatePoints(float points)
    {
        pendingPoints += points;
        Debug.Log($"Total points" + pendingPoints);
    }

    public void RemoveScorePoints()
    {
        pendingPoints -= removedPoints;
    }

    void MakeFood()
    {
        if (orderManager == null) return;

        Order currentOrder = orderManager.GetCurrentOrder();

        if (currentOrder != null && currentOrder.IsCompleted())
        {
            // Suma los puntos a Score
            if (score != null)
            {
                score.AddPoints(pendingPoints);
            }

            // Marca la orden como finalizada y pasa a la siguiente
            currentOrder.CompleteOrderSequence();


            hasBaked = true;
        }
        CleanOven();
    }

    void CleanOven()
    {
        // Al volver a abrir el horno tras hornear, apaga los prefabs
        OpenOven();
        pendingPoints = 0f;
        ovenTray.DeactivateAllFoodItems();
        hasBaked = false;
    }

    public void OrderCompleted()
    {
        if (orderManager.CompletedOrder())
        {
            orderCompleted = false;
        }
        else if (!orderManager.CompletedOrder())
        {
            orderCompleted = true;
        }
    }



}
