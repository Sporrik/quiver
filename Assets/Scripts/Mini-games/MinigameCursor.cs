using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinigameCursor : MonoBehaviour
{
    [Header("Visuals:")]
    [SerializeField] private RawImage _point;
    [SerializeField] private RawImage _hover;
    [SerializeField] private RawImage _grab;

    [Header("Controls:")]
    [SerializeField] private InputActionAsset _input;
    [SerializeField] private RectTransform _bounds;
    [SerializeField] private float _speed = 25f;

    [Header("raycast:")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;

    private InputAction _move;
    private InputAction _click;

    private Vector2 _position;
    private Vector2 _offset = Vector2.zero;

    private bool _isUsed = false;
    private float _countDown;

    private bool _isDown = false;
    private bool _hovering = false;
    private bool _wasDown = false;

    private bool _didMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _input.Enable();

        _move = _input.FindActionMap("MinigameCursor").FindAction("Move");
        _click = _input.FindActionMap("MinigameCursor").FindAction("Click");

        _offset = new Vector2(0, _bounds.rect.height );

        _position = new Vector2(_bounds.rect.width / 2, _bounds.rect.height / 2);

        _point.enabled = true;
        _hover.enabled = false;
        _grab.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        const float timeUntilSleep = 5f;

        _wasDown = _isDown;

        _isDown = Click();
        _didMove = MoveCursor();

        // disable cursor when not used
        if (_didMove || _isDown)
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

        // hide cursor when disabled
        if(_isUsed)
        {
            CheckHover();
            ShowCursor();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _point.enabled = false;
            _hover.enabled = false;
            _grab.enabled = false;
        }
    }

    private void ShowCursor()
    {
        if(_hovering && !_isDown)
        {
            _point.enabled = false;
            _hover.enabled = true;
            _grab.enabled = false;
        }
        else if( _isDown )
        {
            _point.enabled = false;
            _hover.enabled = false;
            _grab.enabled = true;
        }
        else
        {
            _point.enabled = true;
            _hover.enabled = false;
            _grab.enabled = false;
        }

        
    }

    private bool MoveCursor()
    {
        if (_move == null)
        {
            Debug.Log("No matching input action for MOVE!");
            return false;
        }

        Vector2 added = _move.ReadValue<Vector2>();

        if (added.sqrMagnitude == 0f)
        {
            return false;
        }

        _position += added * Time.deltaTime * _speed;

        if(_position.x > _bounds.rect.width)  _position.x = _bounds.rect.width;
        else if (_position.x < 0) _position.x = 0;

        if (_position.y > _bounds.rect.height) _position.y = _bounds.rect.height;
        else if (_position.y < 0) _position.y = 0;


        _point.rectTransform.anchoredPosition = _position - _offset;
        _hover.rectTransform.anchoredPosition = _position - _offset;
        _grab.rectTransform.anchoredPosition = _position - _offset;

        return true;
    }

    private bool Click()
    {
        if (_click.IsPressed())
        {
            //_point.color = Color.green;
            return true;
        }

        //_point.color = Color.red;

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
        return _isDown;
    }

    public Vector2 GetPosition()
    {
        //return _position;

        return 
            RectTransformUtility.WorldToScreenPoint
            (
            null,   // canvas is in screen space so no camera is needed
            _point.rectTransform.position
            );
    }

    public bool OnDownEvent()
    {
        return _isDown && !_wasDown;
    }

    public bool OnUpEvent()
    {
        return !_isDown && _wasDown;
    }

    public void Hover(bool enable)
    {
        _hovering = enable;
    }

    private void CheckHover()
    {
        _hovering = false;

        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(new Vector3(_position.x, _position.y, 0f));

        // check if the controller cursor is on the objects
        if (Physics.Raycast(ray, out hit, float.MaxValue, _layerMask))
        {
            _hovering = true;
        }

    }
}
