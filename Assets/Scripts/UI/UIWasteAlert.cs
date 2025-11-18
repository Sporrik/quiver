using UnityEngine;
using UI;   // needed for UIScriptableObject

public class UIWasteAlert : MonoBehaviour
{
    [Header("UI Message To Show")]
    [SerializeField] private GameObject _alertUI;

    [Header("Optional: Minigame Manager Reference")]
    [SerializeField] private MinigameManager _manager;

    private UIScriptableObject _uiData;
    private bool _isAlertShown = false;

    [Header("UI Happiness Message")]
    [SerializeField] private GameObject _happinessUI;
    [SerializeField] private float _happinessMessageDuration = 5f;

    private bool _happinessShown = false;
    private float _happinessHideTime = 0f;

    private void Awake()
    {
        // Fetch the shared UI data reference
        _uiData = UIMeterDataProvider.Shared;

        if (_uiData == null)
        {
            Debug.LogError("UIWasteAlert: No UIData found. Make sure UIMeterDataProvider exists in the scene.");
            enabled = false;
            return;
        }

        // Try auto-find manager if not assigned
        if (_manager == null)
        {
            _manager = Object.FindFirstObjectByType<MinigameManager>();
        }
    }

    private void OnEnable()
    {
        if (_uiData == null) return;

        // Subscribe to changes
        _uiData.PoopChanged += OnValueChanged;
        _uiData.PeeChanged += OnValueChanged;

        _uiData.HappinessChanged += OnHappinessChanged;
    }

    private void OnDisable()
    {
        if (_uiData == null) return;

        // Unsubscribe
        _uiData.PoopChanged -= OnValueChanged;
        _uiData.PeeChanged -= OnValueChanged;

        _uiData.HappinessChanged -= OnHappinessChanged;
    }

    private void Update()
    {
        // Hide UI on SPACE
        if (_isAlertShown && Input.GetKeyDown(KeyCode.Space))
        {
            HideAlert();
        }

        // If minigame starts while alert is visible, hide it
        if (_isAlertShown && _manager != null && _manager.MinigameIsRunning())
        {
            HideAlert();
        }

        // Auto-hide the happiness alert after duration
        if (_happinessShown && Time.time >= _happinessHideTime)
        {
            HideHappinessAlert();
        }

        // If minigame starts while happiness alert is visible, hide it
        if (_happinessShown && _manager != null && _manager.MinigameIsRunning())
        {
            HideHappinessAlert();
        }
    }

    private void OnValueChanged(float value)
    {
        // If minigame is running, do NOT show alert
        if (_manager != null && _manager.MinigameIsRunning())
            return;

        // If either one is maxed out -> show alert
        if (_uiData.GetPoop() >= 100f || _uiData.GetPee() >= 100f)
        {
            ShowAlert();
        }
    }

    private void ShowAlert()
    {
        if (_alertUI == null || _isAlertShown) return;

        // If minigame is running, do NOT show
        if (_manager != null && _manager.MinigameIsRunning())
            return;

        _alertUI.SetActive(true);
        _isAlertShown = true;
    }

    private void HideAlert()
    {
        if (_alertUI == null) return;

        _alertUI.SetActive(false);
        _isAlertShown = false;
    }

    private void OnHappinessChanged(float value)
    {
        // If minigame is running, do NOT show alert
        if (_manager != null && _manager.MinigameIsRunning())
            return;

        if (value >= 100f)
        {
            ShowHappinessAlert();
        }
    }

    private void ShowHappinessAlert()
    {
        if (_happinessUI == null) return;

        // Do NOT show multiple times
        if (_happinessShown) return;

        _happinessUI.SetActive(true);
        _happinessShown = true;

        // Store the future time at which it should hide
        _happinessHideTime = Time.time + _happinessMessageDuration;
    }

    private void HideHappinessAlert()
    {
        if (_happinessUI == null) return;

        _happinessUI.SetActive(false);
        _happinessShown = false;
    }
}
