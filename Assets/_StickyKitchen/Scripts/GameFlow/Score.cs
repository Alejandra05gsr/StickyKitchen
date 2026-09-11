using TMPro;
using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI pointsGameText;
    public float orderPoints;
    public float totalPoints;

    [Header("Configuración de Colores")]
    public Color colorNormal = Color.black;
    public Color colorSuma = Color.green;
    public Color colorResta = Color.red;
    public float duracionEfectoColor = 1.0f; // Tiempo que permanece el color


    [Header("Finish Screen")]
    public GameObject finishScreen;
    public GameObject[] stars;
    public TextMeshProUGUI totalPointText;


    private Coroutine corrutinaColorPoints;
    private Coroutine corrutinaColorTotal;


    void Start()
    {
        StartPoints();
    }

    //Points
    void StartPoints() 
    {
        orderPoints = 0;
        totalPoints = 0;

        if (pointsGameText != null) pointsGameText.color = colorNormal;
        if (totalPointText != null) totalPointText.color = colorNormal;

        UpdateUIPoints();
    }


    void UpdateUIPoints() 
    {
        pointsGameText.text = "Points: " + orderPoints;
        totalPointText.text = ("Total: " + totalPoints);
    }


    
    public void ShowFinishScreen() 
    {
        finishScreen.SetActive(true);
        CalculateStars();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AddPoints(float addPoint)
    {
        orderPoints += addPoint;
        totalPoints += addPoint;
        UpdateUIPoints();

        AplicarEfectoColor(colorSuma);
    }

    public void RemovePoints(float removePoint)
    {
        orderPoints -= removePoint;
        totalPoints -= removePoint;

        if (orderPoints < 0) orderPoints = 0;
        if (totalPoints < 0) totalPoints = 0;

        UpdateUIPoints();

        AplicarEfectoColor(colorResta);
    }


    private void AplicarEfectoColor(Color colorEfecto)
    {
        // Si ya había una corrutina activa, la detenemos para reiniciar el temporizador
        if (corrutinaColorPoints != null) StopCoroutine(corrutinaColorPoints);
        if (corrutinaColorTotal != null) StopCoroutine(corrutinaColorTotal);

        if (pointsGameText != null)
            corrutinaColorPoints = StartCoroutine(CambiarColorTemporal(pointsGameText, colorEfecto));

        if (totalPointText != null)
            corrutinaColorTotal = StartCoroutine(CambiarColorTemporal(totalPointText, colorEfecto));
    }

    IEnumerator CambiarColorTemporal(TextMeshProUGUI textoUI, Color nuevoColor)
    {
        textoUI.color = nuevoColor;

        // Espera el tiempo especificado en el Inspector
        yield return new WaitForSeconds(duracionEfectoColor);

        // Vuelve gradualmente o de golpe al color normal
        float tiempoInterpolacion = 0f;
        Color colorActual = textoUI.color;

        while (tiempoInterpolacion < 0.5f)
        {
            tiempoInterpolacion += Time.deltaTime;
            textoUI.color = Color.Lerp(colorActual, colorNormal, tiempoInterpolacion / 0.5f);
            yield return null;
        }

        textoUI.color = colorNormal;
    }


    void CalculateStars()
    {
        if (totalPoints >= 30)
        {
            for (int i = 0; i < 5; i++) stars[i].SetActive(true);
        }
        else if (totalPoints >= 20)
        {
            for (int i = 0; i < 4; i++) stars[i].SetActive(true);
        }
        else if (totalPoints >= 15)
        {
            for (int i = 0; i < 3; i++) stars[i].SetActive(true);
        }
        else if (totalPoints >= 10)
        {
            for (int i = 0; i < 2; i++) stars[i].SetActive(true);
        }
        else if (totalPoints >= 5)
        {
            for (int i = 0; i < 1; i++) stars[i].SetActive(true);
        }
    }
}
