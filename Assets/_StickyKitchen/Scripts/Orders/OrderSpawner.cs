using UnityEngine;

public class OrderSpawner : MonoBehaviour
{
    public GameObject[] orderPrefab; // Prefab del pedido
    public Transform[] placeOrder; // Lugar donde se va a instanciar el pedido

    public int maxOrders = 5; //Maximo de pedidos que se van a instanciar en la escena, si se supera este numero no se instanciara ningun pedido mas.


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnOrder(maxOrders);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimerOrderToAppear()
    {

    }


    public void SpawnOrder(int orderIndex)
    {
        // Adquirimos de manera random un prefab de los de la lista y luego lo instanciamos en uno de los lugares del place order si no tiene un pedido ya instanciado.
        if (orderIndex >= 0 && orderIndex < orderPrefab.Length)
        {
            GameObject selectedOrderPrefab = orderPrefab[orderIndex];
            // Buscar un lugar disponible para instanciar el pedido
            foreach (Transform place in placeOrder)
            {
                if (place.transform.childCount == 0) // Si no hay ningún pedido ya instanciado
                {
                    Instantiate(selectedOrderPrefab, place.transform.position, Quaternion.identity, place.transform);
                    break; // Salimos del bucle después de instanciar el pedido
                }
            }
        }

    }

}
