using UnityEngine;

public class StuckInteractuable : MonoBehaviour
{
    public Material offMat;
    public Material onMat;
    public Material stuckMat1;
    public Material stuckMat2;

    int typeMaterial = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaterialTurnOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MaterialTurnOff()
    {
        typeMaterial = 0;
        GetComponent<Renderer>().material = offMat;
    }

    public void MaterialTurnOn()
    {
        typeMaterial = 1;
        GetComponent<Renderer>().material = onMat;
    }

    public void Changematerial(int start)
    {
        typeMaterial = start;
        switch (typeMaterial)
        {
            case 2:
                GetComponent<Renderer>().material = stuckMat1;
                typeMaterial = 3;
                break;
            case 3:
                GetComponent<Renderer>().material = stuckMat2;
                typeMaterial = 2;
                break;
        }
    }


}
