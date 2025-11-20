using UnityEngine;
using UI;

public class UIWasteAlert : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MinigameManager _manager;
    [SerializeField] private RectTransform poopBar;
    [SerializeField] private RectTransform peeBar;

    [Header("UI Components")]
    [SerializeField] private GameObject alertMessage;   // your original UI message
    [SerializeField] private GameObject arrowUI;        // one arrow reused for both bars

    [Header("Arrow Offset From Bar Edge")]
    [SerializeField] private float arrowOffsetX = 20f;
    [SerializeField] private float arrowOffsetY = 0f;

    [Header("Display Settings")]
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float arrowBopAmount = 10f;
    [SerializeField] private float arrowBopSpeed = 3f;

    private UIScriptableObject _uiData;

    private CanvasGroup _messageGroup;
    private CanvasGroup _arrowGroup;

    private bool _hasShown = false;     // prevents future displays
    private bool _isFading = false;
    private float _hideTime;

    private RectTransform _arrowRect;
    private bool _arrowActive = false;
    private bool _alertTriggered = false;

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

        // Prepare canvas groups
        _messageGroup = alertMessage.GetComponent<CanvasGroup>();
        if (_messageGroup == null) _messageGroup = alertMessage.AddComponent<CanvasGroup>();

        _arrowGroup = arrowUI.GetComponent<CanvasGroup>();
        if (_arrowGroup == null) _arrowGroup = arrowUI.AddComponent<CanvasGroup>();

        _arrowRect = arrowUI.GetComponent<RectTransform>();

        alertMessage.SetActive(false);
        arrowUI.SetActive(false);
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
        if (!_arrowActive) return;

        // If minigame starts, hide immediately
        if (_manager != null && _manager.MinigameIsRunning())
        {
            StartFadeOut();
            return;
        }

        // Bop arrow left-right
        float offset = Mathf.Sin(Time.time * arrowBopSpeed) * arrowBopAmount;
        _arrowRect.anchoredPosition += new Vector2(offset * Time.deltaTime, 0);

        // Check if time to fade out
        if (!_isFading && Time.time >= _hideTime)
            StartFadeOut();

        // Perform fade out
        if (_isFading)
        {
            float t = (Time.time - _hideStartTime) / fadeDuration;

            _messageGroup.alpha = Mathf.Lerp(1f, 0f, t);
            _arrowGroup.alpha = Mathf.Lerp(1f, 0f, t);

            if (t >= 1f)
                FinishFadeOut();
        }
    }

    private float _hideStartTime;

    private void StartFadeOut()
    {
        _isFading = true;
        _hideStartTime = Time.time;
    }

    private void FinishFadeOut()
    {
        alertMessage.SetActive(false);
        arrowUI.SetActive(false);

        _arrowActive = false;
        _isFading = false;

        // Mark as permanently done
        _hasShown = true;
    }

    private void OnValueChanged(float value)
    {
        if (_hasShown) return;
        if (_manager != null && _manager.MinigameIsRunning()) return;

        bool poopFull = _uiData.GetPoop() >= 100f;
        bool peeFull = _uiData.GetPee() >= 100f;

        if (poopFull || peeFull)
            ShowAlert(poopFull ? poopBar : peeBar);
    }

    private void ShowAlert(RectTransform targetBar)
    {
        if (_hasShown) return;

        // If alert already triggered once (arrow & message shown), do NOT reposition the arrow
        if (_alertTriggered)
            return;

        _alertTriggered = true;

        // Position arrow next to bar
        arrowUI.SetActive(true);
        alertMessage.SetActive(true);

        float barHalfWidth = targetBar.rect.width * 0.5f;

        _arrowRect.anchoredPosition =
            targetBar.anchoredPosition +
            new Vector2(barHalfWidth + arrowOffsetX, arrowOffsetY);

        // Reset alpha
        _messageGroup.alpha = 1f;
        _arrowGroup.alpha = 1f;

        _arrowActive = true;

        _hideTime = Time.time + visibleDuration;
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
