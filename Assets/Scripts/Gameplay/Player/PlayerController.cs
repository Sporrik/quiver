using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public sealed class PlayerController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private PlayerConfig _playerCfg;
    [SerializeField] private Transform _groundCheck;

    #endregion

    #region Components

    private CharacterController _characterController;
    private Animator _animator;
    private Camera _camera;

    #endregion

    #region Runtime State

    private Vector2 _moveInput;     // raw axes
    private Vector3 _moveDirWorld;  // camera-relative XZ
    private float _verticalVelocity;
    private float _currentSpeed;

    #endregion

    #region Stamina State

    private float _stamina;
    private float _regenDelayTimer;
    private bool _sprintHeld;
    private bool _isSprinting;

    #endregion

    #region Animator Parameters

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int SprintParam = Animator.StringToHash("IsSprinting");

    #endregion

    #region Constants / Buffers

    private const float BASE_GRAVITY = -9.81f;
    private const float GROUND_STICK = -2f;

    private static readonly Collider[] _groundHits = new Collider[4];

    #endregion

    #region Events / Read-Only

    public event System.Action<float, float> OnStaminaChanged;
    public bool IsSprinting => _isSprinting;
    public float StaminaNorm => _playerCfg.Stamina ? _stamina / _playerCfg.Stamina.Max : 0f;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;

        if (_playerCfg.Movement == null)
            Debug.LogError("PlayerController: MovementConfig missing.");

        if (_playerCfg.Stamina == null)
            Debug.LogError("PlayerController: StaminaConfig missing.");

        _currentSpeed = _playerCfg.Movement ? _playerCfg.Movement.BaseSpeed : 0f;
        _stamina = _playerCfg.Stamina ? _playerCfg.Stamina.Max : 0f;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        ReadWorldspaceMoveDirection();  // input -> world-space vector
        HandleSprintState();            // decide start/stop sprinting
        TickStamina(dt);                // drain / regen stamina
        TickGravity(dt);                // apply gravity to vertical velocity
        TickRotation(dt);               // rotate towards movement
        TickMovement(dt);               // move character controller
        TickAnimator();                 // sync animation parameters
    }

    #endregion

    #region Input Callbacks

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            _sprintHeld = true;

        if (ctx.canceled)
            _sprintHeld = false;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (!IsGrounded()) return;

        _verticalVelocity += _playerCfg.Movement.JumpPower;
    }

    #endregion

    #region Movement & Rotation

    private void ReadWorldspaceMoveDirection()
    {
        if (_camera == null)
        {
            _moveDirWorld = new Vector3(_moveInput.x, 0f, _moveInput.y);
        }
        else
        {
            Vector3 fwd = _camera.transform.forward; fwd.y = 0f; fwd.Normalize();
            Vector3 rgt = _camera.transform.right; rgt.y = 0f; rgt.Normalize();
            _moveDirWorld = rgt * _moveInput.x + fwd * _moveInput.y;
        }

        // Avoid faster diagonal movement
        if (_moveDirWorld.sqrMagnitude > 1f) _moveDirWorld.Normalize();
    }

    private void TickRotation(float dt)
    {
        if (_moveDirWorld.sqrMagnitude < 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(_moveDirWorld, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _playerCfg.Movement.RotationDegPerSec *  dt);
    }

    private void TickMovement(float dt)
    {
        Vector3 horizontal = _moveDirWorld * _currentSpeed;
        Vector3 vertical = Vector3.up * _verticalVelocity;

        _characterController.Move((horizontal + vertical) * dt);
    }

    #endregion

    #region Gravity & Grounding

    private void TickGravity(float dt)
    {
        if (IsGrounded())
        {
            if (_verticalVelocity < 0f)
                _verticalVelocity = GROUND_STICK; // keep grounded
        }
        else
        {
            _verticalVelocity += BASE_GRAVITY * _playerCfg.Movement.GravityMultiplier * dt;
        }
    }

    private bool IsGrounded()
    {
        if (_groundCheck == null)
            return _characterController.isGrounded;

        int count = Physics.OverlapSphereNonAlloc(_groundCheck.position, _playerCfg.GroundRadius, _groundHits, _playerCfg.GroundMask, QueryTriggerInteraction.Ignore);
        return count > 0;
    }

    #endregion

    #region Sprint & Stamina

    private bool CanSprint => !_isSprinting && _sprintHeld && _stamina >= _playerCfg.Stamina.SprintThreshold;

    private void HandleSprintState()
    {
        if (CanSprint)
            StartSprint();

        if (_isSprinting && (!_sprintHeld || _stamina <= 0f))
            EndSprint();
    }

    private void StartSprint()
    {
        _isSprinting = true;
        _currentSpeed = _playerCfg.Movement.BaseSpeed * _playerCfg.Movement.SprintMultiplier;
    }

    private void EndSprint()
    {
        _isSprinting = false;
        _currentSpeed = _playerCfg.Movement.BaseSpeed;
        _regenDelayTimer = _playerCfg.Stamina.RegenDelay;
    }

    private void TickStamina(float dt)
    {
        float prev = _stamina;

        if (_isSprinting)
        {
            _stamina -= _playerCfg.Stamina.DrainPerSec * dt;
            if (_stamina <= 0f)
            {
                _stamina = 0f;
                EndSprint();
            }
        }
        else
        {
            if (_regenDelayTimer > 0f)
                _regenDelayTimer -= dt;
            else
                _stamina += _playerCfg.Stamina.RegenPerSec * dt;
        }

        _stamina = Mathf.Clamp(_stamina, 0f, _playerCfg.Stamina.Max);
        if (!Mathf.Approximately(prev, _stamina))
            OnStaminaChanged?.Invoke(_stamina, _playerCfg.Stamina.Max);
    }

    #endregion

    # region Animation

    private void TickAnimator()
    {
        if (_animator == null) return;

        float moveMagnitude = _moveDirWorld.magnitude;
        if (_isSprinting)
            moveMagnitude *= _playerCfg.Movement.SprintMultiplier;

        _animator.SetFloat(SpeedParam, moveMagnitude);
        _animator.SetBool(SprintParam, _isSprinting);
    }

    #endregion
}