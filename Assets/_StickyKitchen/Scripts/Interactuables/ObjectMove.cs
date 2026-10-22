using System.Collections;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public Vector3 moveDirection = Vector3.up;
    public float moveSpeed = 5f;
    public float timeMoving = 2f;

    [Header("Estado")]
    public bool isMoving = false;
    private float countingTime;


    void Start()
    {
        moveSpeed *= -1;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);

        countingTime -= Time.deltaTime;

        if (countingTime <= 0f)
        {
            countingTime = 0f;
            isMoving = false;
        }
    }


    public void StartMoving()
    {
        if (isMoving) return; 

        moveSpeed *= -1; // Invertir la dirección del movimiento
        countingTime = timeMoving;
        isMoving = true;
    }
}