using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int _currentLevel;

    private void NewLevel()
    {
        _currentLevel++;

        switch (_currentLevel)
        {
            case 1:
                SceneManager.LoadScene("Map_1");
                break;
            case 2:
                SceneManager.LoadScene("Map_2");
                break;
            case 3:
                SceneManager.LoadScene("Map_3");
                break;
        }
    }
}
