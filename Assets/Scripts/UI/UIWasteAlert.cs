using UnityEngine;
using UI;
using UnityEngine.UI;
using System.Collections;

public class UIWasteAlert : MonoBehaviour
{
    private MinigameManager _manager;

    [Header("Bars and Arrows")]
    [SerializeField] private BarArrowPair _poop;
    [SerializeField] private BarArrowPair _pee;
    [SerializeField] private BarArrowPair _hunger;
    [SerializeField] private BarArrowPair _unhappy;

    [System.Serializable]
    public class BarArrowPair
    {
        public string Root;
        public string ArrowName;
    }

    [Header("Alert Screen Root")]
    [SerializeField] private string _alertRootName;

    private Transform _alertRoot;

    [Header("UI Lookup names")]
    [SerializeField] private string _firstAlertScreenName;
    [SerializeField] private string _secondAlertScreenName;
    [SerializeField] private string _continueButton1Name;
    [SerializeField] private string _continueButton2Name;

    private GameObject _firstAlertScreen;
    private GameObject _secondAlertScreen;
    private Button _continueButton1;
    private Button _continueButton2;

    private UIScriptableObject _uiData;

    private GameObject _activeArrow;

    private enum AlertStage
    {
        None,
        First,
        Second
    }

    private AlertStage _currentStage = AlertStage.None;
    private bool _alertShown;
    private bool _gamePaused;

    private static bool _alertAlreadyTriggeredGlobally = false;

    private void Awake()
    {
        _uiData = UIMeterDataProvider.Shared;

        if (_uiData == null)
        {
            Debug.LogError("UIWasteAlert: No UIData found.");
            enabled = false;
            return;
        }

        if (_manager == null)
            _manager = FindFirstObjectByType<MinigameManager>();

        //_firstAlertScreen.SetActive(false);
        //_secondAlertScreen.SetActive(false);
        Debug.Log($"Poop bar at Awake: {_poop.Root}");
    }

    private void Start()
    {
        ResolveUIRoot();
        _manager = FindFirstObjectByType<MinigameManager>();

        _firstAlertScreen = FindUIObject(_firstAlertScreenName);
        _secondAlertScreen = FindUIObject(_secondAlertScreenName);
        _continueButton1 = FindButton(_continueButton1Name);
        _continueButton2 = FindButton(_continueButton2Name);

        if (_firstAlertScreen != null) _firstAlertScreen.SetActive(false);
        if (_secondAlertScreen != null) _secondAlertScreen.SetActive(false);

        if (_continueButton1 != null)
            _continueButton1.onClick.AddListener(Continue);

        if (_continueButton2 != null)
            _continueButton2.onClick.AddListener(Continue);

        StartCoroutine(HideArrowsAfterUIBuild());
    }

    private void OnEnable()
    {
        _uiData.PoopChanged += OnValueChanged;
        _uiData.PeeChanged += OnValueChanged;
        _uiData.HungryChanged += OnValueChanged;

        //_continueButton1.onClick.AddListener(OnContinueFirst);
        //_continueButton2.onClick.AddListener(OnContinueSecond);
    }

    private void OnDisable()
    {
        _uiData.PoopChanged -= OnValueChanged;
        _uiData.PeeChanged -= OnValueChanged;
        _uiData.HungryChanged -= OnValueChanged;

        //_continueButton1.onClick.RemoveListener(OnContinueFirst);
        //_continueButton2.onClick.RemoveListener(OnContinueSecond);
    }

    private void Update()
    {
        if (!_gamePaused) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            //if (_currentStage == AlertStage.First)
            //    OnContinueFirst();
            //else if (_currentStage == AlertStage.Second)
            //    OnContinueSecond();
            Continue();
        }
    }

    private void OnValueChanged(float value)
    {
        HideAllArrows();

        if (_alertAlreadyTriggeredGlobally) return;
        
        if(IsGameBlocked()) return; 

        if (_manager != null && _manager.MinigameIsRunning()) return;

        if (_uiData.GetPoop() >= 75f)
            TriggerAlertFromPair(_poop);
        else if (_uiData.GetPee() >= 75f)
            TriggerAlertFromPair(_pee);
        else if (_uiData.GetHungry() >= 75f)
            TriggerAlertFromPair(_hunger);
    }

    private void TriggerAlertFromPair(BarArrowPair pair)
    {
        HideAllArrows();

        GameObject arrow = FindArrow(pair);
        if (arrow == null) return;

        _alertShown = true;
        _alertAlreadyTriggeredGlobally = true;
        _activeArrow = arrow;

        ShowArrow(_activeArrow);

        _firstAlertScreen.SetActive(true);
        _secondAlertScreen.SetActive(false);

        _currentStage = AlertStage.First;
        StartCoroutine(PauseNextFrame());
    }

    private Transform FindBar(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            Debug.LogError($"Bar root '{name}' not found in scene!");
            return null;
        }
        return go.transform;
    }

    private GameObject FindUIObject(string name)
    {
        if (_alertRoot == null)
        {
            Debug.LogError("UI Root not resolved!");
            return null;
        }

        Transform t = FindChildRecursive(_alertRoot, name);
        if (t == null)
        {
            Debug.LogError($"UI object '{name}' not found under UI Root (recursive)!");
            return null;
        }

        return t.gameObject;
    }

    private Transform FindChildRecursive(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
                return child;

            Transform found = FindChildRecursive(child, targetName);
            if (found != null)
                return found;
        }
        return null;
    }

    private Button FindButton(string name)
    {
        GameObject go = FindUIObject(name);
        return go != null ? go.GetComponent<Button>() : null;
    }

    private GameObject FindArrow(BarArrowPair pair)
    {
        Transform barRoot = FindBar(pair.Root);
        if (barRoot == null) return null;

        Transform arrow = barRoot.Find(pair.ArrowName);
        if (arrow == null)
        {
            Debug.LogError($"Arrow '{pair.ArrowName}' not found under {pair.Root}");
            return null;
        }

        return arrow.gameObject;
    }

    private void ResolveUIRoot()
    {
        if (_alertRoot != null) return;

        GameObject go = GameObject.Find(_alertRootName);
        if (go == null)
        {
            Debug.LogError($"UI Root '{_alertRootName}' not found!");
            return;
        }

        _alertRoot = go.transform;
    }

    private IEnumerator HideArrowsAfterUIBuild()
    {
        // wait until UI is fully created & enabled
        yield return new WaitForEndOfFrame();

        HideAllArrows();
    }

    private void HideAllArrows()
    {
        HideArrow(FindArrow(_poop));
        HideArrow(FindArrow(_pee));
        HideArrow(FindArrow(_hunger));
        HideArrow(FindArrow(_unhappy));
    }

    private void ShowArrow(GameObject arrow)
    {
        if (arrow == null) return;
        arrow.SetActive(true);
    }

    private void HideArrow(GameObject arrow)
    {
        if (arrow == null) return;
        arrow.SetActive(false);
    }

    private void OnContinueFirst()
    {
        if (_currentStage != AlertStage.First) return;

        HideArrow(_activeArrow);

        // Show unhappy arrow on second screen
        _activeArrow = FindArrow(_unhappy);
        ShowArrow(_activeArrow);

        _firstAlertScreen.SetActive(false);
        _secondAlertScreen.SetActive(true);

        _currentStage = AlertStage.Second;
    }

    private void OnContinueSecond()
    {
        if (_currentStage != AlertStage.Second) return;

        HideArrow(_activeArrow);

        _secondAlertScreen.SetActive(false);
        ResumeGame();

        _currentStage = AlertStage.None;
    }

    private IEnumerator PauseNextFrame()
    {
        yield return null;
        PauseGame();
    }

    private void PauseGame()
    {
        if (_gamePaused) return;
        Time.timeScale = 0f;
        _gamePaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        _gamePaused = false;
    }

    private void Continue()
    {
        if (_currentStage == AlertStage.First)
            OnContinueFirst();
        else if (_currentStage == AlertStage.Second)
            OnContinueSecond();
    }

    private bool IsGameBlocked()
    {
        if (_manager != null && _manager.MinigameIsRunning())
            return true;

        LevelManager lm = FindFirstObjectByType<LevelManager>();
        if (lm != null && lm.IsGameOver())
            return true;

        return false;
    }

    //private void ShowAlert(GameObject arrow)
    //{
    //    if (_isFading) return;

    //    _activeArrow = arrow;
    //    _activeArrow.SetActive(true);

    //    _activeArrowGroup = _activeArrow.GetComponent<CanvasGroup>();
    //    if (_activeArrowGroup == null)
    //        _activeArrowGroup = _activeArrow.AddComponent<CanvasGroup>();

    //    _activeArrowGroup.alpha = 1f;

    //    _firstAlertScreen.SetActive(true);
    //    _messageGroup.alpha = 1f;

    //    // Trigger fade automatically
    //    Invoke(nameof(StartFadeOut), _visibleDuration);
    //}

    //private void StartFadeOut()
    //{
    //    _isFading = true;
    //    _fadeStartTime = Time.time;
    //}

    //private void FinishFadeOut()
    //{
    //    _isFading = false;

    //    _firstAlertScreen.SetActive(false);

    //    if (_activeArrow != null)
    //        _activeArrow.SetActive(false);

    //    _activeArrow = null;
    //    _activeArrowGroup = null;
    //}
}
