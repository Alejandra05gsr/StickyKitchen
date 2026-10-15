using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject objectMove;
    int syrupCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag ("Syrup"))
        {
            //Realizar la acción mover
            objectMove.GetComponent<ObjectMove>().StartMoving();

            //Checar si tiene más de 3 jarabes en la palanca, si es asi entonces se estanca

        }
    }

}
