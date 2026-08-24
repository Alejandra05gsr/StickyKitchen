using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] ingredientSlots; 
    [SerializeField] private GameObject completedTxt;

    public RecipeData CurrentRecipe { get; private set; }

    bool timerActive;
    public float timerTime = 30;
    public Slider slider;


    void Update()
    {
        Timer();
    }

    void Start()
    {
        timerActive = true;
    }


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

    void Timer()
    {
        if (!timerActive) return;

        if (timerTime > 0)
        {
            //slider
            timerTime -= Time.deltaTime;
        }
        else if (timerTime <= 0)
        {
            timerActive = false;
            timerTime = 0;
            CancelOrder();
        }

    }


    void CancelOrder()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Orden Cancelada");
    }
}
