using UnityEngine;

public class InteractableDoors : MonoBehaviour
{
    //Cajones
    public bool isDrawer;
    public Vector3 initialPosition;
    public Vector3 finalPosition;

    //Puertas
    public bool isDoor;
    public Vector3 initialRotation;
    public Vector3 finalRotation;



    bool isOpen;


    void Start()
    {
        isOpen = false;
    }

    void Update()
    {

    }

    public void InteractFurniture()
    {

        if (isDoor)
        {
            //Esta pos inicial y final
            initialPosition = this.transform.position;
            finalPosition = this.transform.position;
            InteractDoor();
        }
        else if (isDrawer)
        {
            //Esta rot inicial y final
            //initialRotation = this.transform.rotation;
            //finalRotation = this.transform.rotation;
            InteractDrawer();
        }

    }




    void InteractDoor() //Rotación
    {
        if (!isOpen)
        {
            transform.Rotate(finalRotation);
            Debug.Log("Open");
            isOpen = true;
        }
        else if (isOpen)
        {
            transform.Rotate(initialRotation);
            Debug.Log("Close");
            isOpen = false;
        }
    }


    void InteractDrawer() //Posición
    {
        if (!isOpen)
        {
            transform.position = finalPosition;
            Debug.Log("Open");
            isOpen = true;
        }
        else if (isOpen)
        {
            transform.position = initialPosition;
            Debug.Log("Close");
            isOpen = false;
        }

    }

}
