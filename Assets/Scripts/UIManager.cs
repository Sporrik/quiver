using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image[] _imageMeters;
    [SerializeField] private float[] _floatMeters;
    
    //[SerializeField] private float _maxTime;
    //[SerializeField] private GameObject emptyMeter2;
    private Vector3 _localScale;
    private float _timer;
    private Vector3 _startScale;
    private GameObject _player;
    private GameObject _barManagerObject;
    private BarManager _barManager;
    private PlayerController _playerController;





    void Start()
    {
        
        _player = GameObject.FindGameObjectWithTag("Player");
        _barManagerObject = GameObject.Find("BarManager");

        _barManager = _barManagerObject.GetComponent<BarManager>();
        _playerController =  _player.GetComponent<PlayerController>();

       _startScale = _imageMeters[0].transform.localScale; // scale = first empty scale
        

        //_floatMeters[0] = (_barManager._angryMeter / 100);
        //_floatMeters[1] = (_barManager._poopMeter / 100);
        //_floatMeters[2] = (_barManager._sneezMeter / 100);
        _floatMeters[3] = (_ / 100);  // IMPLEMENT PUBLIC STAMINAMETER
        _playerController.stamina



        
    }

    // Update is called once per frame
    void Update()
    {
        float[] meters = { _barManager._sneezMeter, _barManager._angryMeter, _barManager._poopMeter };
        for (int i = 0; i < meters.Length; i++)
            _floatMeters[i] = meters[i] / 100f;

        

        for(int i = 0; i < _imageMeters.Length; i++)
        {
            _localScale = new Vector3((1 - _floatMeters[i]) * _startScale.x ,_startScale.y ,_startScale.z); // make an empty on the angrymeter
            _imageMeters[i].rectTransform.localScale = _localScale;
        }
        Debug.Log(_localScale.x);
        
            
    }
}
