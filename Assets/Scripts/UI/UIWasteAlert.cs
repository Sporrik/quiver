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

//using UnityEngine;
//using UI;   // needed for UIScriptableObject

//public class UIWasteAlert : MonoBehaviour
//{
//    [Header("UI Message To Show")]
//    [SerializeField] private GameObject _alertUI;

//    [Header("Optional: Minigame Manager Reference")]
//    [SerializeField] private MinigameManager _manager;

//    private UIScriptableObject _uiData;
//    private bool _isAlertShown = false;

//    [Header("UI Happiness Message")]
//    [SerializeField] private GameObject _happinessUI;
//    [SerializeField] private float _happinessMessageDuration = 5f;

//    private bool _happinessShown = false;
//    private float _happinessHideTime = 0f;

//    private void Awake()
//    {
//        // Fetch the shared UI data reference
//        _uiData = UIMeterDataProvider.Shared;

//        if (_uiData == null)
//        {
//            Debug.LogError("UIWasteAlert: No UIData found. Make sure UIMeterDataProvider exists in the scene.");
//            enabled = false;
//            return;
//        }

//        // Try auto-find manager if not assigned
//        if (_manager == null)
//        {
//            _manager = Object.FindFirstObjectByType<MinigameManager>();
//        }
//    }

//    private void OnEnable()
//    {
//        if (_uiData == null) return;

//        // Subscribe to changes
//        _uiData.PoopChanged += OnValueChanged;
//        _uiData.PeeChanged += OnValueChanged;

//        _uiData.HappinessChanged += OnHappinessChanged;
//    }

//    private void OnDisable()
//    {
//        if (_uiData == null) return;

//        // Unsubscribe
//        _uiData.PoopChanged -= OnValueChanged;
//        _uiData.PeeChanged -= OnValueChanged;

//        _uiData.HappinessChanged -= OnHappinessChanged;
//    }

//    private void Update()
//    {
//        // Hide UI on SPACE
//        if (_isAlertShown && Input.GetKeyDown(KeyCode.Space))
//        {
//            HideAlert();
//        }

//        // If minigame starts while alert is visible, hide it
//        if (_isAlertShown && _manager != null && _manager.MinigameIsRunning())
//        {
//            HideAlert();
//        }

//        // Auto-hide the happiness alert after duration
//        if (_happinessShown && Time.time >= _happinessHideTime)
//        {
//            HideHappinessAlert();
//        }

//        // If minigame starts while happiness alert is visible, hide it
//        if (_happinessShown && _manager != null && _manager.MinigameIsRunning())
//        {
//            HideHappinessAlert();
//        }
//    }

//    private void OnValueChanged(float value)
//    {
//        // If minigame is running, do NOT show alert
//        if (_manager != null && _manager.MinigameIsRunning())
//            return;

//        // If either one is maxed out -> show alert
//        if (_uiData.GetPoop() >= 100f || _uiData.GetPee() >= 100f)
//        {
//            ShowAlert();
//        }
//    }

//    private void ShowAlert()
//    {
//        if (_alertUI == null || _isAlertShown) return;

//        // If minigame is running, do NOT show
//        if (_manager != null && _manager.MinigameIsRunning())
//            return;

//        _alertUI.SetActive(true);
//        _isAlertShown = true;
//    }

//    private void HideAlert()
//    {
//        if (_alertUI == null) return;

//        _alertUI.SetActive(false);
//        _isAlertShown = false;
//    }

//    private void OnHappinessChanged(float value)
//    {
//        // If minigame is running, do NOT show alert
//        if (_manager != null && _manager.MinigameIsRunning())
//            return;

//        if (value >= 100f)
//        {
//            ShowHappinessAlert();
//        }
//    }

//    private void ShowHappinessAlert()
//    {
//        if (_happinessUI == null) return;

//        // Do NOT show multiple times
//        if (_happinessShown) return;

//        _happinessUI.SetActive(true);
//        _happinessShown = true;

//        // Store the future time at which it should hide
//        _happinessHideTime = Time.time + _happinessMessageDuration;
//    }

//    private void HideHappinessAlert()
//    {
//        if (_happinessUI == null) return;

//        _happinessUI.SetActive(false);
//        _happinessShown = false;
//    }
//}
