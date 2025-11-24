using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject[] dots;
    private float timer;
    [SerializeField] private float BlinkingTime;
    void Start()
    {
        gameObject.SetActive(true);
        foreach (GameObject d in dots)
        {
            d.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > BlinkingTime)
        {
            
            foreach (GameObject d in dots)
            {
                d.SetActive(true);
            }
            if(timer > BlinkingTime + 1)
            {
                foreach(GameObject d in dots)
                {
                    d.SetActive(false);
                    BlinkingTime = 0;
                }
            }



        }
    }
}
