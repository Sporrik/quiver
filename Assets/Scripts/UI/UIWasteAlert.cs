using UnityEngine;
using UI;

public class UIWasteAlert : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MinigameManager _manager;

    [Header("Arrow Objects (children of bars)")]
    [SerializeField] private GameObject poopArrow;
    [SerializeField] private GameObject peeArrow;

    [Header("UI Message")]
    [SerializeField] private GameObject alertMessage;

    [Header("Display Settings")]
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private UIScriptableObject _uiData;

    private CanvasGroup _messageGroup;
    private CanvasGroup _activeArrowGroup;

    private bool _alertShown = false;  
    private bool _isFading = false;

    private float _fadeStartTime;
    private GameObject _activeArrow;

    private void Awake()
    {
        _uiData = UIMeterDataProvider.Shared;

        if (_uiData == null)
        {
            Debug.LogError("UIWasteAlert: No UIData found. Make sure UIMeterDataProvider exists in the scene.");
            enabled = false;
            return;
        }

        if (_manager == null)
            _manager = Object.FindFirstObjectByType<MinigameManager>();

        _messageGroup = alertMessage.GetComponent<CanvasGroup>();
        if (_messageGroup == null)
            _messageGroup = alertMessage.AddComponent<CanvasGroup>();

        alertMessage.SetActive(false);
        poopArrow.SetActive(false);
        peeArrow.SetActive(false);
    }

    private void OnEnable()
    {
        _uiData.PoopChanged += OnValueChanged;
        _uiData.PeeChanged += OnValueChanged;
    }

    private void OnDisable()
    {
        _uiData.PoopChanged -= OnValueChanged;
        _uiData.PeeChanged -= OnValueChanged;
    }

    private void Update()
    {
        if (!_isFading) return;

        float t = (Time.time - _fadeStartTime) / fadeDuration;

        _messageGroup.alpha = Mathf.Lerp(1f, 0f, t);
        if (_activeArrowGroup != null)
            _activeArrowGroup.alpha = Mathf.Lerp(1f, 0f, t);

        if (t >= 1f)
            FinishFadeOut();
    }

    private void OnValueChanged(float value)
    {
        // If ANY alert has ever shown, stop permanently
        if (_alertShown) return;

        // If minigame running: do not show alerts
        if (_manager != null && _manager.MinigameIsRunning()) return;

        bool poopFull = _uiData.GetPoop() >= 100f;
        bool peeFull = _uiData.GetPee() >= 100f;

        if (poopFull)
        {
            ShowAlert(poopArrow);
            _alertShown = true; 
        }
        else if (peeFull)
        {
            ShowAlert(peeArrow);
            _alertShown = true;
        }
    }

    private void ShowAlert(GameObject arrow)
    {
        if (_isFading) return;

        _activeArrow = arrow;
        _activeArrow.SetActive(true);

        _activeArrowGroup = _activeArrow.GetComponent<CanvasGroup>();
        if (_activeArrowGroup == null)
            _activeArrowGroup = _activeArrow.AddComponent<CanvasGroup>();

        _activeArrowGroup.alpha = 1f;

        alertMessage.SetActive(true);
        _messageGroup.alpha = 1f;

        // Trigger fade automatically
        Invoke(nameof(StartFadeOut), visibleDuration);
    }

    private void StartFadeOut()
    {
        _isFading = true;
        _fadeStartTime = Time.time;
    }

    private void FinishFadeOut()
    {
        _isFading = false;

        alertMessage.SetActive(false);

        if (_activeArrow != null)
            _activeArrow.SetActive(false);

        _activeArrow = null;
        _activeArrowGroup = null;
    }
}