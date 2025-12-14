using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string _mainMenuScene;

    [SerializeField] private string[] _levelScenes;
    [SerializeField] private string[] _minigameScenes;

    private int _currentLevelIndex = -1;
    private string _currentMinigameScene = null;

    public void LoadMainMenu()
    {
        // Unload all non-persistent scenes
        SceneManager.LoadSceneAsync(_mainMenuScene, LoadSceneMode.Additive);
    }

    public void StartGameAtLevel(int levelIndex)
    {
        _currentLevelIndex = levelIndex;
        LoadLevelInternal(_levelScenes[levelIndex]);
    }

    public void LoadNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;
        if (nextIndex >= 0 && nextIndex < _levelScenes.Length)
        {
            _currentLevelIndex = nextIndex;
            LoadLevelInternal(_levelScenes[nextIndex]);
        }
        else
        {
            LoadMainMenu();
        }
    }

    private void LoadLevelInternal(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void LoadMinigame(int minigameIndex)
    {
        if (_currentMinigameScene != null) return;
        _currentMinigameScene = _minigameScenes[minigameIndex];
        SceneManager.LoadSceneAsync(_currentMinigameScene, LoadSceneMode.Additive);
    }

    public void UnloadMinigame()
    {
        if (_currentMinigameScene == null) return;
        SceneManager.UnloadSceneAsync(_currentMinigameScene);
        _currentMinigameScene = null;
    }
}