using UnityEngine;

public class Tanks : MonoBehaviour
{

    public Shoot playerShoot;
    public int typeTank;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TypeTank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TypeTank()
    {
        if(this.gameObject.CompareTag("FillWater") )
        {
            typeTank = 0;
        }
        else if (this.gameObject.CompareTag("FillSyrup"))
        {
            typeTank = 1;
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterAmmo") && typeTank == 0)
        {
            playerShoot.UpdateAmmoWater();
        }
        else if (other.CompareTag("SyrupAmmo") && typeTank == 1)
        {
            playerShoot.UpdateAmmoSyrup();
        }
    }

}
