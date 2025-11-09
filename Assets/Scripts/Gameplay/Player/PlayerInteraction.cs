using UnityEngine;
using UnityEngine.InputSystem;
using Gameplay.Interaction;

[DisallowMultipleComponent]
public sealed class PlayerInteraction : MonoBehaviour
{
    #region Inspector

    [Header("Interaction Settings")]
    [SerializeField] private Transform _origin;
    [SerializeField, Min(0f)] private float _range = 2f;
    [SerializeField] private LayerMask _interactableMask;

    #endregion

    #region Components / State

    private readonly Collider[] _hits = new Collider[8];
    private Interactor _interactor;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        _interactor = new Interactor(transform);
        if (_origin == null)
            _origin = transform;    //fallback
    }

    #endregion

    #region Input

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        TryInteract();
    }

    public void OnTakedown(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        TryTakedown();
    }

    #endregion

    #region Query + Execute

    private void TryInteract()
    {
        int count = OverlapAtOrigin();
        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract(_interactor))
            {
                interactable.Interact(_interactor);
                break;  // interact with first valid target only
            }
        }
    }

    private void TryTakedown()
    {
        int count = OverlapAtOrigin();
        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<ITakedownTarget>(out var target) && target.CanTakedown(_interactor))
            {
                target.Takedown(_interactor);
                break;  // interact with first valid target only
            }
        }
    }

    #endregion

    #region Helpers

    private int OverlapAtOrigin()
    {
        return Physics.OverlapSphereNonAlloc(_origin.position, _range, _hits, _interactableMask, QueryTriggerInteraction.Ignore);
    }

    #endregion

#if UNITY_EDITOR
    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (_origin == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_origin.position, _range);
    }

    #endregion
#endif
}