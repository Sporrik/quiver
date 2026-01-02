using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleInput : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private MinigameManager _minigameManager;
    private bool _isEnabled = false;

    void Start()
    {
        //_minigameManager = FindFirstObjectByType<MinigameManager>();

        //if (_minigameManager == null)
        //{
        //    Debug.LogError("MinigameManager not found in the scene!");
        //}
        //_playerInput.enabled = false; // Start with PlayerInput disabled
    }

    void Update()
    {
        //if (_minigameManager == null) return;

        //// Use IsMiniGameInputEnabled to determine whether to enable or disable PlayerInput
        //if (_minigameManager.IsMiniGameInputEnabled && !_isEnabled)
        //{
        //    _playerInput.enabled = true;
        //    _isEnabled = true;
        //}
        //else if (!_minigameManager.IsMiniGameInputEnabled && _isEnabled)
        //{
        //    _playerInput.enabled = false;
        //    _isEnabled = false;
        //}
    }
}
