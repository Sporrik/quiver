using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Stamina", fileName = "Stamina_Default")]
public sealed class StaminaConfig : ScriptableObject
{
    [Header("Caps & Rates")]
    [SerializeField, Min(0f)] private float _max = 100f;
    [SerializeField, Min(0f)] private float _drainPerSec = 10f;
    [SerializeField, Min(0f)] private float _regenPerSec = 10f;

    [Header("Rules")]
    [SerializeField, Min(0f)] private float _sprintThreshold = 10f;
    [SerializeField, Min(0f)] private float _regenDelay = 1f;

    public float Max => _max;
    public float DrainPerSec => _drainPerSec;
    public float RegenPerSec => _regenPerSec;
    public float SprintThreshold => _sprintThreshold;
    public float RegenDelay => _regenDelay;
}