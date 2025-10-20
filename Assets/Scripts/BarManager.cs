using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _poopMeterText;
    [SerializeField] private TextMeshProUGUI _sneezMeterText;
    [SerializeField] private TextMeshProUGUI _angryMeterText;

    [SerializeField] private GameObject _player;

    [SerializeField] private float _poopMeter;
    [SerializeField] private float _sneezMeter;
    [SerializeField] private float _angryMeter;
    [SerializeField] private float TimeToGetRandomEvent = 5;

    [SerializeField] private GameObject[] Guards;

    [SerializeField] private float _sneezRange;
    [SerializeField] private float _cryRange;

    private float Timer;
    void Start()
    {
        
        Guards = GameObject.FindGameObjectsWithTag("Guard");
        Debug.Log(Guards);

        //Physics.OverlapSphere()
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.P))
        //    _poopMeter++;
        
        //if (Input.GetKey(KeyCode.S))
        //    _sneezMeter++;

        //if (Input.GetKey(KeyCode.A))
        //    _angryMeter++;

        Timer += Time.deltaTime;
        if(Timer >= TimeToGetRandomEvent)
        {
            Timer -= TimeToGetRandomEvent;
            RandomBarIncrease();

        }


        if (Input.GetKeyUp(KeyCode.Z))
        {
            Debug.Log("STOP CRYING");
            _angryMeter = 0;
        }

        if(_angryMeter >= 100)
        {
            _angryMeter = Math.Min(_angryMeter, 100);
            AlertGuard(_cryRange);
        }
        if (_sneezMeter >= 100)
        {
            _sneezMeter = 0;
            AlertGuard(_sneezRange);
        }
        if (_poopMeter >= 100)
        {
            _poopMeter = 0;
            Poop();
        }

        _poopMeterText.text = $"Poop: {_poopMeter}";
        _sneezMeterText.text = $"Sneez: {_sneezMeter}";
        _angryMeterText.text = $"Angry: {_angryMeter}";
    }

    private void RandomBarIncrease()
    {
        int num = UnityEngine.Random.Range(0, 3);
        switch (num)
        {
            case 0:
                _angryMeter++;
                break;
            case 1:
                //_poopMeter++;
                break;
            case 2:
                _sneezMeter++;
                break;
            default:
                Debug.Log("LITTLE PROBLEM");
                break;
        }
    }

    private void AlertGuard(float range)
    {
        Debug.Log("Alert");
        foreach (var guard in Guards)
        {
            GuardBehavior b = guard.GetComponent<GuardBehavior>();
            b.AlertGuardsToPosition(range);
        }
    }
    private void Poop()
    {
        SceneManager.LoadScene("Diaper", LoadSceneMode.Single);
    }
}
