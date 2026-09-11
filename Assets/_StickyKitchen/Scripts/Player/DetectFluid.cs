using UnityEngine;

public class DetectFluid : MonoBehaviour
{
    [Header("Configuración del Jugador Base")]
    [SerializeField] private float normalSpeed = 8f;
    [SerializeField] private float normalJump = 6f;

    [Header("Modificadores de Fluido")]
    [SerializeField] private float waterSpeedMultiplier = 1.4f;
    [SerializeField] private float syrupSpeedMultiplier = 0.4f;
    [SerializeField] private float syrupJumpMultiplier = 0.5f;

    [Header("Raycast Ground Check")]
    [SerializeField] private float rayDistance = 1.2f; // Reducido para evitar lecturas falsas en el aire
    [SerializeField] private LayerMask groundLayer;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        CheckSurfaceUnderPlayer();
    }

    private void CheckSurfaceUnderPlayer()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask finalMask = groundLayer == 0 ? ~(1 << playerLayer) : groundLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, finalMask))
        {
            if (hit.collider.CompareTag("Syrup"))
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, new Color(0.6f, 0.3f, 0f), 0.1f);
                ApplyStats(normalSpeed * syrupSpeedMultiplier, normalJump * syrupJumpMultiplier);
                return;
            }
            else if (hit.collider.CompareTag("Water"))
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.cyan, 0.1f);
                ApplyStats(normalSpeed * waterSpeedMultiplier, normalJump);
                return;
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.yellow, 0.1f);
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red, 0.1f);
        }

        ResetStats();
    }

    private void ApplyStats(float speed, float jump)
    {
        if (playerController != null)
        {
            playerController.baseMoveSpeed = speed;
            playerController.baseJumpForce = jump;
        }
    }

    private void ResetStats()
    {
        ApplyStats(normalSpeed, normalJump);
    }
}