using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject[] dots;
    private float timer;
    [SerializeField] private float BlinkingTimeOff;
    [SerializeField] private float BlinkingTimeOn;

    [SerializeField] Transform radarCenter;
    [SerializeField] float radarAngle = 5f; // how "wide" the sweep detects
    public Transform radarSweep;      // the rotating sweep line

    [SerializeField] private GameObject _radar;
    [SerializeField] private float _rotation;

    [SerializeField] private float rotSpeed = -40f;

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
        //UpdateCirlces();

        UpdatedAngle();


        RotateRadar();
    }

    private void RotateRadar()
    {

        _radar.transform.Rotate(rotSpeed * Time.deltaTime, 0f, 0f, Space.Self);
    }


    private void UpdatedAngle()
    {
        Vector3 sweepDir = _radar.transform.forward; // or correct axis

        foreach (GameObject dot in dots)
        {
            // Now radar can be anywhere!
            Vector3 dirToDot = (dot.transform.position - _radar.transform.position).normalized;

            float dp = Vector3.Dot(sweepDir, dirToDot);

            float threshold = Mathf.Cos(radarAngle * Mathf.Deg2Rad);

            dot.SetActive(dp > threshold);
        }
    }

    private void UpdateCirlces()
    {
        timer += Time.deltaTime;
        if (timer > BlinkingTimeOff)
        {
            foreach (GameObject d in dots)
            {
                d.SetActive(true);
            }
            if (timer > BlinkingTimeOff + BlinkingTimeOn)
            {
                foreach (GameObject d in dots)
                {
                    d.SetActive(false);
                    timer = 0;
                }
            }
        }
    }
}
