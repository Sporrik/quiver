using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController _characterController;
    [SerializeField] private float _baseSpeed;
    private float _currentSpeed;

    [SerializeField] private float _maxStamina, _staminaRegenRate;
    private float _stamina, _staminaRegen;

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

    [SerializeField] private float _sprintSpeedMulti;
    private bool _isSprinting;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = gameObject.GetComponent<Animator>();
        _mainCamera = Camera.main;

        _currentSpeed = _baseSpeed;
        _stamina = _maxStamina;
    }

    private void Update()
    {
        ApplyRotation();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ApplyMovement();
        UpdateStamina();
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

    private void UpdateStamina()
    {
        if (_isSprinting)
        {
            _stamina--;
        }
        else
        {
            _staminaRegen += Time.deltaTime;
            if (_staminaRegen >= _staminaRegenRate)
            {
                _staminaRegen = 0;
                if (_stamina < _maxStamina)
                {
                    _stamina++;
                    //_staminaBar.UpdateStaminaBar(_stamina, _maxStamina);
                }
            }
        }  
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

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed && CanSprint)
        {
            StartSprint();
        }
        
        if (context.canceled || !CanSprint)
        {
            EndSprint();
        }
    }

    public bool IsSprinting() => _isSprinting;

    private bool CanSprint => _stamina > 0;

    private void StartSprint()
    {
        _isSprinting = true;
        _currentSpeed = _baseSpeed * _sprintSpeedMulti;
    }

    private void EndSprint()
    {
        _isSprinting = false;
        _currentSpeed = _baseSpeed;
    }

    //Change Player Animation State
    private void ChangeAnimationState(string newState)
    {
        if (newState == _currentState) return;

        _playerAnimator.Play(newState);
        _currentState = newState;
    }

    //Check for specific animation
    private bool isAnimationPlaying(Animator animator, string stateName)
        => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
}