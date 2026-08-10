using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour
{
    private int idProduct;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TypeOfProduct(idProduct);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void TypeOfProduct(int idProduct)
    {
        switch(this.gameObject.tag)
        {
            case "Flour":
                idProduct = 0;
                break;
            case "Eggs":
                idProduct = 1;
                break ;
            case "Butter":
                idProduct = 2;
                break ;
            case "Chocolate":
                idProduct = 3;
                break ;
            case "Milk":
                idProduct = 4;
                break ;
            case "Sugar":
                idProduct = 5;
                break ;
        }
        //Debug.Log("Type: " + idProduct);
    }

}
