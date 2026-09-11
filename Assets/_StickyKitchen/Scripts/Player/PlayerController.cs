using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Base Movement")]
    public float baseMoveSpeed = 8f;
    public float baseJumpForce = 6f;

    [Header("Modifiers")]
    public float syrupSpeedMultiplier = 0.4f;
    public float syrupJumpMultiplier = 0.5f;
    public float waterSpeedMultiplier = 1.4f;

    // Flags que activa FluidSurface.cs
    [HideInInspector] public bool isSteppingOnSyrup;
    [HideInInspector] public bool isSteppingOnWater;

    [Header("Jump & Double Jump")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Dash Settings")]
    public float dashForce = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    [Header("Camera")]
    public Transform playerCamera;
    public Transform gunTransform;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        MoveCamera();
        Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Calcular la velocidad según sobre qué colisión está parado
        float currentSpeed = baseMoveSpeed;

        if (isSteppingOnSyrup)
        {
            currentSpeed *= syrupSpeedMultiplier;
        }
        else if (isSteppingOnWater)
        {
            currentSpeed *= waterSpeedMultiplier;
        }

        rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            float currentJump = baseJumpForce;

            if (isSteppingOnSyrup)
            {
                currentJump *= syrupJumpMultiplier;
            }

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * currentJump, ForceMode.Impulse);

            jumpsRemaining--;
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dashDir = (transform.forward * vertical + transform.right * horizontal).normalized;

        if (dashDir == Vector3.zero)
        {
            dashDir = transform.forward;
        }

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (playerCamera != null)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            playerCamera.localEulerAngles = new Vector3(xRotation, 0f, 0f);

            if (gunTransform != null)
            {
                gunTransform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Reiniciar los saltos al colisionar con cualquier superficie que esté debajo de los pies
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                jumpsRemaining = maxJumps;
                break;
            }
        }
    }
}