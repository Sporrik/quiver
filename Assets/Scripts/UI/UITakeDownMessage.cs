using UnityEngine;
using Gameplay.Interaction;

public sealed class StealthTakedownUI : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform player;                      // usually the player's transform
    [SerializeField] private Transform uiBillboard;                 // world-space UI above player's head
    [SerializeField] private float checkRadius = 2f;                // same as PlayerInteraction range
    [SerializeField] private LayerMask takedownMask;                // same mask used for interactions

    private readonly Collider[] _hits = new Collider[8];
    private Interactor _interactor;

    private void Awake()
    {
        if (player == null) player = transform; // fallback
        if (uiBillboard != null) uiBillboard.gameObject.SetActive(false);

        _interactor = new Interactor(player);
    }

    private void Update()
    {
        bool canTakedown = CheckForValidTakedownTarget();

        if (uiBillboard != null)
            uiBillboard.gameObject.SetActive(canTakedown);
    }

    /// <summary>
    /// Returns true if ANY guard in radius can be stealth-takedowned.
    /// </summary>
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
                // ITakedownTarget already checks:
                // - guard state
                // - distance
                // - angle from behind
                // - LoS
                // - cooldown
                // - whether guard is patrolling
                if (target.CanTakedown(_interactor))
                    return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player != null ? player.position : transform.position, checkRadius);
    }
#endif
}