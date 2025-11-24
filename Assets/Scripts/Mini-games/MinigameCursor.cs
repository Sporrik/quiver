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
        MoveCursor();
        Click();
    }

    private void MoveCursor()
    {
        if (_move == null)
        {
            Debug.Log("nope, not happening!");
            return;
        }

        Vector2 added = _move.ReadValue<Vector2>();

        _position += added * Time.deltaTime * _speed;

        if(_position.x > _bounds.rect.width)  _position.x = _bounds.rect.width;
        else if (_position.x < 0) _position.x = 0;

        if (_position.y > _bounds.rect.height) _position.y = _bounds.rect.height;
        else if (_position.y < 0) _position.y = 0;

        _cursor.rectTransform.anchoredPosition = _position - _offset;
    }

    private void Click()
    {
        if (_click.IsPressed())
        {
            _cursor.color = Color.green;
        }
        else
        {
            _cursor.color = Color.red;
        }
    }
}
