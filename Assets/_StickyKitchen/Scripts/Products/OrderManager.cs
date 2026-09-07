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

    public InteractableDoors iDoors;
    public bool orderCompleted = false;

    void Start()
    {
        SpawnOrder(initialRecipe);
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
        GameObject newOrderGO = Instantiate(orderPrefab, ordersParent);
        Order order = newOrderGO.GetComponent<Order>();
        order.Setup(recipe, this); // Pasamos la referencia de este manager

        activeOrders.Add(order);
    }

    // Devuelve la orden activa en la primera posición de la cola
    public Order GetCurrentOrder()
    {
        return activeOrders.Count > 0 ? activeOrders[0] : null;
    }

    public void CompleteCurrentOrder()
    {
        if (activeOrders.Count == 0) //Si hay 0 activos entonces no falta completar
        {
            OrderCompleted();
            return;
        }
        else
        {
            OrderNotCompleted();
        }

        Order targetOrder = activeOrders[0];
        targetOrder.MarkAsCompleted();
        StartCoroutine(RemoveOrderRoutine(targetOrder));

    }

    public void RemoveOrder(Order order)
    {
        StartCoroutine(RemoveOrderRoutine(order));
    }

    IEnumerator RemoveOrderRoutine(Order order)
    {
        if (activeOrders.Contains(order))
        {
            activeOrders.Remove(order);
        }

        yield return new WaitForSeconds(1.0f);

        if (order != null)
        {
            Destroy(order.gameObject);
        }
    }

    public void OrderCompleted()
    {
        orderCompleted = true;
    }

    public void OrderNotCompleted()
    {
        orderCompleted = false;
    }

    public bool CompletedOrder()
    {
        if (!orderCompleted)
        {
            return false;
        }
        else 
        {
            return true;
        }

    }

}
