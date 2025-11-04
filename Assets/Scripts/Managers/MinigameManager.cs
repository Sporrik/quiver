using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private string _diaperMinigameScene;
    [SerializeField] private RawImage _diaperCam;

    [SerializeField] private GameObject _panel;

    [SerializeField] private Transform _minigameSpawnPoint;

    private Scene _currentMinigame;

    private Vector2 _lastMousePos;
    private Vector3 _panelStartPos;

    private bool _isDraggingPanel = false;

    private bool _miniGameIsLoaded = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space) && !_miniGameIsLoaded)
        {
            StartCoroutine(LoadScene(_diaperMinigameScene, _minigameSpawnPoint.position));
            _miniGameIsLoaded = true;
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(UnloadScene());
            _miniGameIsLoaded = false;
        }
        
        DragPanel();
    }

    private void DragPanel()
    {
        float xDrag = GetDrag().x;

        if (Input.GetMouseButton(0))
        {
            if (!IsInsideImage(_diaperCam, _lastMousePos))
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

        if( _isDraggingPanel && Input.GetMouseButton(0) )
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

    private IEnumerator LoadScene(string sceneName, Vector3 sceneOffset)
    {
        // offsets the entire scene so it doesn't spawn inside of the levels

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        _currentMinigame = SceneManager.GetSceneByName(sceneName);

        foreach (GameObject rootObject in _currentMinigame.GetRootGameObjects())
        {
            rootObject.transform.position += sceneOffset;
        }
    }

    private IEnumerator UnloadScene()
    {
        if (_currentMinigame.IsValid())
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_currentMinigame);
            yield return new WaitUntil(() => asyncUnload.isDone);
            Debug.Log($"Scene '{_currentMinigame.name}' unloaded!");
        }
        else
        {
            Debug.LogWarning("No valid minigame scene to unload.");
        }
    }

    private bool IsInsideImage(RawImage image, Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, pos);
    }
}