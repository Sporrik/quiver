using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private string _diaperMinigameScene;

    [SerializeField] private Transform _minigameSpawnPoint;

    private void Start()
    {
        // IEnumerator + yield return + StartCoroutline let's us skip time without pausing the game
        StartCoroutine(LoadScene(_diaperMinigameScene, _minigameSpawnPoint.position));

    }
    private void Update()
    {
        
    }

    private IEnumerator LoadScene(string sceneName, Vector3 sceneOffset)
    {
        // offsets the entire scene so it doesn't spawn inside of the levels

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        foreach (GameObject rootObject in loadedScene.GetRootGameObjects())
        {
            rootObject.transform.position += sceneOffset;
        }
    }
}