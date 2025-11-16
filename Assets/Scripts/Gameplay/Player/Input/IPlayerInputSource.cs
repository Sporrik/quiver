using UnityEngine;

public interface IPlayerInputSource
{
    Vector2 Move { get; }
    bool SprintHeld { get; }

    bool JumpStartedThisFrame();
    bool InteractStartedThisFrame();
    bool TakedownStartedThisFrame();
}