using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;

public class TakedownTriggerMessage : MonoBehaviour
{
    [Header("UI Messages (each must have CanvasGroup)")]
    [SerializeField] private GameObject keyboardMouseMessage;
    [SerializeField] private GameObject xboxMessage;
    [SerializeField] private GameObject playStationMessage;

    [Header("Timings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private GameObject _activeMessage;
    private CanvasGroup _canvasGroup;

    private bool _hasTriggered;
    private bool _isAnimating;

    private float _animStartTime;

    private enum AnimState { None, FadeIn, VisibleWait, FadeOut }
    private AnimState _state = AnimState.None;

    private PlayerInput _playerInput;

    private void Awake()
    {
        DisableAllMessages();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        _playerInput = other.GetComponent<PlayerInput>();
        if (_playerInput == null) return;

        _hasTriggered = true;

        SelectCorrectMessage();
        StartFadeIn();
    }

    private void Update()
    {
        if (!_isAnimating || _canvasGroup == null) return;

        float t = Time.time - _animStartTime;

        switch (_state)
        {
            case AnimState.FadeIn:
                _canvasGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
                if (_canvasGroup.alpha >= 1f)
                    StartVisibleWait();
                break;

            case AnimState.VisibleWait:
                if (t >= visibleDuration)
                    StartFadeOut();
                break;

            case AnimState.FadeOut:
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
                if (_canvasGroup.alpha <= 0f)
                    Finish();
                break;
        }
    }


    private void SelectCorrectMessage()
    {
        DisableAllMessages();

        string scheme = _playerInput.currentControlScheme;

        if (scheme.Contains("Gamepad"))
        {
            var gamepad = Gamepad.current;

            if (gamepad != null &&
                gamepad.description.manufacturer.ToLower().Contains("sony"))
            {
                _activeMessage = playStationMessage;
            }
            else
            {
                _activeMessage = xboxMessage;
            }
        }
        else
        {
            _activeMessage = keyboardMouseMessage;
        }

        _canvasGroup = _activeMessage.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = _activeMessage.AddComponent<CanvasGroup>();

        _activeMessage.SetActive(true);
        _canvasGroup.alpha = 0f;
    }

    private void DisableAllMessages()
    {
        keyboardMouseMessage.SetActive(false);
        xboxMessage.SetActive(false);
        playStationMessage.SetActive(false);
    }

    private void StartFadeIn()
    {
        _isAnimating = true;
        _animStartTime = Time.time;
        _state = AnimState.FadeIn;
    }

    private void StartVisibleWait()
    {
        _animStartTime = Time.time;
        _state = AnimState.VisibleWait;
    }

    private void StartFadeOut()
    {
        _animStartTime = Time.time;
        _state = AnimState.FadeOut;
    }

    private void Finish()
    {
        _isAnimating = false;
        _state = AnimState.None;
        _activeMessage.SetActive(false);
    }
}
