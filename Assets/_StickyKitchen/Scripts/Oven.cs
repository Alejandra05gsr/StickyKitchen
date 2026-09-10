using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections;

public class Oven : MonoBehaviour
{
    public Vector3 initialRotation;
    public Vector3 finalRotation;
    public float rotationSpeed = 2f;
    public GameObject rotationPoint;

    [Header("Referencias del Horno")]
    public Tray ovenTray;
    public OrderManager orderManager;
    public Score score;

    private bool hasBaked = false;
    private float pendingPoints = 0f;

    public bool orderCompleted = false;
    public float removedPoints = 5f;

    private bool isMoving = false;

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
        if (isMoving) return;

        Debug.Log("Horno");
        OrderCompleted();//Checa si se completó la orden

        AnimateDoor(initialRotation);
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
        AnimateDoor(finalRotation);
    }


    private void AnimateDoor(Vector3 targetEuler)
    {
        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        Transform targetTransform = (rotationPoint != null) ? rotationPoint.transform : transform;

        StartCoroutine(RotateDoor(targetTransform, targetRotation));
    }

    IEnumerator RotateDoor(Transform objectToRotate, Quaternion target)
    {
        isMoving = true;
        Quaternion startRotation = objectToRotate.localRotation;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * rotationSpeed;
            objectToRotate.localRotation = Quaternion.Slerp(startRotation, target, time);
            yield return null;
        }

        objectToRotate.localRotation = target;
        isMoving = false;
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
        if (ovenTray != null)
        {
            ovenTray.DeactivateAllFoodItems();
        }
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
