using UnityEngine;
using Gameplay.Interaction;

public sealed class StealthTakedownUI : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform uiBillboard;

    [Header("Detection")]
    [SerializeField] private float checkRadius = 2f;
    [SerializeField] private LayerMask takedownMask;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.15f;

    private readonly Collider[] _hits = new Collider[8];
    private Interactor _interactor;

    private CanvasGroup _canvasGroup;
    private float _targetAlpha;
    private float _fadeVelocity;
    private bool _isVisible;

    private void Awake()
    {
        if (player == null) player = transform;

        if (uiBillboard != null)
        {
            _canvasGroup = uiBillboard.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = uiBillboard.gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            uiBillboard.gameObject.SetActive(false);
        }

        _interactor = new Interactor(player);
    }

    private void Update()
    {
        bool canTakedown = CheckForValidTakedownTarget();

        if (canTakedown)
            Show();
        else
            Hide();

        UpdateFade();
    }

    /// Returns true if ANY guard in radius can be stealth-takedowned.
    private bool CheckForValidTakedownTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(
            player.position,
            checkRadius,
            _hits,
            takedownMask,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<ITakedownTarget>(out var target))
            {
                if (target.CanTakedown(_interactor))
                    return true;
            }
        }

        return false;
    }

    private void Show()
    {
        if (_isVisible) return;

        _isVisible = true;
        _targetAlpha = 1f;

        if (!uiBillboard.gameObject.activeSelf)
            uiBillboard.gameObject.SetActive(true);
    }

    private void Hide()
    {
        if (!_isVisible) return;

        _isVisible = false;
        _targetAlpha = 0f;
    }

    private void UpdateFade()
    {
        if (_canvasGroup == null) return;

        float duration = _targetAlpha > _canvasGroup.alpha
            ? fadeInDuration
            : fadeOutDuration;

        _canvasGroup.alpha = Mathf.SmoothDamp(
            _canvasGroup.alpha,
            _targetAlpha,
            ref _fadeVelocity,
            duration);

        if (_canvasGroup.alpha <= 0.01f && !_isVisible)
            uiBillboard.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(
            player != null ? player.position : transform.position,
            checkRadius);
    }
#endif
}