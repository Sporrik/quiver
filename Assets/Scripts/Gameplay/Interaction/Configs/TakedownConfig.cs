using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Takedown", fileName = "Takedown_Default")]
public class TakedownConfig : ScriptableObject
{
    [Header("Rules")]
    [SerializeField, Min(0f)] private float _range = 1.5f;
    [SerializeField, Range(0f, 180f)] private float _backAngle = 60f;
    [SerializeField, Min(0f)] private float _cooldownSeconds = 0f;
    [SerializeField] private LayerMask _losMask;

    public float Range => _range;
    public float BackAngle => _backAngle;
    public float CooldownSeconds => _cooldownSeconds;
    public LayerMask LoSMask => _losMask;
}