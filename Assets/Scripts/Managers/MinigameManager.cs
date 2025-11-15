using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class MinigameManager : MonoBehaviour
{
    #region Inspector
    [Header("Placement")]
    [SerializeField] private Transform _minigameSpawnPoint;

    [Header("Minigame Scenes (names)")]
    [SerializeField] private List<string> _sceneNames = new();

    [Header("Options")]
    [Tooltip("If true, unloading then loading another minigame happens as a single operation")]
    [SerializeField] private bool _allowSwapWhileRunning = true;
    #endregion

    #region Events
    public event System.Action<string> Opened;     // sceneName
    public event System.Action<string> Closed;     // previous sceneName
    public event System.Action<string> LoadFailed; // sceneName
    public event System.Action Paused;
    public event System.Action Resumed;
    #endregion

    #region State
    private enum MiniState { Idle, Loading, Running, Paused, Unloading }
    [SerializeField] private MiniState _state = MiniState.Idle;

    private int _currentMinigameIndex = -1;
    private Scene _currentMinigameScene;

    public string CurrentSceneName => _currentMinigameScene.IsValid() ? _currentMinigameScene.name : string.Empty;
    public bool IsBusy => _state == MiniState.Loading || _state == MiniState.Unloading;
    #endregion

    #region Public API
    public void LoadMinigame(string sceneName)
    {
        int index = _sceneNames.FindIndex(n => n == sceneName);
        if (index < 0)
        {
            Debug.LogError($"{nameof(MinigameManager)}: Scene '{sceneName}' not found in list");
            LoadFailed?.Invoke(sceneName);
            return;
        }

        if (_state is MiniState.Running or MiniState.Paused && CurrentSceneName == sceneName) return;

        if (IsBusy)
        {
            Debug.LogWarning($"{nameof(MinigameManager)}: Busy '{_state}'. Try again later.");
            return;
        }

        if (_state is MiniState.Running or MiniState.Paused)
        {
            if (_allowSwapWhileRunning)
                StartCoroutine(CoSwap(_sceneNames[index]));
            else
                Debug.LogWarning($"{nameof(MinigameManager)}: A minigame is running; call QuitMinigame() first or enable swap.");
            return;
        }

        _currentMinigameIndex = index;
        StartCoroutine(CoLoad(_sceneNames[index]));
    }

    public string QuitMinigame()
    {
        if (!MinigameIsRunning()) return string.Empty;

        if (IsBusy)
        {
            Debug.LogWarning($"{nameof(MinigameManager)}: Busy {_state}. Quit Ignored.");
            return CurrentSceneName;
        }

        StartCoroutine(CoUnload());
        return CurrentSceneName;
    }

    public void PauseMinigame(bool paused)
    {
        if (!MinigameIsRunning()) return;

        if (paused && _state == MiniState.Running)
        {
            _state = MiniState.Paused;
            Paused?.Invoke();
        }
        else if (!paused && _state == MiniState.Paused)
        {
            _state = MiniState.Running;
            Resumed?.Invoke();
        }
    }

    public bool MinigameIsRunning() => _currentMinigameScene.IsValid();

    public bool IsMinigamePaused() => _state == MiniState.Paused;

    public Camera GetCamera()
    {
        if (!_currentMinigameScene.IsValid())
        {
            Debug.LogWarning($"{nameof(MinigameManager)} GetCamera() called but no minigame is running.");
            return null;
        }

        var roots = _currentMinigameScene.GetRootGameObjects();
        foreach (var root in roots)
        {
            var cam = root.GetComponentInChildren<Camera>();
            if (cam != null) return cam;
        }

        Debug.Log($"{nameof(MinigameManager)}: No camera found in minigame scene.");
        return null;
    }
    #endregion

    #region Coroutines
    private IEnumerator CoLoad(string sceneName)
    {
        _state = MiniState.Loading;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            Debug.LogError($"{nameof(MinigameManager)}: LoadSceneAsync returned null for '{sceneName}'.");
            LoadFailed?.Invoke(sceneName);
            _state = MiniState.Idle;
            yield break;
        }

        yield return new WaitUntil(() => asyncLoad.isDone);

        _currentMinigameScene = SceneManager.GetSceneByName(sceneName);
        if (!_currentMinigameScene.IsValid())
        {
            Debug.LogError($"[MinigameManager] Loaded scene handle invalid for '{sceneName}'.");
            LoadFailed?.Invoke(sceneName);
            _state = MiniState.Idle;
            yield break;
        }

        // Offset minigame roots to spawn position
        if (_minigameSpawnPoint != null)
        {
            Vector3 offset = _minigameSpawnPoint.position;
            foreach (GameObject root in _currentMinigameScene.GetRootGameObjects())
                root.transform.position += offset;
        }

        // Finish
        _state = MiniState.Running;
        _currentMinigameIndex = _sceneNames.FindIndex(n => n == sceneName);
        Opened?.Invoke(sceneName);
        yield return null;
    }

    private IEnumerator CoUnload()
    {
        _state = MiniState.Unloading;

        string prevName = CurrentSceneName;

        AsyncOperation unload = SceneManager.UnloadSceneAsync(_currentMinigameScene);
        if (unload == null)
        {
            Debug.LogWarning($"{nameof(MinigameManager)}: UnloadSceneAsync returned null; forcing state reset.");
            _currentMinigameScene = default;
            _currentMinigameIndex = -1;
            _state = MiniState.Idle;
            Closed?.Invoke(prevName);
            yield break;
        }

        yield return new WaitUntil(() => unload.isDone);

        _currentMinigameScene = default;
        _currentMinigameIndex = -1;
        _state = MiniState.Idle;
        Closed?.Invoke(prevName);
    }

    private IEnumerator CoSwap(string nextScene)
    {
        yield return CoUnload();
        yield return CoLoad(nextScene);
    }
    #endregion
}