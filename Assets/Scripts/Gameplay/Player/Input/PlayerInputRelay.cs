using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerInputRelay : MonoBehaviour, IPlayerInputSource
{
    private Vector2 _move;
    private bool _sprintHeld;

    private bool _jumpEdge;
    private bool _interactEdge;
    private bool _takedownEdge;

    public Vector2 Move => _move;
    public bool SprintHeld => _sprintHeld;

    private void LateUpdate()
    {
        _jumpEdge = false;
        _interactEdge = false;
        _takedownEdge = false;
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