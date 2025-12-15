using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlDetector : MonoBehaviour
{
    private bool _usingController = false;
    private string _controllerType = "Unknown";
    private PlayerInput _playerInput;
    private string _previousScheme = "Irrelevant";

    [SerializeField] private Image _moveImage;
    [SerializeField] private Sprite _keyboardMoveSprite;
    [SerializeField] private Sprite _controllerMoveSprite;
    [SerializeField] private Sprite _PSMoveSprite;

    [SerializeField] private Image _grabImage;
    [SerializeField] private Sprite _keyboardGrabSprite;
    [SerializeField] private Sprite _controllerGrabSprite;
    [SerializeField] private Sprite _PSGrabSprite;


    void Update()
    {
        if (_playerInput == null)
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (_playerInput != null)
        {
            string currentScheme = _playerInput.currentControlScheme;

            if (currentScheme == "Keyboard&Mouse" && currentScheme != _previousScheme)
            {
                _usingController = false;
                _previousScheme = currentScheme;

                _moveImage.sprite = _keyboardMoveSprite;
                _grabImage.sprite = _keyboardGrabSprite;
            }
            else if (currentScheme == "Gamepad" && currentScheme != _previousScheme)
            {
                _usingController = true;
                DetectControllerType();
                _previousScheme = currentScheme;

                if (_controllerType == "PlayStationController")
                {
                    _moveImage.sprite = _PSMoveSprite;
                    _grabImage.sprite = _PSGrabSprite;
                }
                else
                {
                    _moveImage.sprite = _controllerMoveSprite;
                    _grabImage.sprite = _controllerGrabSprite;
                }
            }
            else
            {
                Debug.Log($"User is using an unknown control scheme: {currentScheme}");
            }
        }
    }

    private void DetectControllerType()
    {
        if (Gamepad.current != null)
        {
            string controllerName = Gamepad.current.displayName.ToLower();

            // Check for PlayStation controllers
            if (controllerName.Contains("playstation") ||
                controllerName.Contains("dualshock") ||
                controllerName.Contains("dualsense") ||
                controllerName.Contains("dual sense"))
            {
                _controllerType = "PlayStationController";
            }
            else
            {
                _controllerType = "Unknown Gamepad";
            }

            Debug.Log($"Detected Controller: {_controllerType}");
        }
        else
        {
            _controllerType = "No Gamepad Connected";
        }
    }
}
