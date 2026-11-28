using UnityEngine;

public class OpenRecipes : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject recipe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        OpenCloseRecipe();

    }

    public void OpenCloseRecipe()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (recipe.activeInHierarchy)
            {
                recipe.SetActive(false);
                playerController.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                recipe.SetActive(true);
                playerController.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        }
    }

    public void CloseRecipe()
    {
        recipe.SetActive(false);
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
