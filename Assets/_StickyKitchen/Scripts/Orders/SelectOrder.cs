using UnityEngine;

public class SelectOrder : MonoBehaviour
{
    public GameObject componentSelected;

    public Vector3 normalSize;
    public Vector3 selectedSize;

    public float scaleFactor = 1.2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //el tamaño normal se iguala al tamaño en escala del componente seleccionado, y el tamaño seleccionado se calcula multiplicando el tamaño normal por el factor de escala
        normalSize = componentSelected.transform.localScale;
        selectedSize = normalSize * scaleFactor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            componentSelected.transform.localScale = selectedSize;
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            componentSelected.transform.localScale = normalSize;
        }

    }

}
