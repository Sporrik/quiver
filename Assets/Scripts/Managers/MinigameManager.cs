using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Transform _minigameSpawnPoint;

    [SerializeField] private List<string> _sceneNames;

    private int _currentMinigameIndex;
    private Scene _currentMinigameScene;
    private MinigameWinToggle _currentMinigameWinToggle = null;

    private bool _miniGameIsLoaded = false;
    private bool _miniGameIsPaused = false;

    private IEnumerator LoadScene(string sceneName, Vector3 sceneOffset)
    {
        // offsets the entire scene so it doesn't spawn inside of the levels

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        _currentMinigameScene = SceneManager.GetSceneByName(sceneName);

        foreach (GameObject obj in _currentMinigameScene.GetRootGameObjects())
        {
            obj.transform.position += sceneOffset;

            MinigameWinToggle winCond = obj.GetComponent<MinigameWinToggle>();

            if (winCond != null)
            {
                _currentMinigameWinToggle = winCond;
            }
        }

        yield return null;

        _miniGameIsLoaded = true;
    }

    private IEnumerator UnloadScene()
    {
        if (_currentMinigameScene.IsValid())
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_currentMinigameScene);
            yield return new WaitUntil(() => asyncUnload.isDone);
            Debug.Log($"Scene '{_sceneNames[_currentMinigameIndex]}' unloaded!");
        }
        else
        {
            Debug.LogWarning("No valid minigame scene to unload.");
        }
    }

    public void LoadMinigame(string sceneName)
    {
        for (int index = 0; index < _sceneNames.Count; ++index)
        {
            if (_sceneNames[index] == sceneName)
            {
                if (_miniGameIsLoaded) QuitMinigame();

                _currentMinigameIndex = index;

                _miniGameIsLoaded = true;
            }
        }

        StartCoroutine
        (
            LoadScene(_sceneNames[_currentMinigameIndex], _minigameSpawnPoint.position)
        );
    }

    public string QuitMinigame()
    {
        string name = _currentMinigameScene.name;
        _miniGameIsLoaded = false;
        _currentMinigameWinToggle = null;

        StartCoroutine(UnloadScene()); // error

        return name;
    }

    public void PauseMiniGame(bool pause)
    {
        _miniGameIsPaused = pause;

        GameObject[] objs = _currentMinigameScene.GetRootGameObjects();

        foreach (GameObject obj in objs)
        {
            Canvas canvas = obj.GetComponentInChildren<Canvas>();

            if (canvas != null)
            {
                canvas.enabled = !pause;
                continue; // assuming nobody puts a camera in a canvas
            }

            Camera cam = obj.GetComponentInChildren<Camera>();

            if (cam != null)
            {
                cam.enabled = !pause;
            }

        }

    }

    public bool MinigameIsRunning()
    {
        return _currentMinigameScene.IsValid();
    }

    public bool IsMinigamePaused()
    {
        return _miniGameIsPaused;
    }

    public Camera GetCamera()
    {
        
        return null;
    }

    public bool WonMinigame()
    {
        if (!_miniGameIsLoaded) return false;
        if (_currentMinigameWinToggle == null) return false;

        return _currentMinigameWinToggle.WonMinigame();
    }

}