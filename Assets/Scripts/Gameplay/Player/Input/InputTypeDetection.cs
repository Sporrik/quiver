using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputTypeDetection : MonoBehaviour
{
    public bool UsingController = false;
    public string ControllerType = "Unknown";
    private PlayerInput _playerInput;
    private string _previousScheme = "Irrelevant";
    public bool IsTrackingInput = false;

    private void Update()
    {
        if (IsTrackingInput)
        {
            string currentScheme = _playerInput.currentControlScheme;

            if (currentScheme == "Keyboard&Mouse" && currentScheme != _previousScheme)
            {
                UsingController = false;
                _previousScheme = currentScheme;
            }
            else if (currentScheme == "Gamepad" && currentScheme != _previousScheme)
            {
                UsingController = true;
                DetectControllerType();
                _previousScheme = currentScheme;

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
                ControllerType = "PlayStationController";
            }
            else
            {
                ControllerType = "Unknown Gamepad";
            }

            Debug.Log($"Detected Controller: {ControllerType}");
        }
        else
        {
            ControllerType = "No Gamepad Connected";
        }
    }
}
