using UnityEngine;

public class FluidSurface : MonoBehaviour
{
    public enum FluidType { Syrup, Water }

    [Header("Tipo de Fluido")]
    public FluidType fluidType = FluidType.Syrup;

    private void OnCollisionStay(Collision collision)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                if (fluidType == FluidType.Syrup)
                {
                    player.isSteppingOnSyrup = true;
                }
                else if (fluidType == FluidType.Water)
                {
                    player.isSteppingOnWater = true;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                if (fluidType == FluidType.Syrup)
                {
                    player.isSteppingOnSyrup = false;
                }
                else if (fluidType == FluidType.Water)
                {
                    player.isSteppingOnWater = false;
                }
            }
        }
    }
}