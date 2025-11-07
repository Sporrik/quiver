using UnityEngine;

public class DiaperChangingBehavior : MonoBehaviour
{
    [SerializeField] private Collider _leftStrap;
    [SerializeField] private Collider _rightStrap;
    [SerializeField] private Collider _frontStrap;

    [SerializeField] private Camera _camera;

    private Animator _animator;

    private Vector3 _lastMousePosition;
    private bool _isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator = GetComponent<Animator>();
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
            }

            if (dragDirection.y < 0)
            {
                _animator.SetBool("frontIsWorn", false);
            }
        }

        if (target == _leftStrap)
        {
            if (dragDirection.x > 0)
            {
                _animator.SetBool("leftIsWorn", false);
            }

            if (dragDirection.x < 0)
            {
                _animator.SetBool("leftIsWorn", true);
            }
        }

        if (target == _rightStrap)
        {
            if (dragDirection.x > 0)
            {
                _animator.SetBool("rightIsWorn", true);
            }

            if (dragDirection.x < 0)
            {
                _animator.SetBool("rightIsWorn", false);
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
