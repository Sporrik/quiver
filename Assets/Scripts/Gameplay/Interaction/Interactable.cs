using UnityEngine;
using UnityEngine.Events;
using Gameplay.Interaction;

[DisallowMultipleComponent]
public sealed class Interactable : MonoBehaviour, IInteractable
{
    #region Inspector
    [Header("Config & Targeting")]
    [SerializeField] private InteractableConfig _config;

    [Tooltip("Optional custom point used for distance/LOS checks. Defaults to this transform.")]
    [SerializeField] private Transform _usePoint;

    [Header("Events")]
    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] private UnityEvent _onInteractFailed;   // e.g., blocked LOS, wrong angle, cooldown
    #endregion

    #region State
    private float _cooldownTimer;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (_usePoint == null) _usePoint = transform;
        if (_config == null)
            Debug.LogError($"{name}: InteractableConfig is missing.");
    }

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }
    #endregion

    #region IInteractable
    public bool CanInteract(Interactor interactor)
    {
        if (_config == null) return false;
        if (_cooldownTimer > 0f) return false;

        // Distance
        float dist = Vector3.Distance(interactor.Transform.position, _usePoint.position);
        if (dist > _config.UseRange) return false;

        // Facing check
        if (_config.RequiredFacingAngle < 180f)
        {
            Vector3 toMe = (_usePoint.position - interactor.Transform.position);
            toMe.y = 0f;
            Vector3 fwd = interactor.Transform.forward; fwd.y = 0f;

            if (toMe.sqrMagnitude > 0.0001f)
            {
                float angle = Vector3.Angle(fwd, toMe);
                if (angle > _config.RequiredFacingAngle) return false;
            }
        }

        // Line of sight
        if (_config.RequireLineOfSight)
        {
            Vector3 from = interactor.Transform.position + Vector3.up * 0.1f;
            Vector3 to = _usePoint.position;
            if (Physics.Linecast(from, to, out var hit, _config.LoSMask, QueryTriggerInteraction.Ignore))
            {
                // If we hit something that is not this interactable, fail
                if (!hit.transform.IsChildOf(transform))
                    return false;
            }
        }

        return true;
    }

    public void Interact(Interactor interactor)
    {
        if (!CanInteract(interactor))
        {
            _onInteractFailed?.Invoke();
            return;
        }

        _onInteract?.Invoke();

        // Start cooldown (if any)
        if (_config.CooldownSeconds > 0f)
            _cooldownTimer = _config.CooldownSeconds;
    }

    public string GetPrompt(Interactor interactor)
    {
        return _config != null ? _config.Prompt : null;
    }
    #endregion

#if UNITY_EDITOR
    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (_usePoint == null) _usePoint = transform;
        if (_config == null) return;

        // Range
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawWireSphere(_usePoint.position, _config.UseRange);

        // Facing cone (approximate)
        if (_config.RequiredFacingAngle < 180f)
        {
            Vector3 pos = _usePoint.position;
            float r = Mathf.Min(_config.UseRange, 1.5f);
            float a = _config.RequiredFacingAngle;

            // Draw two rays indicating the allowed cone edges (from target back toward interactor)
            Vector3 forward = (_usePoint.forward.sqrMagnitude < 0.01f) ? Vector3.forward : _usePoint.forward;
            Vector3 leftDir = Quaternion.Euler(0f, 180f - a, 0f) * forward;
            Vector3 rightDir = Quaternion.Euler(0f, 180f + a, 0f) * forward;

            Gizmos.color = new Color(0f, 0.7f, 1f, 0.8f);
            Gizmos.DrawRay(pos, leftDir.normalized * r);
            Gizmos.DrawRay(pos, rightDir.normalized * r);
        }
    }
    #endregion
#endif
}
