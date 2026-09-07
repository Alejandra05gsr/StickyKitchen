using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrderTimer : MonoBehaviour
{
    public Slider slider;
    public Order order;
    public Score score;
    public float sliderTimer = 30f;
    public float removePoints = 3;
    private float currentTimer;
    public bool timerActive = true;

    void Start()
    {
        currentTimer = sliderTimer;
        slider.maxValue = sliderTimer;
        slider.value = sliderTimer;
    }

    void Update()
    {
        if (!timerActive) return;

        currentTimer -= Time.deltaTime;
        slider.value = currentTimer;
        ColorSlider();

        if (currentTimer <= 0)
        {
            timerActive = false;
            //score.RemovePoints(removePoints);
            Debug.Log("Points removed: " + removePoints);
            order.CancelOrder();
        }
    }

    void ColorSlider()
    {
        if (currentTimer <= slider.maxValue * 0.3f)
            slider.fillRect.GetComponent<Image>().color = Color.red;
        else if (currentTimer >= slider.maxValue * 0.7f)
            slider.fillRect.GetComponent<Image>().color = Color.green;
        else
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
    }

    void StopTimer()
    {
        timerActive = false;
    }
}
