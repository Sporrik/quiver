using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UI;

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

    private MinigameManager _manager;
    // TODO add a way to read the win conditions of minigame

    private Vector2 _lastMousePos;
    private Vector3 _panelStartPos;

    private string _pendingSceneName;
    private bool _isDraggingPanel = false;
    private bool _slideIn = false;
    private bool _slideOut = false;

    private float _panelWidth;

    private void OnEnable()
    {
        if (_barManager == null) { Debug.LogError($"{nameof(MinigameScreen)}: BarManager not set.", this); return; }
        _barManager.OnNeedFilled += HandleNeedFilled;
    }

    private void OnDisable()
    {
        if (_barManager != null) _barManager.OnNeedFilled -= HandleNeedFilled;
    }

    void Start()
    {
        _manager = GetComponent<MinigameManager>();
        _minigameArea.enabled = false;

        if(_visualsBars != null)
        {
            _visualsBars.SetActive(true);
        }

        _panelWidth = _panel.GetComponent<RectTransform>().rect.width;
        _panelStartPos = _panel.transform.position;
        _panel.transform.position = new Vector3(_clipPosition.x - _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);
    }

    void Update()
    {
        SelectMiniGame();

        //DragPanel();

        ToggleScreen();
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!GotClipped())
            {
                TryOpenBySpace();
            }
            else _slideOut = true;
        }

        if (_slideIn && !string.IsNullOrEmpty(_pendingSceneName)) SlideIn(_pendingSceneName);
        if (_slideOut) SlideOut(false);
        if (_manager.WonCurrentMinigame()) SlideOut(true);
    }

    private void ResetMinigame()
    {
        string sceneName = _manager.QuitMinigame();
        if (_uiData == null) return;

        if (sceneName == _diaperMinigame)       _uiData.ResetPoop();
        else if (sceneName == _peeMinigame)     _uiData.ResetPee();
        else if (sceneName == _feedingMinigame) _uiData.ResetHungry();

        _pendingSceneName = null;
        _slideIn = false;
    }

    private void ToggleScreen()
    {
        if (GotClipped())
        {
            //Debug.Log("Clipped screen to center!");

            _manager.PauseMinigame(false);

            if(_manager.MinigameIsRunning())
            {
                _blackScreen.enabled = false;

                if (_visualsBars != null)
                {
                    _visualsBars.SetActive(false);
                }
            }
        }
        else
        {
            _manager.PauseMinigame(true);
            _blackScreen.enabled = true;

            if (_visualsBars != null)
            {
                _visualsBars.SetActive(true);
            }
        }

    }

    // dragging works!
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

    // clipping gets registered
    public bool GotClipped()
    {
        _panelStartPos = _panel.transform.position;

        if (_clipPosition.x + _panelWidth / 2 <= _panel.transform.position.x)
        {
            _panel.transform.position = new Vector3(_clipPosition.x + _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);

            //Debug.Log("Got clipped!");

            return true;
        }
        else
        {
            _border.transform.localScale = Vector3.one;
        }

        float progress = Mathf.Max(_panel.transform.position.x, 0) / (_panelWidth / 2);

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
            if (!_manager.MinigameIsRunning())
            {
                _manager.LoadMinigame(sceneName);
            }

            _slideIn = false;
        }
        else if (!GotClipped())
        {
            _panel.transform.position = Vector3.MoveTowards
            (
                _panel.transform.position,
                new Vector3(_clipPosition.x + _panelWidth / 2, _panelStartPos.y, _panelStartPos.z),
                _slideSpeed * Time.deltaTime
            );
        }
    }

    public void SlideOut(bool unloadScene = false)
    {
        if (_clipPosition.x - _panelWidth / 2 >= _panel.transform.position.x)
        {
            _panel.transform.position = new Vector3(_clipPosition.x - _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);

            if(unloadScene) ResetMinigame();

            _slideOut = false;
        }
        else if (_clipPosition.x - _panelWidth / 2 < _panel.transform.position.x)
        {
            _panel.transform.position = Vector3.MoveTowards
            (
                _panel.transform.position,
                new Vector3(_clipPosition.x - _panelWidth / 2, _panelStartPos.y, _panelStartPos.z),
                _slideSpeed * Time.deltaTime
            );
        }
    }

    private void TryOpenBySpace()
    {
        if (_uiData == null) return;

        float poop = _uiData.GetPoop();
        float pee = _uiData.GetPee();
        float hungry = _uiData.GetHungry();

        // Check threshold
        bool poopOk = poop >= _maxProgress, peeOk = pee >= _maxProgress, hungryOk = hungry >= _maxProgress;
        if (!poopOk && !peeOk && !hungryOk) return;


        if (poop >= _maxProgress) _pendingSceneName = _diaperMinigame;

        else if (pee >= _maxProgress) _pendingSceneName = _peeMinigame;

        //else if(hungry >= _maxProgress) _pendingSceneName = _feedingMinigame;     //=> not implemented yet so it causes errors now
        
        //Debug.Log(_pendingSceneName);

        _slideIn = true;


        // an old fix/redo which may have been a solution for a bug, but we don't know what
        // it's supposed to change in terms of functionality

        //// Pick the most filled among those >= threshold (tie priority: Poop > Pee > Hungry)
        //if (poopOk && poop >= Mathf.Max(peeOk ? pee : -1f, hungryOk ? hungry : -1f))
        //{
        //    _pendingSceneName = _diaperMinigame;
        //}
        //else if (peeOk && pee >= Mathf.Max(hungryOk ? hungry : -1f))
        //{
        //    _pendingSceneName = _peeMinigame;
        //}
        //else
        //{
        //    _pendingSceneName = _feedingMinigame;
        //}
    }
}