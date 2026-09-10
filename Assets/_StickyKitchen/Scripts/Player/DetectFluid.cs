using UnityEngine;

public class DetectFluid : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    [SerializeField] private float normalSpeed = 8f;
    [SerializeField] private float normalJump = 5f;

    [Header("Modificadores de Fluido (Pseudofísicas)")]
    [SerializeField] private float waterSpeedMultiplier = 1.3f; // Desliza/aumenta velocidad
    [SerializeField] private float syrupSpeedMultiplier = 0.4f; // Resistencia de viscosidad
    [SerializeField] private float syrupJumpMultiplier = 0.5f;  // Dificulta el salto

    [Header("Raycast Ground Check")]
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask groundLayer;

    // Variables públicas de lectura para tu script de movimiento actual
    public float CurrentSpeed { get; private set; }
    public float CurrentJump { get; private set; }

    private void Start()
    {
        ResetStats();
    }

    private void Update()
    {
        CheckSurfaceUnderPlayer();
    }

    private void CheckSurfaceUnderPlayer()
    {
        // Se calcula el origen ligeramente arriba de los pies del personaje por si el pivote está en el suelo
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        // Si la LayerMask está en "Nothing", usará todas las capas excepto al propio Player
        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask finalMask = groundLayer == 0 ? ~(1 << playerLayer) : groundLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, finalMask))
        {
            // Dibuja la línea verde/amarilla si toca superficie
            if (hit.collider.CompareTag("Syrup"))
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, new Color(0.6f, 0.3f, 0f), 0.1f); // Marrón para Jarabe
                CurrentSpeed = normalSpeed * syrupSpeedMultiplier;
                CurrentJump = normalJump * syrupJumpMultiplier;
                Debug.Log("PISANDO JARABE: " + hit.collider.gameObject.name);
                return;
            }
            else if (hit.collider.CompareTag("Water"))
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.cyan, 0.1f); // Azul para Agua
                CurrentSpeed = normalSpeed * waterSpeedMultiplier;
                CurrentJump = normalJump;
                Debug.Log("PISANDO AGUA: " + hit.collider.gameObject.name);
                return;
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.yellow, 0.1f); // Amarillo para Suelo Normal
                // Uncomment para depurar qué objeto está detectando:
                // Debug.Log("TOCANDO SUELO NORMAL: " + hit.collider.gameObject.name + " | Tag: " + hit.collider.tag);
            }
        }
        else
        {
            // Dibuja una línea roja si el rayo no alcanza a tocar el suelo (está en el aire)
            Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red, 0.1f);
        }

        ResetStats();
    }

    private void ResetStats()
    {
        CurrentSpeed = normalSpeed;
        CurrentJump = normalJump;
    }
}
