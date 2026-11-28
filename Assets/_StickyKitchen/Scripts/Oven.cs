using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections;

public class Oven : MonoBehaviour
{
    public Vector3 initialRotation; //abierto
    public Vector3 finalRotation; //cerrado
    public float rotationSpeed = 2f;
    public GameObject rotationPoint;

    [Header("Referencias del Horno")]
    public Tray ovenTray;
    public OrderManager orderManager;


    public bool orderCompleted = false;
 
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

        CloseOven();
        Invoke("OpenOven", 4.0f); //Se vincula al paso 3
    }

    void CloseOven()    //Se cierra la puerta (Paso 3)
    {
        AnimateDoor(initialRotation);
    }



    public void OpenOven()
    {
        AnimateDoor(finalRotation);
        Invoke("ResetingTray", 1.0f); 
    }

    public void ResetingTray()
    {
        ovenTray.ResetTrayPosition();
    }



    //Se abre la puerta (Paso 1)
    private void AnimateDoor(Vector3 targetEuler)
    {
        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        Transform targetTransform = (rotationPoint != null) ? rotationPoint.transform : transform;

        StartCoroutine(RotateDoor(targetTransform, targetRotation));
    }


    //Rota la puerta (Paso 2 y 4)
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


  

    //void MakeFood()
    //{
    //    if (orderManager == null) return;

    //    Order currentOrder = orderManager.GetCurrentOrder();

    //    if (currentOrder != null && currentOrder.IsCompleted())
    //    {
    //        // Suma los puntos a Score
    //        if (score != null)
    //        {
    //            score.AddPoints(pendingPoints);
    //        }

    //        // Marca la orden como finalizada y pasa a la siguiente
    //        currentOrder.CompleteOrderSequence();


    //        hasBaked = true;
    //    }
    //    CleanOven();
    //}

    void CleanOven()
    {
        // Al volver a abrir el horno tras hornear, apaga los prefabs

        if (ovenTray != null)
        {
            ovenTray.DeactivateAllFoodItems();
        }
        OpenOven();
    }

 


}
