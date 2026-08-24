using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] ingredientSlots; // Arrastra aquí los 4 componentes Image del Prefab
    [SerializeField] private GameObject completedTxt;

    public RecipeData CurrentRecipe { get; private set; }

    // Inicializa la carta con los datos de una receta
    public void Setup(RecipeData recipe)
    {
        CurrentRecipe = recipe;
        if (completedTxt != null) completedTxt.SetActive(false);

        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (i < recipe.ingredientIcons.Count)
            {
                ingredientSlots[i].sprite = recipe.ingredientIcons[i];
                ingredientSlots[i].gameObject.SetActive(true);
            }
            else
            {
                ingredientSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void MarkAsCompleted()
    {
        if (completedTxt != null) completedTxt.SetActive(true);
    }
}
