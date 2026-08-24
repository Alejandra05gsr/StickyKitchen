using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Recetas Configuradas")]
    [SerializeField] private RecipeData initialRecipe; // La receta fija de 4 ingredientes
    [SerializeField] private List<RecipeData> possibleRecipes; // Las 3+ recetas disponibles

    [Header("Configuración UI")]
    [SerializeField] private Transform ordersParent; // GameObject con 'Horizontal Layout Group'
    [SerializeField] private GameObject orderPrefab; // Prefab con el script OrderUI

    [Header("Tiempos")]
    [SerializeField] private float spawnInterval = 5f; // Tiempo entre nuevos pedidos
    [SerializeField] private int maxOrdersOnScreen = 5;

    private List<Order> activeOrders = new List<Order>();

    void Start()
    {
        // 1. Crear el primer pedido preestablecido
        SpawnOrder(initialRecipe);

        // 2. Iniciar la rutina de spawn automático
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeOrders.Count < maxOrdersOnScreen && possibleRecipes.Count > 0)
            {
                RecipeData randomRecipe = possibleRecipes[Random.Range(0, possibleRecipes.Count)];
                SpawnOrder(randomRecipe);
            }
        }
    }

    void SpawnOrder(RecipeData recipe)
    {
        // Instancia la nueva comanda
        GameObject newOrderGO = Instantiate(orderPrefab, ordersParent);
        Order order = newOrderGO.GetComponent<Order>();
        order.Setup(recipe);

        // Si quieres que aparezcan a la DERECHA, se añaden al final de la lista UI
        activeOrders.Add(order);
    }

    // Llama a esto cuando el jugador prepare con éxito el pedido actual
    public void CompleteCurrentOrder()
    {
        if (activeOrders.Count == 0) return;

        // Según tu lógica, el pedido de la extrema derecha (o el primero) se completa
        Order targetOrder = activeOrders[activeOrders.Count - 1]; // Extrema derecha
        targetOrder.MarkAsCompleted();

        StartCoroutine(RemoveOrderRoutine(targetOrder));
    }

    IEnumerator RemoveOrderRoutine(Order order)
    {
        yield return new WaitForSeconds(1.5f); // Tiempo de espera visual

        activeOrders.Remove(order);
        Destroy(order.gameObject);
        // Al destruir el GameObject, el 'Horizontal Layout Group' 
        // acomoda y recorre automáticamente todas las demás cartas.
    }
}
