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

    public bool finishCook = false;

    public GameObject door;
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
        if (finishCook) return;

        //Prender el ingrediente de comida que se hizo
        if (flour.activeInHierarchy && sugar.activeInHierarchy && eggs.activeInHierarchy && honey.activeInHierarchy) //Muffins
        {
            muffin.SetActive(true);
        }
        else if (flour.activeInHierarchy && butter.activeInHierarchy && eggs.activeInHierarchy && chocolate.activeInHierarchy) //Cookies
        {
            cookies.SetActive(true);
        }
        else
        {
            failCook.SetActive(true);
        }

        //limpiar el horno de los ingredientes
        tray.DeactivateAllFoodItems(); //Duplicado?

        finishCook = true;
    }

    public void InteractOven() //Se inicia con este al darle a la palanca
    {
        if (isMoving) return;

        finishCook = false;

        door.GetComponent<Oven>().InteractOven(); //Se mueve la puerta del horno
        Invoke("Cook", 3.0f); //Se activa el postre
    }


   
}
