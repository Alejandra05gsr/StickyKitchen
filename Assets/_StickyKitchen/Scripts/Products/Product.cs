using UnityEngine;

public class Product : MonoBehaviour
{
    public int idProduct;
    public int points;

    void Start()
    {
        CheckTypeOfProduct();
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
                points = 4;
                break;
            case "Eggs":
                idProduct = 2;
                points = 4;
                break;
            case "Honey":
                idProduct = 3;
                points = 8;
                break;
            case "Chocolate":
                idProduct = 4;
                points = 10;
                break;
            case "Sugar":
                idProduct = 5;
                points = 9;
                break;
            case "Coffee":
                idProduct = 6;
                points = 15;
                break;
        }
    }
}