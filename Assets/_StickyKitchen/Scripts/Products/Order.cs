using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Order : MonoBehaviour
{
    [Header("UI References - Receta Principal")]
    [SerializeField] private TextMeshProUGUI recipeNameText; // Texto para el nombre (Pan, Galletas, etc.)
    [SerializeField] private Image finalProductImage;        // Imagen del plato/receta final

    [Header("UI References - Ingredientes")]
    [SerializeField] private Image[] ingredientSlots;
    [SerializeField] private GameObject completedTxt;

    public RecipeData CurrentRecipe { get; private set; }

    // Lista para rastrear los IDs pendientes de entregar
    private List<int> pendingIngredientIDs = new List<int>();
    private OrderManager orderManager;
    public Tray tray;

    public void Setup(RecipeData recipe, OrderManager manager)
    {
        CurrentRecipe = recipe;
        orderManager = manager;

        if (completedTxt != null) completedTxt.SetActive(false);

        // 1. Asignar Nombre e Imagen del Producto Final
        if (recipeNameText != null)
        {
            recipeNameText.text = recipe.recipeName;
        }

        if (finalProductImage != null)
        {
            if (recipe.finalProductSprite != null)
            {
                finalProductImage.sprite = recipe.finalProductSprite;
                finalProductImage.gameObject.SetActive(true);
            }
            else
            {
                finalProductImage.gameObject.SetActive(false); // Ocultar si no hay sprite asignado
            }
        }

        // 2. Configurar Slots de Ingredientes
        pendingIngredientIDs.Clear();

        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (i < recipe.ingredientIcons.Count)
            {
                ingredientSlots[i].sprite = recipe.ingredientIcons[i];
                ingredientSlots[i].gameObject.SetActive(true);

                if (i < recipe.ingredientIDs.Count)
                {
                    pendingIngredientIDs.Add(recipe.ingredientIDs[i]);
                }
            }
            else
            {
                ingredientSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public bool TryDeliverIngredient(int ingredientID)
    {
        for (int i = 0; i < pendingIngredientIDs.Count; i++)
        {
            if (pendingIngredientIDs[i] == ingredientID)
            {
                // Oculta la UI del ingrediente de inmediato
                ingredientSlots[i].gameObject.SetActive(false);
                pendingIngredientIDs[i] = -1; // Marcado como entregado

                return true;
            }
        }
        return false;
    }

    // Consulta si ya se entregaron todos los ingredientes sin completar la orden todavía
    public bool IsCompleted()
    {
        foreach (int id in pendingIngredientIDs)
        {
            if (id != -1) return false;
        }
        return true;
    }

    private void CheckCompletion()
    {
        bool allDone = true;
        foreach (int id in pendingIngredientIDs)
        {
            if (id != -1)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            MarkAsCompleted();
            if (orderManager != null)
            {
                orderManager.CompleteCurrentOrder();
            }
        }
    }

    public void CompleteOrderSequence()
    {
        MarkAsCompleted();
        if (orderManager != null)
        {
            orderManager.CompleteCurrentOrder();
        }
    }

    public void MarkAsCompleted()
    {
        if (completedTxt != null) completedTxt.SetActive(true);
    }

    public void CancelOrder()
    {
        if (orderManager != null)
        {
            orderManager.RemoveOrder(this);
        }
        else
        {
            tray.DeactivateAllFoodItems();
            Destroy(this.gameObject);
        }
    }
}
