using System.Collections;
using UnityEngine;

public class FluidArea : MonoBehaviour
{
    [System.Serializable]
    public enum FluidAreaType { Water, Syrup }

    [Header("Tipo de Fluido")]
    public FluidAreaType areaType = FluidAreaType.Water;

    [Header("Desaparición del Agua")]
    [SerializeField] private float waterDuration = 4f;    // Tiempo que dura el agua estática
    [SerializeField] private float fadeOutDuration = 2f; // Tiempo que tarda en secarse/desaparecer

    private Renderer fluidRenderer;
    private Material fluidMaterial;

    private void Awake()
    {
        fluidRenderer = GetComponent<Renderer>();
        if (fluidRenderer != null)
        {
            fluidMaterial = fluidRenderer.material;
        }
    }

    private void Start()
    {
        // El agua se autodestruye con el tiempo; el jarabe NO (permanece infinito)
        if (areaType == FluidAreaType.Water)
        {
            StartCoroutine(DisappearWaterRoutine());
        }
    }

    /// <summary>
    /// Destruye el objeto inmediatamente (usado por la limpieza de Agua sobre Jarabe)
    /// </summary>
    public void DisapearFluid()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Transición de desaparición progresiva para el agua
    /// </summary>
    private IEnumerator DisappearWaterRoutine()
    {
        yield return new WaitForSeconds(waterDuration);

        float timer = 0f;
        bool hasFillProperty = fluidMaterial != null && fluidMaterial.HasProperty("_Fill");
        float startFill = hasFillProperty ? fluidMaterial.GetFloat("_Fill") : 1f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;

            if (hasFillProperty)
            {
                float currentFill = Mathf.Lerp(startFill, -1f, progress);
                fluidMaterial.SetFloat("_Fill", currentFill);
            }
            else
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}