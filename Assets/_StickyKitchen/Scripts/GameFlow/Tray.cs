using UnityEngine;

public class Tray : MonoBehaviour
{
    public GameObject[] foodItems; // Array to hold the food items on the tray

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Funcion que prende el objeto de comida en el tray
    public void ActivateFoodItem(int index)
    {
        if (index >= 0 && index < foodItems.Length)
        {
            foodItems[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Index out of range for foodItems array.");
        }
    }

    //Funcion que apaga el objeto de comida en el tray
    public void DeactivateFoodItem(int index)
    {
        if (index >= 0 && index < foodItems.Length)
        {
            foodItems[index].SetActive(false);
        }
        else
        {
            Debug.LogWarning("Index out of range for foodItems array.");
        }
    }

    //Funcion que apaga todos los objetos de comida en el tray
    public void DeactivateAllFoodItems()
    {
        foreach (GameObject foodItem in foodItems)
        {
            foodItem.SetActive(false);
        }
    }
}
