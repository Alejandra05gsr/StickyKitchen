using UnityEngine;

public class PoolRange : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    private float timer = 0f;

    private void OnEnable()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            gameObject.SetActive(false); // Regresa al pool
        }
    }
}
