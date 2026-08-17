using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI pointsGameText;
    public float orderPoints;
    public float totalPoints;


    [Header("Finish Screen")]
    public GameObject finishScreen;
    public GameObject[] stars;
    public TextMeshProUGUI totalPointText;


    void Start()
    {
        StartPoints();
    }

    void Update()
    {

    }

    //Points
    void StartPoints() //Empezamos con 0 puntos
    {
        orderPoints = 0;
        totalPoints = 0;
        UpdateUIPoints();
    }


    void UpdateUIPoints() //Actualizamos los puntos  de la UI
    {
        pointsGameText.text = "Points: " + orderPoints;
        totalPointText.text = ("Total: " + totalPoints);
    }


    
    public void ShowFinishScreen() //Finish Screen. SE MANDA A LLAMAR 
    {
        finishScreen.SetActive(true);
        CalculateStars();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CalculateTotalPoints()
    {
        totalPoints += orderPoints;
       
    }

    public void AddPoints(float addPoint)
    {
        orderPoints += addPoint;
        CalculateTotalPoints();
        UpdateUIPoints();
    }


    void CalculateStars() //Calculamos los puntos al finalizar el tiempo (timer)
    {
        //Sumamos los puntos

        if (totalPoints >= 15)
        {
            for (int i = 0; i < 5; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 10)
        {
            for (int i = 0; i < 4; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 8)
        {
            for (int i = 0; i < 3; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 5)
        {
            for (int i = 0; i < 2; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 3)
        {
            for (int i = 0; i < 1; i++)
            {
                stars[i].SetActive(true);
            }
        }

    }
       
}
