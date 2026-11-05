using System.Collections;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public Vector3 moveDirection;
    public float moveSpeed = 0f;
    public float timeMoving = 0f;

    [Header("Configuración de Rotación")]
    public Vector3 rotateDirection;
    public float rotateSpeed = 0f;
    public float timeRotating = 0f;

    [Header("Estado")]
    public bool isMoving = false;
    public bool isRotating = false;
    private float countingTimeMoving = 0;
    private float countingTimeRotating = 0;


    void Start()
    {
        moveSpeed *= -1;
        rotateSpeed *= -1;
    }

    void Update()
    {
        Moving();
        Rotating();
    }

    public void Moving()
    {
        if (!isMoving) return;

        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
        countingTimeMoving -= Time.deltaTime;

        if (countingTimeMoving <= 0f)
        {
            countingTimeMoving = 0f;
            isMoving = false;
        }
    }

    public void Rotating()
    {
        if (!isRotating) return;

        transform.Rotate(rotateDirection.normalized * rotateSpeed * Time.deltaTime);
        countingTimeRotating -= Time.deltaTime;

        if (countingTimeRotating <= 0f)
        {
            countingTimeRotating = 0f;
            isRotating = false;
        }
    }

    public void StartMoving()
    {
        if (isMoving) return; 

        moveSpeed *= -1; // Invertir la dirección del movimiento
        countingTimeMoving = timeMoving;
        isMoving = true;
    }

    public void StartRotating()
    {
        if (isRotating) return; 

        rotateSpeed *= -1; // Invertir la dirección de la rotación
        countingTimeRotating = timeRotating;
        isRotating = true;
    }
}