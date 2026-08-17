using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public float totalMinutes = 3.0f;
    float minutes = 0f;

    [SerializeField]
    public float totalSeconds = 60;
    float seconds = 0;

   

    private float currentTime;

    public bool timerActive;
    public Score score;

    public TextMeshProUGUI timerText;

    void Start()
    {
        seconds = totalSeconds;
        UITime();
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
        score.GetComponent<Score>().ShowFinishScreen(); ;
    }

    void GameTimer()
    {
        if (!timerActive) return;

        if(seconds >= 0)
        {
            seconds -= Time.deltaTime;
            timerText.text = ("0" + minutes + ":" + seconds);
        }
        else
        {
            seconds = 0;
            FinishTimer();
        }

        
    }

    void UITime()
    {
        timerText.text = ("0" + minutes + ":" + seconds);

    }

}
