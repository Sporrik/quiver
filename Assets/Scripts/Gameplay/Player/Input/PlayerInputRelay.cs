using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerInputRelay : MonoBehaviour, IPlayerInputSource
{
    private Vector2 _move;
    private bool _sprintHeld;

    // one-frame edges
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
}