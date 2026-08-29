using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 6f;
    private bool canMove;

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

    [Header("Ground")]
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = true;
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        MoveCamera();
        Jump();

        // Presionar Shift Izquierdo para hacer Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate()
    {
        // No aplicar movimiento regular si estamos en mitad de un Dash
        if (!isDashing)
        {
            Move();
        }
    }

    void Move()
    {
        if (!canMove) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (transform.forward * vertical + transform.right * horizontal).normalized;

        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            // Limpiar la velocidad vertical antes de saltar para que el segundo salto en el aire se sienta uniforme
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
            isGrounded = false;
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // Obtener la dirección de entrada del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dashDir = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Si no está moviendo las teclas WASD, hacer el dash hacia adelante por defecto
        if (dashDir == Vector3.zero)
        {
            dashDir = transform.forward;
        }

        // Resetear la velocidad actual y aplicar fuerza de impulso instantánea
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        // Esperar el cooldown antes de poder usar el dash otra vez
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

    private void OnCollisionEnter(Collision collision)
    {
        // Reiniciar saltos al tocar el suelo
        isGrounded = true;
        jumpsRemaining = maxJumps;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}