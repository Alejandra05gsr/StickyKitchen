using UnityEngine;
using System.Collections;

public class InteractableDoors : MonoBehaviour
{
    // Cajones / Horno
    public bool isDrawer;
    public Vector3 initialPosition;
    public Vector3 finalPosition;

    // Puertas
    public bool isDoor;
    public Vector3 initialRotation;
    public Vector3 finalRotation;

    private bool isOpen = false;


    void Start()
    {
        isOpen = false;
    }

    public void InteractFurniture() //Checar para que tambien se llame el horno al disparar
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
    }

    void InteractDoor()
    {
        if (!isOpen)
        {
            transform.Rotate(finalRotation);
            isOpen = true;

        }
        else
        {
            transform.Rotate(initialRotation);
            isOpen = false;

        }
    }

    void InteractDrawer()
    {
        if (!isOpen)
        {
            transform.position = finalPosition;
            isOpen = true;

        }
        else
        {
            transform.position = initialPosition;
            isOpen = false;

        }
    }

   

   
}