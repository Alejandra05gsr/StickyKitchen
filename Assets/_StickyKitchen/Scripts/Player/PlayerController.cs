using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movimiento")]
    public float moveSpeed = 5;
    public float aimMoveSpeed = 2.5f;
    public Vector2 sensitivity = new Vector2(2f, 2f);

    [Header("Cameras")]
    public Transform cameraTransform;
    public CinemachineCamera normalCamera; 
    public CinemachineCamera aimCamera;

    [Header("UI / Retícula")]
    public GameObject crosshairUI;

    private bool isAiming = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        SetAimingState(false);
    }


    void Update()
    {
        HandleAiming();
        Movement();
        CameraLook();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float speed = isAiming ? aimMoveSpeed : moveSpeed;

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = (transform.forward * vertical + transform.right * horizontal).normalized;

            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {         
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

    }

    private void CameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0)
        {
            transform.Rotate(0, mouseX * sensitivity.x, 0);
        }


        if (mouseY != 0 && cameraTransform != null)
        {
            Vector3 rotation = cameraTransform.localEulerAngles;

            float newX = rotation.x - (mouseY * sensitivity.y);

            if (newX > 180) newX -= 360;
            newX = Mathf.Clamp(newX, -85f, 85f);

            cameraTransform.localEulerAngles = new Vector3(newX, 0, 0);
        }

    }

    private void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetAimingState(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SetAimingState(false);
        }
    }

    private void SetAimingState(bool state)
    {
        isAiming = state;

        if (normalCamera != null && aimCamera != null)
        {
            normalCamera.Priority = isAiming ? 0 : 10;
            aimCamera.Priority = isAiming ? 10 : 0;
        }

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(isAiming);
        }
    }

}
