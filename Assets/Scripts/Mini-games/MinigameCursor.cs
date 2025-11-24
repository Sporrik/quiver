using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinigameCursor : MonoBehaviour
{
    [SerializeField] private RawImage _cursor;

    [Header("Controls:")]
    [SerializeField] private InputActionAsset _input;
    [SerializeField] private RectTransform _bounds;
    [SerializeField] private float _speed = 25f;

    private InputAction _move;
    private InputAction _click;

    private Vector2 _position;
    private Vector2 _offset;

    private bool _isUsed = false;
    private float _countDown;

    private bool _didClick;
    private bool _didMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _move = _input.FindActionMap("MinigameCursor").FindAction("Move");
        _click = _input.FindActionMap("MinigameCursor").FindAction("Click");

        _offset = new Vector2(_bounds.rect.width / 2, _bounds.rect.height / 2);

        _position = _offset;
    }

    // Update is called once per frame
    private void Update()
    {
        const float timeUntilSleep = 5f;

        _didClick = Click();
        _didMove = MoveCursor();

        if(_didMove || _didClick)
        {
            _isUsed = true;
            _countDown = timeUntilSleep;
        }
        else
        {
            _countDown -= Time.deltaTime;
        }

        if(_countDown <= 0)
        {
            _isUsed = false;
        }
    }

    private bool MoveCursor()
    {
        if (_move == null)
        {
            Debug.Log("nope, not happening!");
            return false;
        }

        Vector2 added = _move.ReadValue<Vector2>();

        if (added.x == 0 || added.y == 0)
        {
            return false;
        }

        _position += added * Time.deltaTime * _speed;

        if(_position.x > _bounds.rect.width)  _position.x = _bounds.rect.width;
        else if (_position.x < 0) _position.x = 0;

        if (_position.y > _bounds.rect.height) _position.y = _bounds.rect.height;
        else if (_position.y < 0) _position.y = 0;

        _cursor.rectTransform.anchoredPosition = _position - _offset;

        return true;
    }

    private bool Click()
    {
        if (_click.IsPressed())
        {
            _cursor.color = Color.green;
            return true;
        }

        _cursor.color = Color.red;

        return false;        
    }

    private void OnMouseDown()
    {
        // disable when using mouse again
        _isUsed = false;
        _countDown = 0;
    }

    private void OnMouseUp()
    {
        // disable when using mouse again
        _isUsed = false;
        _countDown = 0;
    }

    public bool IsUsed()
    {
        return _isUsed;
    }

    public bool IsPressed()
    {
        return _didClick;
    }

    public Vector2 GetPosition()
    {
        return _position;
    }
}
