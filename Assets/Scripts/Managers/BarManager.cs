using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarManager : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI _poopMeterText;
    //[SerializeField] private TextMeshProUGUI _sneezMeterText;
    //[SerializeField] private TextMeshProUGUI _angryMeterText;

    [SerializeField] private GameObject _player;
    private PlayerController _playerController;

    //[SerializeField] public float _poopMeter;
    //[SerializeField] public float _hungerMeter;
    //[SerializeField] public float _peeMeter;
    //[SerializeField] public float _angryMeter;
    [SerializeField] private float TimeToGetRandomEvent = 1;

    [SerializeField] private float _amountToIncreaseBar;
    [SerializeField] private float _amountToIncreaseHapiness;

    [SerializeField] private GameObject[] Guards;

    [SerializeField] private UIScriptableObject _scriptableObject;

  //  [SerializeField] private float _sneezRange;
    [SerializeField] private float _cryRange;
    [SerializeField] private float _timeToGetAngry;


    private float _eventTimer;
    private float _happyTimer;
    void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();   
        Guards = GameObject.FindGameObjectsWithTag("Guard");
        Debug.Log(Guards);
        _playerController.OnStaminaChanged += OnStaminaChanged;
        //Physics.OverlapSphere()
    }

    private void OnStaminaChanged(float stamina, float max)
    {
        _scriptableObject.SetStamina(stamina);
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
        _eventTimer += Time.deltaTime;
        _happyTimer += Time.deltaTime;

        if(_eventTimer >= TimeToGetRandomEvent)
        {
            _eventTimer -= TimeToGetRandomEvent;
            RandomBarIncrease();

        }
        if(_scriptableObject.GetHungry() >= 100 && _happyTimer >= _timeToGetAngry)
        {
            _happyTimer = 0;
            _scriptableObject.IncrementHapiness(_amountToIncreaseHapiness);
        }
        if(_scriptableObject.GetPoop() >= 100 && _happyTimer >= _timeToGetAngry)
        {
            _happyTimer = 0;
            _scriptableObject.IncrementHapiness(_amountToIncreaseHapiness);

        }
        if (_scriptableObject.GetPee() >= 100 && _happyTimer >= _timeToGetAngry)
        {
            _happyTimer = 0;
            _scriptableObject.IncrementHapiness(_amountToIncreaseHapiness);

        }

        //if(_scriptableObject.GetHapiness() >= 100)
        //{
        //    AlertGuard();
        //}

        //_poopMeterText.text = $"Poop: {_poopMeter}";
        //_sneezMeterText.text = $"Sneez: {_hungerMeter}";
        //_angryMeterText.text = $"Angry: {_angryMeter}";
    }


    private void RandomBarIncrease()
    {
        int num = UnityEngine.Random.Range(1, 4);
        switch (num)
        {
            case 1:
                _scriptableObject.IncrementPoop(_amountToIncreaseBar);
                break;
            case 2:
                _scriptableObject.IncrementHungry(_amountToIncreaseBar);
                break;
            case 3:
                _scriptableObject.IncrementPee(_amountToIncreaseBar);
                break;
            default:
                Debug.Log("LITTLE PROBLEM");
                break;
        }
    }

    //private void AlertGuard()
    //{
    //    Debug.Log("Alert");
    //    foreach (var guard in Guards)
    //    {
    //        GuardBehavior b = guard.GetComponent<GuardBehavior>();
    //        b.AlertGuardsToPosition(_cryRange);
    //    }
    //}
    //private void Poop()
    //{
    //    //throw new NotImplementedException();
    //    //SceneManager.LoadScene("Diaper", LoadSceneMode.Single);
    //}
    //private void Pee()
    //{
    //    //throw new NotImplementedException();
    //}
    //private void Hunger()
    //{
    //    //throw new NotImplementedException();
    //}
}
