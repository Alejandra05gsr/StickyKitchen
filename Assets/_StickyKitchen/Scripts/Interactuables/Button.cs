using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject[] objectMove;
    public int syrupCount;
    public int maxSyrupCount;

    public bool canInteract = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canInteract = true;
        syrupCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StuckButton();
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name + " ha colisionado con el botón");

    //    if (collision.gameObject.CompareTag ("Player") || collision.gameObject.CompareTag ("Interactuable")) //&& canInteract
    //    {
    //        //Realizar la acción mover
    //        //Por cada objeto en el array de objetos a mover, llamar al método StartMoving() del script ObjectMove
    //        foreach (GameObject obj in objectMove)
    //        {
    //            obj.GetComponent<ObjectMove>().StartMoving();
    //        }
    //    }
    //}



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactuable") && syrupCount < maxSyrupCount)
        {
            this.gameObject.GetComponent<StuckInteractuable>().MaterialTurnOn();
            foreach (GameObject obj in objectMove)
            {
                obj.GetComponent<ObjectMove>().StartMoving();
            }
            syrupCount++;
        }
    }


    public void StuckButton()
    {
        if (syrupCount >= maxSyrupCount)
        {
            this.gameObject.GetComponent<StuckInteractuable>().Changematerial(3);
        }
    }


}
