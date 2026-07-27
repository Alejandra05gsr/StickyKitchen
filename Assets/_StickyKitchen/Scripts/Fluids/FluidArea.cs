using UnityEngine;

public class FluidArea : MonoBehaviour
{
    public Vector3 center;

    public Vector3 normal;

    public float radius = 1;
    public float maxRadius = 3f;

    public float viscosity;

    public float concentration = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
