using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour
{
    public int idProduct;
    public int currentProd;

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
                break;
            case "Butter":
                idProduct = 1;
                break;
            case "Eggs":
                idProduct = 2;
                break;
            case "Chocolate":
                idProduct = 3;
                break;
            case "Milk":
                idProduct = 4;
                break;
            case "Sugar":
                idProduct = 5;
                break;
        }
        //Debug.Log("Type: " + idProduct);
        TypeOfProduct(currentProd);
    }

    public void TypeOfProduct(int idCurrentProduct)
    {
        idCurrentProduct = idProduct;
    }

}
