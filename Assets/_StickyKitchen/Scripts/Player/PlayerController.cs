using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public float moveSpeed;
    public Vector2 sensitivity;

    public Transform camera;


    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        Movement();
        CameraLook();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = (transform.forward * vertical + transform.right * horizontal).normalized;

            rb.linearVelocity = direction * moveSpeed;
        }

    }

    private void CameraLook()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        if (horizontal != 0)
        {
            transform.Rotate(0, horizontal * sensitivity.x, 0);
            transform.Rotate(0, vertical * sensitivity.y, 0);
        }


        if (vertical != 0)
        {
            Vector3 rotation = camera.localEulerAngles;
            rotation.x = (rotation.x - vertical * sensitivity.y + 360) % 360;
            if (rotation.x > 180 && rotation.x < 180)
            {
                rotation.x = 80;
            }
            else if (rotation.x < 280 && rotation.x > 180)
            {
                rotation.x = 280;
            }

            camera.localEulerAngles = rotation;
        }

    }

}
