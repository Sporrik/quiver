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


    private Vector3 _lastMousePosition;
    private bool _isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _animator.speed = _animationSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 dragDirection = GetDragDirection();

        Ray ray;
        RaycastHit hit;

        ray = _camera.ScreenPointToRay(Input.mousePosition);

        Collider target = null;

        if (Physics.Raycast(ray, out hit))
        {
            //print(hit.collider.name);

            target = hit.collider;
        }

        if(target == _frontStrap)
        {
            if(dragDirection.y > 0)
            {
                _animator.SetBool("frontIsWorn", true);
                _isDragging = false;
            }

            if (dragDirection.y < 0)
            {
                _animator.SetBool("frontIsWorn", false);
                _isDragging = false;
            }
        }

        if (target == _leftStrap)
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
        }

        if (target == _rightStrap)
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
        }
    }

    private Vector2 GetDragDirection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
            _isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
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

}
