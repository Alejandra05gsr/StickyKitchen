using UnityEngine;

public class Nodo : MonoBehaviour
{
    bool isColliding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EliminateNodo();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isColliding = true;
        }
    }

    void EliminateNodo()
    {
        if (isColliding) return;
        this.gameObject.SetActive(false);
    }

}
