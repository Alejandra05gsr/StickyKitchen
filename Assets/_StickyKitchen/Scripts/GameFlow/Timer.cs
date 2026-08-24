using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public float time = 180.0f;
    float remainingTime = 0f;

    public PlayerController player;

    public bool timerActive;
    public Score score;

    public TextMeshProUGUI timerText;

    void Start()
    {
        remainingTime = time;
        timerActive = true; //Al iniciar debe ser falso
    }

    void Update()
    {
        TimerActive();  
    }

    void TimerActive()
    {
        if (!timerActive) return;

        //Pasa el tiempo del timer
        GameTimer();

    }

    public void ActiveTimer()
    {
        timerActive = true;
    }

    void FinishTimer()
    {
        timerActive = false;
        score.GetComponent<Score>().ShowFinishScreen(); 
        player.GetComponent<PlayerController>().enabled = false; 
    }

    void GameTimer()
    {
        if (!timerActive) return;


        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            FinishTimer();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);


    }

}
