using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Vector2 _input;
    private CharacterController _characterController;
    [SerializeField] private float _baseSpeed;
    private float _currentSpeed;

    [SerializeField] private float _rotationSpeed;
    private Vector3 _direction;
    private Camera _mainCamera;

    private float _gravity = -9.81f;
    [SerializeField] private float _gravityMultiplier;
    private float _velocity;
    [SerializeField] private float _jumpPower;

    //Player animation states
    private Animator _playerAnimator;
    private string _currentState;

    [SerializeField] private Transform _groundPosition;
    [SerializeField] private float _groundRadius;
    [SerializeField] private LayerMask _groundLayer;

    private bool _isSneaking;
    private bool _isSprinting;

    [SerializeField] private float _sneakSpeedMulti;
    [SerializeField] private float _sprintSpeedMulti;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _characterController.enabled = true;
        _mainCamera = Camera.main;

        _currentSpeed = _baseSpeed;
        _playerAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        ApplyRotation();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ApplyMovement();

        Debug.Log(IsGrounded());
    }

    private bool IsGrounded()
    {
        Collider[] collider = Physics.OverlapSphere(_groundPosition.position, _groundRadius, _groundLayer);

        return collider.Length != 0;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        //Adjust movement direction in relation to camera rotation
        _direction = Quaternion.Euler(0f, _mainCamera.transform.eulerAngles.y, 0f) * new Vector3(_input.x, 0f, _input.y);
        var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity < 0.1f)
        {
            _velocity = 0f;
        }
        else
        {
            _velocity += _gravity * _gravityMultiplier * Time.deltaTime;
            _direction.y = _velocity;
        }
    }

    private void ApplyMovement()
    {
        _characterController.Move(_direction * _currentSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded()) return;

        _velocity += _jumpPower;
    }

    public void Sneak(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isSneaking = true;
        }
        else if (context.canceled)
        {
            _isSneaking = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isSprinting = true;
        }
        else if (context.canceled)
        {
            _isSprinting = false;
        }
    }

    //Change Player Animation State
    private void ChangeAnimationState(string newState)
    {
        if (newState == _currentState)
        {
            return;
        }
        _playerAnimator.Play(newState);
        _currentState = newState;
    }

    //Check for specific animation
    private bool isAnimationPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(_groundPosition.position, _groundRadius);
    }
}
