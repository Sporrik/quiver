using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.InputSystem;

public sealed class MinigameScreen : MonoBehaviour
{
    [Header("Screen:")]
    [SerializeField] private RawImage _blackScreen;
    [SerializeField] private RawImage _border;
    [SerializeField] private RawImage _minigameArea;
    [SerializeField] private GameObject _panel;
    [SerializeField] private float _borderScaleOnFullscreen = 1f;

    [Header("Scene Names:")]
    [SerializeField] private string _diaperMinigame;
    [SerializeField] private string _peeMinigame;
    [SerializeField] private string _feedingMinigame;

    [Header("Slide Animation:")]
    [SerializeField] private Vector2 _clipPosition;
    [SerializeField] private float _slideSpeed = 2500f;

    [Header("Minigame Bars:")]
    [SerializeField] private float _maxProgress = 50f;
    [SerializeField] private GameObject _visualsBars;
    [SerializeField] private BarManager _barManager;
    [SerializeField] private UIScriptableObject _uiData;

    [Header("Controls")]
    [SerializeField] private InputActionAsset _input;
    private InputAction _openOrCloseTablet;

    [Header("Player Input:")]
    [SerializeField] private PlayerInput _playerInput;

    private MinigameManager _manager;
    // TODO add a way to read the win conditions of minigame

    private Vector2 _lastMousePos;
    private Vector3 _panelStartPos;

    private string _pendingSceneName;
    private bool _isDraggingPanel = false;
    private bool _slideIn = false;
    private bool _slideOut = false;

    private float _panelWidth;

    private float _halfWidth;

    //To hide UI
    public event System.Action ScreenShown;
    public event System.Action ScreenHidden;

    public bool UsingController = false;
    public string ControllerType = "Unknown";
    private string _previousScheme = "Irrelevant";

    void Start()
    {
        if(_playerInput == null)
        {
            Debug.LogWarning($"{name}, does not have a player input to disable!");
        }

        _barManager = GameManager.instance.gameObject.GetComponent<BarManager>();
        if (_barManager == null) { Debug.LogError($"{nameof(MinigameScreen)}: BarManager not set.", this); return; }

        if (_input != null)
        {
            _openOrCloseTablet = _input.FindActionMap("MinigameScreen").FindAction("Toggle");
        }
        else
        {
            Debug.LogError($"{nameof(MinigameScreen)}: _Input is null");
        }

            _manager = GetComponent<MinigameManager>();
        _minigameArea.enabled = false;

        if(_visualsBars != null)
        {
            _visualsBars.SetActive(true);
        }

        RectTransform rectangle = _panel.GetComponent<RectTransform>();
        rectangle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);

        _panelWidth = rectangle.rect.width;

        _panelStartPos = _panel.transform.position;
        _panel.transform.position = new Vector3(_clipPosition.x - _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);

        _halfWidth = _panelWidth / 2;
    }

    void Update()
    {
        SelectMiniGame();

        //DragPanel();

        ToggleScreen();
        DetectInput();
    }

    private void DetectInput()
    {
        if (UsingController)
            return;

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

    private void HandleNeedFilled(BarManager.NeedType need)
    {
        _pendingSceneName = need switch
        {
            BarManager.NeedType.Poop => _diaperMinigame,
            BarManager.NeedType.Pee => _peeMinigame,
            BarManager.NeedType.Hungry => _feedingMinigame,
            _ => null
        };

        if (!string.IsNullOrEmpty(_pendingSceneName)) _slideIn = true;
    }

    // works fine
    private void SelectMiniGame()
    {
        if (UIGlobalBlocker.IsModalUIOpen)
            return;

        if (UIInputBlocker.BlockGameplayInput)
            return;

        //if (Input.GetKeyUp(KeyCode.Space))
        //Fin: I changed wasreleasedthisframe() to waspressedthisframe(), I'm sorry
        if (_openOrCloseTablet.WasPressedThisFrame()) // on releasing botton
        {
            if (!GotClipped() && !_slideOut)
            {
                TryOpenBySpace();
                //_playerInput.enabled = false;
            }
            else if (!_slideIn)
            {
                
                _slideOut = true;
            }
        }

        // ensuring the screen doesn't get pushed in two directions at once
        // (which causes it to freeze in place)
        if (_manager.WonCurrentMinigame())
        {
            SlideOut(true);
            return;
        }

        if (_slideIn && !string.IsNullOrEmpty(_pendingSceneName)) SlideIn(_pendingSceneName);
        if (_slideOut) SlideOut(false);
    }

    private void ResetMinigame()
    {
        Debug.Log("resetting minigame!");

        string sceneName = _manager.QuitMinigame();
        if (_uiData == null) return;

        if (sceneName == _diaperMinigame)       _uiData.ResetPoop();
        else if (sceneName == _peeMinigame)     _uiData.ResetPee();
        else if (sceneName == _feedingMinigame) _uiData.ResetHungry();

        _pendingSceneName = null;
    }

    private void ToggleScreen()
    {
        if (GotClipped())
        {
            //Debug.Log("Clipped screen to center!");

            _manager.PauseMinigame(false);

            if (!_manager.MinigameIsRunning()) return;
            
            _blackScreen.enabled = false;

            if (_visualsBars != null)
            {
                _visualsBars.SetActive(false);
            }
            
        }
        else
        {
            _manager.PauseMinigame(true);
            _blackScreen.enabled = true;

            if (_visualsBars != null) _visualsBars.SetActive(true);
        }

    }

    // clipping gets registered
    public bool GotClipped()
    {
        //_panelStartPos = _panel.transform.position;

        if (_clipPosition.x + _halfWidth <= _panel.transform.position.x)
        {
            _panel.transform.position = new Vector3(_clipPosition.x + _halfWidth, _panelStartPos.y, _panelStartPos.z);

            return true;
        }
        else
        {
            _border.transform.localScale = Vector3.one;
        }

        float progress = Mathf.Max(_panel.transform.position.x, 0) / _halfWidth;

        float scale = _borderScaleOnFullscreen * progress;

        scale = Mathf.Max(scale, 1);

        _border.transform.localScale = new Vector3(scale, scale, scale);
        _blackScreen.transform.localScale = new Vector3(scale, scale, scale);

        return false;
    }

    public void SlideIn(string sceneName)
    {
        if (GotClipped())
        {
            Debug.Log("slid in!");

            if (!_manager.MinigameIsRunning())
            {
                _manager.LoadMinigame(sceneName);

                ScreenShown?.Invoke(); //This was Fin
            }
            _playerInput.enabled = true;
            _playerInput.ActivateInput();

            _slideIn = false;
        }

        _panel.transform.position = new Vector3
            (
            _panel.transform.position.x + _slideSpeed * Time.deltaTime,
            _panelStartPos.y,
            _panelStartPos.z
            );
    }

    public void SlideOut(bool unloadScene = false)
    {
        if (_clipPosition.x - _halfWidth >= _panel.transform.position.x)
        {
            Debug.Log("slid out!");

            if (unloadScene) ResetMinigame();

            _panel.transform.position = new Vector3(_clipPosition.x - _halfWidth, _panelStartPos.y, _panelStartPos.z);

            ScreenHidden?.Invoke();

            _slideOut = false;

            _playerInput.DeactivateInput();
            _playerInput.enabled = false;

            return;
        }

        _panel.transform.position = new Vector3
            (
            _panel.transform.position.x - _slideSpeed * Time.deltaTime,
            _panelStartPos.y,
            _panelStartPos.z
            );
    }

    private void TryOpenBySpace()
    {
        if (_uiData == null) return;

        if(!_manager.MinigameIsRunning())
        {
            float poop = _uiData.GetPoop();
            float pee = _uiData.GetPee();
            float hungry = _uiData.GetHungry();

            // Check threshold
            bool poopOk = poop >= _maxProgress, peeOk = pee >= _maxProgress, hungryOk = hungry >= _maxProgress;
            if (!poopOk && !peeOk && !hungryOk) return;


            if (poop >= _maxProgress) _pendingSceneName = _diaperMinigame;

            else if (pee >= _maxProgress) _pendingSceneName = _peeMinigame;

            else if(hungry >= _maxProgress) _pendingSceneName = _feedingMinigame;
        }

        _slideIn = true;
    }

    // these functions work, but aren't used at the moment
    // we're keeping them in case we have a change of mind
    private void DragPanel()
    {
        float xDrag = GetDrag().x;

        if (Input.GetMouseButton(0))
        {
            if (!IsInsideImage(_minigameArea, _lastMousePos) && IsInsideImage(_border, _lastMousePos))
            {
                _isDraggingPanel = true;

                _panelStartPos = _panel.transform.position;
                _lastMousePos = Input.mousePosition;
            }
            else
            {
                _isDraggingPanel = false;
            }

            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingPanel = false;
        }

        if (_isDraggingPanel && Input.GetMouseButton(0))
        {
            Vector2 currentMouse = Input.mousePosition;

            Vector3 targetPos = new Vector3(_panelStartPos.x + xDrag, _panelStartPos.y, _panelStartPos.z);

            _panel.transform.position = targetPos;
        }
    }

    private Vector2 GetDrag()
    {
        Vector2 currentMousePos = Input.mousePosition;

        Vector2 delta = Vector2.zero;

        delta = currentMousePos - _lastMousePos;

        _lastMousePos = currentMousePos;
        return delta;
    }

    private bool IsInsideImage(RawImage image, Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, pos);
    }
}