using UnityEngine;

public class DiaperChangingBehavior : MonoBehaviour
{
    [Header("Animation:")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _animationSpeed = 1f;

    [Header("Colliders:")]
    [SerializeField] private Collider _leftStrap;
    [SerializeField] private Collider _rightStrap;
    [SerializeField] private Collider _frontStrap;

    [Header("Camera:")]
    [SerializeField] private Camera _camera;

    [Header("UI:")]
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;
    [SerializeField] private GameObject _frontArrow;

    [Header("Controller support:")]
    [SerializeField] private MinigameCursor _cursor;


    private Vector3 _lastMousePosition;
    private bool _isDragging = false;
    private bool _isMouseDown;
    private Collider _target;
    private PoopManager _poopManager;
    private bool _collidersEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _animator.speed = _animationSpeed;

        
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerFeedback();
        MouseCursorUpdates();

        if(_poopManager.CleanDiaperEquipped && _collidersEnabled == false)
        {
            _leftStrap.enabled = true;
            _rightStrap.enabled = true;
            _frontStrap.enabled = true;

            _collidersEnabled = true;
        }
    }

    private void Start()
    {
        _poopManager = FindFirstObjectByType<PoopManager>();
    }

    private Vector2 GetDragDirection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;
            _lastMousePosition = Input.mousePosition;
            _isDragging = true;
        }
        else if (_cursor.OnDownEvent())
        {
            _lastMousePosition = _cursor.GetPosition();
            _isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) || _cursor.OnUpEvent())
        {
            _isMouseDown = false;
            _isDragging = false;
            _rightArrow.SetActive(false);
            _leftArrow.SetActive(false);
            _frontArrow.SetActive(false);
        }

        if (_isDragging)
        {
            Vector3 currentMousePosition = Vector3.zero;

            if (_cursor.IsUsed()) currentMousePosition = _cursor.GetPosition();
            else currentMousePosition = Input.mousePosition;

            Vector3 dragDelta = currentMousePosition - _lastMousePosition;

            Vector2 dragDirection = Vector2.zero;

            if (dragDelta.sqrMagnitude > 0.01f)
            {
                dragDirection = dragDelta.normalized;
            }

            _lastMousePosition = currentMousePosition;

            return dragDirection;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private void MouseCursorUpdates()
    {
        if (_isMouseDown)
        {
            _poopManager.ChangeMouseCursor(2); // Dragging cursor
        }
        else if (_target == null || (_target.name != "UpperStrapLeft" && _target.name != "UpperStrapRight" && _target.name != "Front"))
        {
            _poopManager.ChangeMouseCursor(0); // Default cursor
        }
        
        else
        {
            _poopManager.ChangeMouseCursor(1); // Hover cursor
        }
    }

    private void OnDisable()
    {
        _poopManager.ChangeMouseCursor(0); // Default cursor
    }
    private void PlayerFeedback()
    {
        Vector2 dragDirection = GetDragDirection();

        RaycastHit hit;
        Vector3 clickPosition = Vector3.zero;

        if (_cursor.IsUsed()) clickPosition = _cursor.GetPosition();
        else clickPosition = Input.mousePosition;

        Ray ray = _camera.ScreenPointToRay(clickPosition);

        _target = null;

        if (Physics.Raycast(ray, out hit))
        {
            _target = hit.collider;
        }

        if (_target == _frontStrap)
        {
            if (dragDirection.y > 0)
            {
                _animator.SetBool("frontIsWorn", true);
                _isDragging = false;
            }

            if (dragDirection.y < 0)
            {
                _animator.SetBool("frontIsWorn", false);
                _isDragging = false;
            }
            if (_isDragging) _frontArrow.SetActive(true);
        }

        if (_target == _leftStrap)
        {
            if (dragDirection.x > 0)
            {
                _animator.SetBool("leftIsWorn", false);
                _isDragging = false;
            }

            if (dragDirection.x < 0)
            {
                _animator.SetBool("leftIsWorn", true);
                _isDragging = false;
            }
            if (_isDragging) _leftArrow.SetActive(true);
        }

        if (_target == _rightStrap)
        {
            if (dragDirection.x > 0)
            {
                _animator.SetBool("rightIsWorn", true);
                _isDragging = false;
            }

            if (dragDirection.x < 0)
            {
                _animator.SetBool("rightIsWorn", false);
                _isDragging = false;
            }

            if (_isDragging) _rightArrow.SetActive(true);
        }
    }

}
