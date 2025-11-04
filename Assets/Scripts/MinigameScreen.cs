using UnityEngine;
using UnityEngine.UI;

public class MinigameScreen : MonoBehaviour
{
    [SerializeField] private RawImage _camera;
    [SerializeField] private RawImage _border;

    [SerializeField] private GameObject _panel;

    private MinigameManager _manager;

    private Vector2 _lastMousePos;
    private Vector3 _panelStartPos;

    private bool _isDraggingPanel = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = GetComponent<MinigameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // temporary testing code
        if (Input.GetKeyUp(KeyCode.Space) && !_manager.MinigameIsRunning())
        {
            _manager.LoadMinigame("BabyDiaper");
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _manager.QuitMinigame();
        }
        // end test code

        DragPanel();

        _camera.texture = _manager.GetRenderTexture();
    }

    private void DragPanel()
    {
        float xDrag = GetDrag().x;

        if (Input.GetMouseButton(0))
        {
            if (!IsInsideImage(_camera, _lastMousePos) && IsInsideImage(_border, _lastMousePos))
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
