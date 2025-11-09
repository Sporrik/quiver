using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Movement", fileName = "Movement_Default")]
public sealed class MovementConfig : ScriptableObject
{
    [Header("Speeds")]
    [SerializeField, Min(0f)] private float _baseSpeed = 5f;
    [SerializeField, Min(1f)] private float _sprintMultiplier = 1.5f;

    [Header("Rotation")]
    [SerializeField, Min(0f)] private float _rotationDegPerSec = 540f;

    [Header("Jump and Gravity")]
    [SerializeField, Min(0f)] private float _jumpPower = 5f;
    [SerializeField, Min(0f)] private float _gravityMultiplier = 2f;

    public float BaseSpeed => _baseSpeed;
    public float SprintMultiplier => _sprintMultiplier;
    public float RotationDegPerSec => _rotationDegPerSec;
    public float JumpPower => _jumpPower;
    public float GravityMultiplier => _gravityMultiplier;
}