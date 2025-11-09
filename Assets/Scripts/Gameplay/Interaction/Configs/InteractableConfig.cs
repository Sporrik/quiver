using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Interactable", fileName = "Interactable_Default")]
public class InteractableConfig : ScriptableObject
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Interact";
    [SerializeField] private KeyCode _suggestedKey = KeyCode.E;

    [Header("Rules")]
    [Tooltip("Maximum usable distance from interactor origin to target center/point")]
    [SerializeField, Min(0f)] private float _useRange = 2f;

    [Tooltip("Require interactor to face target within this angle")]
    [SerializeField, Range(0f, 180f)] private float _requiredFacingAngle = 60f;

    [Tooltip("If true, a raycast must be clear between interactor origin and this object")]
    [SerializeField] private bool _requireLineOfSight = true;

    [Tooltip("Layers considered as obstacles for line of sight.")]
    [SerializeField] private LayerMask _losMask;

    [Header("Cooldown")]
    [Tooltip("Minimum time between uses")]
    [SerializeField, Min(0f)] private float _cooldownSeconds;

    //Read-only
    public string Prompt => _prompt;
    public KeyCode SuggestedKey => _suggestedKey;
    public float UseRange => _useRange;
    public float RequiredFacingAngle => _requiredFacingAngle;
    public bool RequireLineOfSight => _requireLineOfSight;
    public LayerMask LoSMask => _losMask;
    public float CooldownSeconds => _cooldownSeconds;
}
