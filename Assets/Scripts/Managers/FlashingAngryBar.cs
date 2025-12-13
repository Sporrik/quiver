using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class FlashingAngryBar : MonoBehaviour
{

    [SerializeField] private BarManager _barManager;
    [SerializeField] private Image _barToFlash;
    [SerializeField] private float _flashMaxTime = 1f;
    [SerializeField] private Color _flashColor;
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _flashTimer;
    [SerializeField] private bool _isFlashing = false;

    [SerializeField] private float _twoSecondDelayTimer;

    

    [SerializeField] private GameObject _colorOverlay;

    void Awake()
    {
        _barManager = GameObject.FindGameObjectWithTag("GameSystems").GetComponent<BarManager>();
        _barManager.OnBabyCrying += FlashLights;


    }

    private void FlashLights(BarManager manager)
    {
        if(!_isFlashing)
            _twoSecondDelayTimer = 0f;
        _isFlashing = true;

    }

    void Update()
    {
        _twoSecondDelayTimer += Time.deltaTime;
        if (_isFlashing && _twoSecondDelayTimer > 2f)
        {
            Debug.Log("SHOWTIME");
            _flashTimer += Time.deltaTime;

            if (_flashTimer > _flashMaxTime / 2)
            {
                _colorOverlay.SetActive(true);
            }

            //float progress = _flashTimer / _flashMaxTime;
            //progress = Mathf.Clamp01(progress);


            //_barToFlash.color = Color.black * (1f - progress) + _flashColor * progress;
            //Debug.Log("Current Color: " + _barToFlash.color);
            //Debug.Log("FlashColor: " + _flashColor);

            if(_flashTimer >= _flashMaxTime)
            {
                _colorOverlay.SetActive(false);
                _flashTimer = 0;

            }

        }
    }


}
