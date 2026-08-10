using UnityEngine;

public class FluidArea : MonoBehaviour
{
    [Header("Configuración de la Malla Cuadrada")]
    public GameObject cylinderPrefab; 
    public int gridSize = 5;          
    public float spacing = 0.5f;

    [Header("WaterColors")]
    public Color strongBlue = Color.blue;                      
    public Color lightBlue = new Color(0.6f, 0.8f, 1f);
    public bool isWaterFluid;

    [Header("OrangeColors")]
    public Color strongOrange = Color.orange;                      
    public Color lightOrange = new Color(1f, 0.6f, 0.2f);
    public bool isSyrupFluid;
    
    
   

    void Start()
    {
        GenerateAndColorGrid();
    }


    void GenerateAndColorGrid()
    {
        //-Crea un arreglo de nodos-
        GameObject[] allNodes = new GameObject[gridSize * gridSize];
        int index = 0;
        float halfSize = (gridSize - 1) * spacing / 2f;

        //-Crea la malla de cilindros-
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {

                Vector3 localPos = new Vector3((x * spacing) - halfSize, 0f, (z * spacing) - halfSize);
                GameObject node = Instantiate(cylinderPrefab, transform);
                node.transform.localPosition = localPos;

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

            float floorCloseness = 1f; 

            if (heightDifference > 0.001f)
            {
                floorCloseness = 1f - ((currentY - lowestY) / heightDifference);
            }

            //Peso de los nodos
            float nodeValue = floorCloseness * 100f;

            if (isWaterFluid)
            {
                //Colorea los nodos
                Renderer render = node.GetComponent<Renderer>();
                if (render != null)
                {
                    render.material.color = Color.Lerp(lightBlue, strongBlue, floorCloseness);
                }
            }
            else if (isSyrupFluid)
            {
                //Colorea los nodos
                Renderer render = node.GetComponent<Renderer>();
                if (render != null)
                {
                    render.material.color = Color.Lerp(lightOrange, strongOrange, floorCloseness);
                }
            }
        }
    }

}


