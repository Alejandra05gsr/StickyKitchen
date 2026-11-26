using UnityEngine;

public class OvenFood : MonoBehaviour
{
    public GameObject flour, sugar, butter, eggs, chocolate, honey;


    public GameObject muffin;
    public GameObject cookies;

    public GameObject failCook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Cook()
    {

        //this.gameObject.GetComponent<Tray>().Desa
        if (flour.activeInHierarchy && sugar.activeInHierarchy && honey.activeInHierarchy) //Muffins
        {
            muffin.SetActive(true);
        }
        else if (flour.activeInHierarchy && sugar.activeInHierarchy && butter.activeInHierarchy && eggs.activeInHierarchy && chocolate.activeInHierarchy) //Cookies
        {
            cookies.SetActive(true);
        }
        else
        {
            failCook.SetActive(true);
        }



    }





}
