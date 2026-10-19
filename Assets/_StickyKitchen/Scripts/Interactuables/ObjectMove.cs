using System.Collections;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    public float moveX,moveY, moveZ;
    public float moveSpeed;

    public float timeMoving;
    public bool timerActive;
    public float countingTime;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool canMove = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpen = false;
        timerActive = false;
        canMove = false;
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

        if (canMove )
        {
            this.gameObject.transform.position = new Vector3(
                moveX += moveSpeed, moveY += moveSpeed, moveZ += moveSpeed);
        }
        //moveY += moveSpeed;
        //moveZ += moveSpeed;
    }

    public void StartMoving()
    {
        if (isMoving) return;
        moveSpeed *= -1;
        timerActive = true;
        canMove = true;
    }

    public void Timer()
    {
        if (!timerActive) return;

        countingTime = timeMoving;

        if (countingTime >= 1)
        {
            countingTime -= Time.deltaTime;
        }
        else if (countingTime <= 0)
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
