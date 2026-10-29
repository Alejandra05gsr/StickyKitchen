using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject[] objectMove;
    int syrupCount;

    public bool canInteract = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canInteract = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag ("Player")) //&& canInteract
        {
            //Realizar la acción mover
            //Por cada objeto en el array de objetos a mover, llamar al método StartMoving() del script ObjectMove
            foreach(GameObject obj in objectMove)
            {
                obj.GetComponent<ObjectMove>().StartMoving();
            }

            //objectMove.GetComponent<ObjectMove>().StartMoving();

            //canInteract = false;

        }

        //if (collision.gameObject.CompareTag("Water") && !canInteract)
        //{
        //    canInteract = true;
        //}
    }

    //Podría guardar como hijos en una lista los disparos de jarabes para que si tiene 3 o más
    //entonces se desactive el canInteract

}
