using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ChangeToLevelMap()
    {
        SceneManager.LoadScene("LevelMap");
    }

    public void ChangeToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
