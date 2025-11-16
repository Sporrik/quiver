using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerInputRelay : MonoBehaviour, IPlayerInputSource
{
    public Vector2 Move => IsBlocked ? Vector2.zero : _move;
    public bool SprintHeld => !IsBlocked && _sprintHeld;

    private bool _jumpEdge;
    private bool _interactEdge;
    private bool _takedownEdge;

    private Vector2 _move;
    private bool _sprintHeld;

    private readonly HashSet<object> _blockTokens = new();
    public bool IsBlocked => _blockTokens.Count > 0;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        _jumpEdge = false;
        _interactEdge = false;
        _takedownEdge = false;
    }

    public void BeginBlock(object token)
    {
        if (token == null) return;
        bool wasBlocked = IsBlocked;
        _blockTokens.Add(token);
        if (!wasBlocked && IsBlocked) OnBecameBlocked();
    }

    public void EndBlock(object token)
    {
        if (token == null) return;
        bool wasBlocked = IsBlocked;
        _blockTokens.Remove(token);
    }

    private void OnBecameBlocked()
    {
        _move = Vector2.zero;
        _sprintHeld = false;
    }

    // ---- Send Messages ----
    public void OnMove(InputValue v)        { _move = v.Get<Vector2>(); }
    public void OnSprint(InputValue v)      { _sprintHeld = v.Get<float>() > 0.5f; }
    public void OnJump(InputValue v)        { if (v.Get<float>() > 0.5f) _jumpEdge = true; }
    public void OnInteract(InputValue v)    { if (v.Get<float>() > 0.5f) _interactEdge = true; }
    public void OnTakedown(InputValue v)    { if (v.Get<float>() > 0.5f) _takedownEdge = true; }

    // ---- Unity Events ----
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _move = ctx.ReadValue<Vector2>();
        else if (ctx.canceled) _move = Vector2.zero;
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _sprintHeld = ctx.ReadValue<float>() > 0.5f;
        else if (ctx.canceled) _sprintHeld = false;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _jumpEdge = true;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _interactEdge = true;
    }

    public void OnTakedown(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _takedownEdge = true;
    }

    // ---- Interface ----
    public bool JumpStartedThisFrame() => _jumpEdge;
    public bool InteractStartedThisFrame() => _interactEdge;
    public bool TakedownStartedThisFrame() => _takedownEdge;
}