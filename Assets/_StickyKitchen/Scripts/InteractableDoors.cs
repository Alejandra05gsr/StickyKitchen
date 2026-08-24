using UnityEngine;

public class InteractableDoors : MonoBehaviour
{
    public Vector3 initialPosition;
    public Vector3 finalPosition;
    public Quaternion initialRotation;
    public Quaternion finalRotation;


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
      //Que si esta en la pos inicial se cambie a la final y si está en la final se ponga en la inicial
      if (!isOpen)
        {
            //Pone su rotación inicial como el inicio
           // this.transform.position = finalPosition;
            transform.Rotate(90, 0, 0);
            Debug.Log("Open");
            isOpen = true;
        }
      else if (isOpen)
        {
            //this .gameObject.transform.rotation;
            //this.transform.position = initialPosition;
            transform.Rotate(-90, 0, 0);
            Debug.Log("Close");
            isOpen = false;
        }
    }



}
