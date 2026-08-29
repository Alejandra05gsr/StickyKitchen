using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrderTimer : MonoBehaviour
{
    public Slider slider;
    public Order order;
    public float sliderTimer = 30f;
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
}
