using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController _characterController;
    [SerializeField] private float _baseSpeed;
    private float _currentSpeed;

    [SerializeField] private TextMeshProUGUI _staminaText;
    [SerializeField] private float _stamina = 100;
    [SerializeField] private float _staminaIncreaseSpeed;
    [SerializeField] private float _stamineDecreaseSpeed;
    [SerializeField] private float TimeToRegainStamina = 3;
    private float StaminTimer = 0;
    private bool _isSprinting = false;

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

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.enabled = true;
        _mainCamera = Camera.main;

        _currentSpeed = _baseSpeed;
        _playerAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        ApplyRotation();
        _staminaText.text = $"Stamina: {Mathf.RoundToInt(_stamina)}";
        StaminTimer += Time.deltaTime;
        if (StaminTimer > TimeToRegainStamina && !_isSprinting)  // increase stamina if u wait a little time
        {
            _stamina += _staminaIncreaseSpeed;
            _stamina = Mathf.Min(100, _stamina); // 100 = max stamina
        }
        if (_isSprinting) // descrease stamina if sprinting
        {
            _stamina -= _staminaIncreaseSpeed;
            if(_stamina <= 0)
            {
                _stamina = 0;
                ResetSpeed();
            }  
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ApplyMovement();
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

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed && _stamina > 0)
        {
            _stamina -= _stamineDecreaseSpeed;
            _currentSpeed = _baseSpeed * _sprintSpeedMulti;
            _isSprinting = true;
        }
        else if (context.canceled || _stamina <= 0)
        {
           ResetSpeed();
        }
        
    }

    public bool IsSprinting()
    {
        return _isSprinting;
    }

private void ResetSpeed()
    {
        _isSprinting = false;
        StaminTimer = 0;
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