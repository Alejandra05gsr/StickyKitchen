using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI pointsGameText;
    public int totalPoints;


    [Header("Finish Screen")]
    public GameObject finishScreen;
    public GameObject[] stars;
    public TextMeshProUGUI totalPointText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Points
    void StartPoints()
    {
        totalPoints = 0;

        UpdatePoints();
    }


    void UpdatePoints()
    {
        pointsGameText.text = "Points: " + totalPoints;
        CalculateStars();
    }

    public void GameTimer()
    {

    }


    //Finish Screen
    void ShowFinishScreen()
    {
        finishScreen.SetActive(true);
        CalculateStars();
    }

    void CalculateStars()
    {
        if (totalPoints >= 100)
        {
            for (int i = 0; i < 5; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 80)
        {
            for (int i = 0; i < 4; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 60)
        {
            for (int i = 0; i < 3; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 40)
        {
            for (int i = 0; i < 2; i++)
            {
                stars[i].SetActive(true);
            }
        }
        else if (totalPoints >= 20)
        {
            for (int i = 0; i < 1; i++)
            {
                stars[i].SetActive(true);
            }
        }

    }
}
