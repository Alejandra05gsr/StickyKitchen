using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Food Game/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public Sprite finalProductSprite;
    public List<Sprite> ingredientIcons; // Lista de sprites (UI)
    public List<int> ingredientIDs;      // IDs asociados a cada icono (0: Flour, 1: Butter, etc.)
}