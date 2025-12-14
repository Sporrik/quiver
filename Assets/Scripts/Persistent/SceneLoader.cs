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
        SceneManager.LoadScene(_mainMenuScene, LoadSceneMode.Single);
    }

    public void StartGameAtLevel(int levelIndex)
    {
        _currentLevelIndex = levelIndex;
        LoadLevelInternal(levelScenes[levelIndex]);
    }

    public void LoadNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;
        if (nextIndex >= 0 && nextIndex < _levelScenes.Length)
        {
            
        }
    }
}
