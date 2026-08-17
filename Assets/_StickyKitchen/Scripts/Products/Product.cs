using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour
{
    public int idProduct;
    public int points;
    int currentProd;
    int currentPoints;

    void Start()
    {
        CheckTypeOfProduct();
    }

    void Update()
    {

    }

    void CheckTypeOfProduct()
    {
        switch (this.gameObject.tag)
        {
            case "Flour":
                idProduct = 0;
                points = 3;
                break;
            case "Butter":
                idProduct = 1;
                points = 3;
                break;
            case "Eggs":
                idProduct = 2;
                points = 3;
                break;
            case "Chocolate":
                idProduct = 3;
                points = 8;
                break;
            case "Milk":
                idProduct = 4;
                points = 5;
                break;
            case "Sugar":
                idProduct = 5;
                points = 7;
                break;
            case "Coffee":
                idProduct = 6;
                points = 10;
                break;
        }
        //Debug.Log("Type: " + idProduct);
        TypeOfProduct(currentProd, currentPoints);
    }

    public void TypeOfProduct(int idCurrentProduct, int currentPoints)
    {
        idCurrentProduct = idProduct;
        currentPoints = points;
    }

}
