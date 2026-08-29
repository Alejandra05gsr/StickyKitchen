using UnityEngine;

public class InteractableDoors : MonoBehaviour
{
    // Cajones / Horno
    public bool isDrawer;
    public bool isOven;
    public Vector3 initialPosition;
    public Vector3 finalPosition;

    // Puertas
    public bool isDoor;
    public Vector3 initialRotation;
    public Vector3 finalRotation;

    [Header("Referencias del Horno")]
    public Tray ovenTray;
    public OrderManager orderManager;
    public Score score;

    private bool isOpen = false;
    private bool hasBaked = false;
    private float pendingPoints = 0f;

    void Start()
    {
        isOpen = false;
    }

    public void InteractFurniture()
    {
        if (isDoor)
        {
            initialPosition = this.transform.position;
            finalPosition = this.transform.position;
            InteractDoor();
        }
        else if (isDrawer)
        {
            InteractDrawer();
        }
        else if (isOven)
        {
            InteractOven();
        }
    }

    void InteractDoor()
    {
        if (!isOpen)
        {
            transform.Rotate(finalRotation);
            isOpen = true;

            if (isOven) CleanOven();
        }
        else
        {
            transform.Rotate(initialRotation);
            isOpen = false;

            if (isOven) MakeFood();
        }
    }

    void InteractDrawer()
    {
        if (!isOpen)
        {
            transform.position = finalPosition;
            isOpen = true;

            if (isOven) CleanOven();
        }
        else
        {
            transform.position = initialPosition;
            isOpen = false;

            if (isOven) MakeFood();
        }
    }

    void InteractOven()
    {
        if (!isOpen)
        {
            transform.Rotate(finalRotation);
            isOpen = true;
            CleanOven();
        }
        else
        {
            transform.Rotate(initialRotation);
            isOpen = false;
            MakeFood();
        }
    }

    public void AccumulatePoints(float points)
    {
        pendingPoints += points;
    }

    void MakeFood()
    {
        // Se ejecuta únicamente al CERRAR el horno
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

            pendingPoints = 0f;
            hasBaked = true;
        }
    }

    void CleanOven()
    {
        // Al volver a abrir el horno tras hornear, apaga los prefabs
        if (hasBaked)
        {
            if (ovenTray != null)
            {
                ovenTray.DeactivateAllFoodItems();
            }
            hasBaked = false;
        }
    }
}