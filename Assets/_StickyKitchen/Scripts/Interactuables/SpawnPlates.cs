using UnityEngine;

public class SpawnPlates : MonoBehaviour
{
    public GameObject plates;
    public Transform spawnPoint;

    int timerCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawningPlates()
    {
        if (plates != null && spawnPoint != null)
        {
            Instantiate(plates, spawnPoint.position, spawnPoint.rotation);
        }
    }


}
