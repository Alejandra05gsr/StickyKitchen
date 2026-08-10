using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movimiento")]
    public float moveForce = 20f;
    public float maxSpeed = 5f;
    public float aimMoveForce = 10f;
    public float aimMaxSpeed = 2.5f;

    [Header("Jump")]
    public float jumpForce = 7f;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;



    [Header("Rotación y Cámara")]
    public Transform cameraTransform; 
    [SerializeField] private float playerRotateDampening = 0.1f;
    private float turnSmoothingVelocity;


    [Header("Mouse Camera")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float verticalLookLimit = 70f;

    private float cameraYaw;
    private float cameraPitch;


    [Header("Cameras")]
    public CinemachineCamera normalCamera;
    public CinemachineCamera aimCamera;

    [Header("UI / Retícula")]
    public GameObject crosshairUI;

    private Vector3 movementDirection;


    private bool isAiming = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Bloqueamos y ocultamos el cursor para que Cinemachine lea el mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            cameraYaw = cameraTransform.eulerAngles.y;
            cameraPitch = cameraTransform.eulerAngles.x;
        }

        SetAimingState(false);
    }

    void Update()
    {
        HandleAiming();

        HandleCameraRotation();

        GetMovementInput();

        if (isAiming)
        {
            HandleShooterRotation();
        }
        else
        {
            HandleThirdPersonRotation();
        }

        // Detectar salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude < 0.1f)
        {
            movementDirection = Vector3.zero;
            return;
        }

        if (isAiming)
        {
            // Movimiento relativo a la dirección del personaje
            movementDirection =
                transform.forward * vertical +
                transform.right * horizontal;
        }
        else
        {
            // Movimiento relativo a la cámara
            float targetAngle =
                Mathf.Atan2(inputDirection.x, inputDirection.z) *
                Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            movementDirection =
                Quaternion.Euler(0f, targetAngle, 0f) *
                Vector3.forward;
        }

        movementDirection.Normalize();
    }

    private void Movement()
    {
        if (movementDirection == Vector3.zero)
            return;

        float force = isAiming ? aimMoveForce : moveForce;
        float maxSpeed = isAiming ? aimMaxSpeed : this.maxSpeed;

        // Aplicamos una fuerza al Rigidbody
        rb.AddForce(
            movementDirection * force,
            ForceMode.Force
        );

        // Limitamos únicamente la velocidad horizontal
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity =
                horizontalVelocity.normalized * maxSpeed;

            rb.linearVelocity = new Vector3(
                horizontalVelocity.x,
                rb.linearVelocity.y,
                horizontalVelocity.z
            );
        }
    }


    private void Jump()
    {
        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }


    private bool IsGrounded()
    {
        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }



    // Rotación suave del personaje SOLO cuando se mueve
    private void HandleThirdPersonRotation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f &&
            cameraTransform != null)
        {
            float targetAngle =
                Mathf.Atan2(inputDirection.x, inputDirection.z) *
                Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            float smoothTargetAngle =
                Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref turnSmoothingVelocity,
                    playerRotateDampening
                );

            transform.rotation =
                Quaternion.Euler(
                    0f,
                    smoothTargetAngle,
                    0f
                );
        }
    }

    // El cuerpo se alinea con la dirección de la vista al apuntar
    private void HandleShooterRotation()
    {
        if (cameraTransform == null)
            return;

        float cameraYaw =
            cameraTransform.eulerAngles.y;

        transform.rotation =
            Quaternion.Euler(
                0f,
                cameraYaw,
                0f
            );
    }

    private void HandleCameraRotation()
    {
        if (cameraTransform == null)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cameraYaw += mouseX * mouseSensitivity;
        cameraPitch -= mouseY * mouseSensitivity;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            -verticalLookLimit,
            verticalLookLimit
        );

        cameraTransform.rotation = Quaternion.Euler(
            cameraPitch,
            cameraYaw,
            0f
        );
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

        if (normalCamera != null &&
            aimCamera != null)
        {
            normalCamera.Priority =
                isAiming ? 0 : 10;

            aimCamera.Priority =
                isAiming ? 10 : 0;
        }

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(isAiming);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }


}
