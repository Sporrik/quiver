using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Player", fileName = "Player_Default")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private MovementConfig _movement;
    [SerializeField] private StaminaConfig _stamina;

    [SerializeField] private float _groundRadius = 0.25f;
    private LayerMask _groundMask;

    public MovementConfig Movement => _movement;
    public StaminaConfig Stamina => _stamina;
    public float GroundRadius => _groundRadius;
    public LayerMask GroundMask => _groundMask;
}