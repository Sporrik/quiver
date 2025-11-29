using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject[] dots;
    private float timer;
    [SerializeField] private float BlinkingTimeOff;
    [SerializeField] private float BlinkingTimeOn;

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
        if(timer > BlinkingTimeOff)
        {
            
            foreach (GameObject d in dots)
            {
                d.SetActive(true);
            }
            if(timer > BlinkingTimeOff + BlinkingTimeOn)
            {
                foreach(GameObject d in dots)
                {
                    d.SetActive(false);
                    timer = 0;
                }
            }



        }
    }
}
