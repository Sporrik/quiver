using UnityEngine;
using UnityEngine.UI;

public class MinigameScreen : MonoBehaviour
{
    [Header("Screen:")]
    [SerializeField] private RawImage _blackScreen;
    [SerializeField] private RawImage _border;
    [SerializeField] private GameObject _panel;

    [Header("Scene Names:")]
    [SerializeField] private string _diaperMinigame;
    [SerializeField] private string _peeMinigame;
    [SerializeField] private string _feedingMinigame;

    [Header("Slide Animation:")]
    [SerializeField] private Vector2 _clipPosition;
    [SerializeField] private float _slideSpeed = 2500f;

    [Header("Minigame Bars:")]
    [SerializeField] private float _maxProgress = 50f;
    [SerializeField] private UIManager _bars;

    private MinigameManager _manager;
    private Camera _minigameCamera;

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
        if (Input.GetKeyUp(KeyCode.Space) && !_manager.MinigameIsRunning())
        {
            _slideIn = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _slideOut = true;
        }

        if (_slideIn)
        {
            if (_bars.GetPoop() >= _maxProgress)
            {
                SlideIn(_diaperMinigame);
            }
            else if (_bars.GetPee() >= _maxProgress)
            {
                SlideIn(_peeMinigame);
            }
            else if (_bars.GetHungry() >= _maxProgress)
            {
                SlideIn(_feedingMinigame);
            }

        }

        if (_slideOut)
        {
            SlideOut();
        }
    }

    private void ResetMinigame()
    {
        string sceneName = _manager.QuitMinigame();

        if (sceneName == _diaperMinigame)
        {
            _bars.ResetPoop();
        }
        else if (sceneName == _peeMinigame)
        {
            _bars.ResetPee();
        }
        else if (sceneName == _feedingMinigame)
        {
            _bars.ResetHungry();
        }
    }

    private void ToggleScreen()
    {
        if (GotClipped())
        {
            //Debug.Log("Clipped screen to center!");
            _manager.PauseMiniGame(false);
            _blackScreen.enabled = false;

            if (_minigameCamera != null)
            {
                _minigameCamera.enabled = true;
            }
        }
        else
        {
            _manager.PauseMiniGame(true);
            _blackScreen.enabled = true;

            if (_minigameCamera != null)
            {
                _minigameCamera.enabled = false;
            }
        }

    }

    private void DragPanel()
    {
        if (_manager.MinigameIsRunning())
        {
            _minigameCamera = _manager.GetCamera();
        }

        float xDrag = GetDrag().x;

        if (Input.GetMouseButton(0))
        {
            if (!IsInsideImage(_blackScreen, _lastMousePos) && IsInsideImage(_border, _lastMousePos))
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

        return false;
    }

    public void SlideIn(string sceneName)
    {
        if (GotClipped() && !_manager.MinigameIsRunning())
        {
            _manager.LoadMinigame(sceneName);

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

    public void SlideOut()
    {
        if (_clipPosition.x - _panelWidth / 2 >= _panel.transform.position.x && _manager.MinigameIsRunning())
        {
            _panel.transform.position = new Vector3(_clipPosition.x - _panelWidth / 2, _panelStartPos.y, _panelStartPos.z);

            ResetMinigame();

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