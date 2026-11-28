using UnityEngine;

public class PlateSounds : MonoBehaviour
{
    public AudioSource dropPlate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        dropPlate.Play();
    }

}
