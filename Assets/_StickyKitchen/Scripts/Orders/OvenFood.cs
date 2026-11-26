using UnityEngine;
using System.Collections;

public class OvenFood : MonoBehaviour
{
    [Header("Ingredientes")]
    public GameObject flour, sugar, butter, eggs, chocolate, honey;

    [Header("Comida")]
    public GameObject muffin;
    public GameObject cookies;
    public GameObject failCook;

    [Header("Cerrar Puerta")]
    public Vector3 initialRotation;
    public Vector3 finalRotation;
    public float rotationSpeed = 2f;
    public GameObject rotationPoint;
    private bool isMoving = false;


    public Tray tray;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Cook() //Cocina el postre
    {
        //limpiar el horno de los ingredintes
        tray.DeactivateAllFoodItems();

        //Prender el ingrediente de comida que se hizo
        if (flour.activeInHierarchy && sugar.activeInHierarchy && honey.activeInHierarchy) //Muffins
        {
            muffin.SetActive(true);
        }
        else if (flour.activeInHierarchy && sugar.activeInHierarchy && butter.activeInHierarchy && eggs.activeInHierarchy && chocolate.activeInHierarchy) //Cookies
        {
            cookies.SetActive(true);
        }
        else
        {
            failCook.SetActive(true);
        }
    }

    public void InteractOven() //Se inicia con este al darle a la palanca
    {
        if (isMoving) return;

        AnimateDoor(initialRotation);
        Invoke("Cook", 4.0f);
    }


    public void AnimateDoor(Vector3 targetEuler)
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

}
