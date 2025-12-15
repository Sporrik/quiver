using System;
using System.Collections.Generic;
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

    //private Dictionary<GameObject, Coroutine> dotTimers = new Dictionary<GameObject, Coroutine>();


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
        Vector3 sweepDir =
       Quaternion.Euler(0f, 40f, 0f) * _radar.transform.forward;

        sweepDir.Normalize();
        Debug.DrawRay(radarCenter.position, sweepDir * 5f, Color.green);

        foreach (GameObject dot in dots)
        {
            // Now radar can be anywhere!
            Vector3 dirToDot = (dot.transform.position - _radar.transform.position).normalized;
            dirToDot.Normalize();
            sweepDir.Normalize();


            float dp = Vector3.Dot(sweepDir, dirToDot);

            if (dots[1] == dot)
            {
                Debug.Log("Dot 1 Info:");
                Debug.Log("sweepDir: " + sweepDir);
                Debug.Log("dirToDot: " + dirToDot);
                Debug.Log("Dot Product: " + dp);
                Debug.Log("");
            }
            //float threshold = Mathf.Cos(radarAngle * Mathf.Deg2Rad);

            dot.SetActive(dp >= 0.7f);
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