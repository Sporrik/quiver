using UnityEngine;

public class Sandwhich : MonoBehaviour
{
    public float Speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "BabyModel")
        {
            Destroy(gameObject);
        }
    }
}
