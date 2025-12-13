using UnityEngine;

public class AnimationKeyCard : MonoBehaviour
{

    [SerializeField] GameObject KeyCardUI;
    private float _timer;
    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private GameObject _endPosition;
    [SerializeField] private bool _isAnimating;
    void Start()
    {
        
    }

    void Update()
    {
        if (_isAnimating)
        {

            _timer += Time.deltaTime;
            float _progress = _timer/_animationTime;
            KeyCardUI.transform.position = Vector3.Slerp(_startPosition, _endPosition.transform.position, _progress);
        }
    }

    public void AnimateKeyCard()
    {
        _timer = 0;
        _isAnimating = true;
        Debug.Log("ANIMATE KEYCARD");
        KeyCardUI.SetActive(true);


    }
}
