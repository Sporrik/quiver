using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlDetector : MonoBehaviour
{
    private bool _usingController = false;
    private string _controllerType = "Unknown";
    [SerializeField] private PlayerInput _playerInput;
    private string _previousScheme = "Irrelevant";

    [SerializeField] private Image _moveImage;
    [SerializeField] private Sprite _keyboardMoveSprite;
    [SerializeField] private Sprite _controllerMoveSprite;
    [SerializeField] private Sprite _PSMoveSprite;

    [SerializeField] private Image _grabImage;
    [SerializeField] private Sprite _keyboardGrabSprite;
    [SerializeField] private Sprite _controllerGrabSprite;
    [SerializeField] private Sprite _PSGrabSprite;

    private void Start()
    {
        MinigameScreen minigameScreen = FindFirstObjectByType<MinigameScreen>();
        if (minigameScreen != null)
        {
            _usingController = minigameScreen.UsingController;
            _controllerType = minigameScreen.ControllerType;
        }
    }

    void Update()
    {

        if (_playerInput != null)
        {
            if (_usingController == false)
            {
                _moveImage.sprite = _keyboardMoveSprite;
                _grabImage.sprite = _keyboardGrabSprite;
            }
            else if (_usingController == true)
            {
                _usingController = true;

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
                //Debug.Log($"User is using an unknown control scheme: {currentScheme}");
            }
        }
    }
}
