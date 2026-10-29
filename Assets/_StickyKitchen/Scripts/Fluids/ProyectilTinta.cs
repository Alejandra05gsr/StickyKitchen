using UnityEngine;

public class ProyectilTinta : MonoBehaviour
{
    public float velocidad = 25f;
    public int cantidadParticulasAInyectar = 50; // Cantidad de partículas SPH que "nacen" aquí
    public float radioDispersion = 0.3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Disparar(Vector3 direccion)
    {
        rb.linearVelocity = direccion * velocidad;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Obtener la posición de impacto
        Vector3 puntoImpacto = collision.contacts[0].point;
        Vector3 velocidadImpacto = rb.linearVelocity;

        // 2. Enviar los datos al Administrador SPH / PIC-FLIP
        //if (SPH.Instance != null)
        //{
        //    SPH.Instance.InyectarParticulas(puntoImpacto, velocidadImpacto, cantidadParticulasAInyectar, radioDispersion);
        //}

        // 3. Destruir la gota del disparo
        Destroy(gameObject);
    }
}