using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Food Game/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public List<Sprite> ingredientIcons; // Lista de los 4 ingredientes
}