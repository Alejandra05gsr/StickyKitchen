using UnityEngine;
using System.Collections;

public class InteractableDoors : MonoBehaviour
{
    // Cajones 
    public bool isDrawer;
    public Vector3 initialPosition;
    public Vector3 finalPosition;

    // Puertas
    public bool isDoor;
    public Vector3 initialRotation;
    public Vector3 finalRotation;
    public float rotationSpeed = 2f;

    public GameObject rotationPoint;

    private bool isOpen = false;
    private bool isMoving = false;


    void Start()
    {
        isOpen = false;
    }

    public void InteractFurniture() 
    {
        if (isMoving) return;

        if (isDoor)
        {
            InteractDoor();
        }
        else if (isDrawer)
        {
            InteractDrawer();
        }
    }

    void InteractDoor()
    {
        isOpen = !isOpen;

        // Determinamos el ángulo destino
        Vector3 targetEuler = isOpen ? finalRotation : initialRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        // Si asignaste un pivote/padre, rotamos ese objeto; si no, rotamos este objeto
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
            // Interpola suavemente la rotación del objeto padre/pivote
            objectToRotate.localRotation = Quaternion.Slerp(startRotation, target, time);
            yield return null;
        }

        objectToRotate.localRotation = target;
        isMoving = false;
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