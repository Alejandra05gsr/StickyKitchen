using UnityEngine;


//Mueve palanca y llama la funcion de cocinar del horno
public class OvenLever : MonoBehaviour
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactuable") && syrupCount < maxSyrupCount)
        {
            this.gameObject.GetComponent<StuckInteractuable>().MaterialTurnOn();
            foreach (GameObject obj in objectMove)
            {
                obj.GetComponent<ObjectMove>().StartMoving();
                this.gameObject.GetComponent<OvenFood>().InteractOven(); //Se interactua con lo que pasará dentro del horno
            }
            syrupCount++;
        }
    }


    public void StuckButton()
    {
        if (syrupCount >= maxSyrupCount)
        {
            this.gameObject.GetComponent<StuckInteractuable>().Changematerial();
        }
    }


}
