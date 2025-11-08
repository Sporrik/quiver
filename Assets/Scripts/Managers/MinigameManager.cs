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

    private bool _miniGameIsLoaded = false;
    private bool _miniGameIsPaused = false;

    private IEnumerator LoadScene(string sceneName, Vector3 sceneOffset)
    {
        // offsets the entire scene so it doesn't spawn inside of the levels

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        _currentMinigameScene = SceneManager.GetSceneByName(sceneName);

        foreach (GameObject rootObject in _currentMinigameScene.GetRootGameObjects())
        {
            rootObject.transform.position += sceneOffset;
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
        for(int index = 0;  index < _sceneNames.Count; ++index)
        {
            if( _sceneNames[index] == sceneName )
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

    public void QuitMinigame()
    {
        _miniGameIsLoaded = false;

        StartCoroutine(UnloadScene());
    }

    public void PauseMiniGame(bool pause)
    {
        _miniGameIsPaused = pause;
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
        GameObject[] objs = _currentMinigameScene.GetRootGameObjects();

        foreach (GameObject obj in objs)
        {
            Camera cam = obj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                return cam;
            }
        }

        Debug.Log("wtf");
        return null;
    }
}