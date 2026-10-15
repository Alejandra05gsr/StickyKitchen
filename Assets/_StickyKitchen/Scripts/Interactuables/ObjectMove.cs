using System.Collections;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{

    public float moveX,moveY, moveZ;
    public float moveSpeed;

    public float timeMoving;
    bool timerActive;
    float countingTime;

    private bool isOpen = false;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpen = false;
        timerActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        Move();
    }

    public void Move()
    {
        if (!timerActive) return;

        //this.gameObject.transform.position.x += moveX(moveSpeed);
        moveY += moveSpeed;
        moveZ += moveSpeed;

    }

    public void StartMoving()
    {
        if (isMoving) return;
        moveSpeed *= -1;
        timerActive = true;
    }

    public void Timer()
    {
        if (!timerActive) return;

        countingTime = timeMoving;

        if (countingTime >= 1)
        {
            countingTime += Time.deltaTime;
        }
        else
        {
            countingTime = 0;
            timerActive= false;
            isMoving = false;
        }
    }


    //Se mueva con animación
    //IEnumerator MovingObject()
    //{
    //    isMoving = true;

    //}
}
