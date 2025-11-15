using System.Runtime.CompilerServices;
using UnityEngine;

public class UITips : MonoBehaviour
{
    [SerializeField] private Vector3 _startSize = new Vector3(0.001f, 0.001f, 0.001f);
    [SerializeField] private Vector3 _maxSize = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private Vector3 _finalSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private float _speed = 2f;

    private bool _isAnimating = false;
    private bool _isMaxed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        _isAnimating = true;
        transform.localScale = _startSize;
        _isMaxed = false;
    }

    private void Update()
    {
        if ((_isAnimating))
        {
            if (!_isMaxed)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _maxSize, _speed * Time.deltaTime);
                if (transform.localScale.x >= _maxSize.x - 0.01f)
                    _isMaxed = true;
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _finalSize, Time.deltaTime * _speed);
                if(transform.localScale.x <= _finalSize.x - 0.01f)
                {
                    transform.localScale = _finalSize;
                    _isAnimating = false;
                }
            }
        }

        float pingPongValue = Mathf.PingPong(Time.time * _speed*0.75f, 5f);
        transform.localRotation = Quaternion.Euler(0, 0, pingPongValue);
    }
}
