using UnityEngine;

public class DeliverZone : MonoBehaviour
{
    public Transform deliverZone;
    public GameObject prefabPlate;
    public GameObject prefabFood;
    private int plateCount = 0;
    private int foodCount = 0;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.gameObject.name);

        if (other.CompareTag("Plate") && plateCount == 0)
        {
            prefabPlate.SetActive(true);
            plateCount++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Food") && foodCount == 0)
        {
            prefabFood.SetActive(true);
            foodCount++;
            Destroy(other.gameObject);
        }

    }

    public void DeliverFood()
    {
        if (prefabFood.activeSelf && prefabPlate.activeSelf)
        {
            prefabFood.SetActive(false);
            prefabPlate.SetActive(false);
            foodCount--;
            plateCount--;
        }
    }


}
