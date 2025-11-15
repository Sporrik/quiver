using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinigameScreen : MonoBehaviour
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
    [SerializeField] private UIManager _uiManager;

    private MinigameManager _manager;
    // TODO add a way to read the win conditions of minigame

    private Vector2 _lastMousePos;
    private Vector3 _panelStartPos;

    private bool _isDraggingPanel = false;
    private bool _slideIn = false;
    private bool _slideOut = false;

    private float _panelWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        DragPanel();

        ToggleScreen();
    }

    private void SelectMiniGame()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !GotClipped())
        {
            _slideIn = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _slideOut = true;
        }

        if (_slideIn)
        {
            if (_uiManager.GetPoop() >= _maxProgress)
            {
                SlideIn(_diaperMinigame);
            }
            else if (_uiManager.GetPee() >= _maxProgress)
            {
                SlideIn(_peeMinigame);
            }
            else if (_uiManager.GetHungry() >= _maxProgress)
            {
                SlideIn(_feedingMinigame);
            }

        }

        if (_slideOut)
        {
            SlideOut(false);
        }

        if(_manager.WonCurrentMinigame())
        {
            SlideOut(true);
        }
    }

    private void ResetMinigame()
    {
        string sceneName = _manager.QuitMinigame();

        if (sceneName == _diaperMinigame)
        {
            _uiManager.ResetPoop();
        }
        else if (sceneName == _peeMinigame)
        {
            _uiManager.ResetPee();
        }
        else if (sceneName == _feedingMinigame)
        {
            _uiManager.ResetHungry();
        }
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

    public bool GotClipped()
    {
        _panelStartPos = _panel.transform.position;

        if (_clipPosition.x + _panelWidth / 2 <= _panel.transform.position.x)
        {
            _panel.transform.position = new Vector3(_clipPosition.x + _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);

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
}