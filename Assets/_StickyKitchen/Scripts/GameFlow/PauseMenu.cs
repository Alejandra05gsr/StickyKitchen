using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseMenuUI.SetActive(true);
            playerController.enabled = false;
            //Prender el mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        
    }

    public void LockCursor()
    {
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
