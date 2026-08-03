using UnityEngine;

public class FluidArea : MonoBehaviour
{
    [Header("Configuración de la Malla Cuadrada")]
    public GameObject cylinderPrefab; 
    public int gridSize = 5;          
    public float spacing = 0.5f;

    [Header("Colors")]
    public Color strongBlue = Color.blue;                      // Closer to the floor
    public Color lightBlue = new Color(0.6f, 0.8f, 1f);        // Farther from the floor
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateAndColorGrid();
    }


    void GenerateAndColorGrid()
    {
        //-Crea un arreglo de nodos-
        GameObject[] allNodes = new GameObject[gridSize * gridSize];
        int index = 0;

        // Calculate offset so the grid remains centered at the hit position
        float halfSize = (gridSize - 1) * spacing / 2f;

        //-Crea la malla de cilindros-
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Local position on the grid plane
                Vector3 localPos = new Vector3((x * spacing) - halfSize, 0f, (z * spacing) - halfSize);

                // Instantiate cylinder as a child of this object
                GameObject node = Instantiate(cylinderPrefab, transform);
                node.transform.localPosition = localPos;

                // Save reference in our array
                allNodes[index] = node;
                index++;
            }
        }

        //-Encuentra en Y el nodo mas cerca y lejos del piso-
        float lowestY = float.MaxValue;
        float highestY = float.MinValue;

        foreach (GameObject node in allNodes)
        {
            float currentY = node.transform.position.y;
            if (currentY < lowestY) lowestY = currentY;
            if (currentY > highestY) highestY = currentY;
        }

        float heightDifference = highestY - lowestY;

        //-Agregamos color a cada nodo basado en su cercania al piso-
        foreach (GameObject node in allNodes)
        {
            float currentY = node.transform.position.y;

            // Calculate floor closeness (from 0.0 to 1.0)
            float floorCloseness = 1f; // Default for flat horizontal surfaces

            if (heightDifference > 0.001f)
            {
                // The lower the node in world Y, the closer this value gets to 1.0
                floorCloseness = 1f - ((currentY - lowestY) / heightDifference);
            }

            // --- WEIGHT VALUE ---
            // Scaled from 0 (farther from floor) to 100 (closer to floor)
            float nodeValue = floorCloseness * 100f;


            // --- COLOR IN BLUE GRADIENT ---
            Renderer render = node.GetComponent<Renderer>();
            if (render != null)
            {
                // Interpolates between light blue and strong blue
                render.material.color = Color.Lerp(lightBlue, strongBlue, floorCloseness);
            }
        }
    }

}


