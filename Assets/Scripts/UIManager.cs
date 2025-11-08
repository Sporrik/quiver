using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image[] _imageMeters;
    [SerializeField] private float[] _floatMeters;
    [SerializeField] private float[] _localScale;
    [SerializeField] private float[] _incrementValues;

    //[SerializeField] private float _maxTime;
    //[SerializeField] private GameObject emptyMeter2;
    private Vector3 _barIncreaseReversed;
    private float _timer;

    //private GameObject _player;
    //private GameObject _barManagerObject;
    //private BarManager _barManager;
    //private PlayerController _playerController;
    [SerializeField] private UIScriptableObject _uiData;

    [SerializeField] private Vector2 _localscaleYZ;

    void Start()
    {

        //_player = GameObject.FindGameObjectWithTag("Player");
        //_barManagerObject = GameObject.Find("BarManager");

        //_barManager = _barManagerObject.GetComponent<BarManager>();
        //_playerController = _player.GetComponent<PlayerController>();


        for (int i = 0; i < _imageMeters.Length; i++)
        {
            {
                _localScale[i] = _imageMeters[i].transform.localScale.x; // scale = scale of each meter

            }


            //_floatMeters[0] = (_barManager._angryMeter / 100);
            //_floatMeters[1] = (_barManager._poopMeter / 100);
            //_floatMeters[2] = (_barManager._sneezMeter / 100);
            // _floatMeters[3] = (_ / 100);  // IMPLEMENT PUBLIC STAMINAMETER
            // _playerController.stamina
        }
    }
    // Update is called once per frame
    void Update()
    {
        float[] meters = { _uiData.GetHapiness(), _uiData.GetPoop(), _uiData.GetHungry(), _uiData.GetPee(), _uiData.GetStamina() };
        for (int i = 0; i < meters.Length; i++)
            _floatMeters[i] = meters[i] / 100f;



        for (int i = 0; i < _imageMeters.Length; i++)
        {
            _barIncreaseReversed = new Vector3((1 - _floatMeters[i]) * _localScale[i], _localscaleYZ.x, _localscaleYZ.y); // make an empty on the angrymeter
            _imageMeters[i].rectTransform.localScale = _barIncreaseReversed;
        }
        Debug.Log(_barIncreaseReversed.x);


    }

    //setters (changes by Warre)
    public void IncrementPoop()
    {
        _uiData.IncrementPoop(_incrementValues[0]);
    }
    public void IncrementHapiness()
    {
        _uiData.IncrementHapiness(_incrementValues[1]);
    }
    public void IncrementHungry()
    {
        _uiData.IncrementHungry(_incrementValues[2]);
    }
    public void IncrementPee()
    {
        _uiData.IncrementPee(_incrementValues[3]);
    }
    public void IncrementStamina()
    {
        _uiData.IncrementStamina(_incrementValues[4]);
    }
}
