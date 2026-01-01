using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private ObjectiveUI _objectiveUIText;
    [SerializeField] public GameObject[] Goals;
    //  [SerializeField] GameObject _winFeedback;
    public int _goalScore = 0;

<<<<<<< HEAD
=======
    [SerializeField] private LevelManager _levelManager;

>>>>>>> FinishGameScreen
    [Header("Victory Settings")]
    [SerializeField] private GameObject victoryScreen; // UI Canvas / Panel
    [SerializeField] private string victorySceneName = "LevelTwo"; // scene where this applies

    void Start()
    {
        foreach (GameObject go in Goals)
        {
            go.SetActive(false);
        }
        Goals[0].SetActive(true);

        if (victoryScreen != null)
            victoryScreen.SetActive(false);
<<<<<<< HEAD
=======

        _levelManager = FindFirstObjectByType<LevelManager>();
>>>>>>> FinishGameScreen
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowNextGoal()
    {
        Goals[_goalScore].SetActive(false);
        _goalScore++;

        _objectiveUIText.OnGoalCompleted();

        if (_goalScore < Goals.Length)
        {
            Goals[_goalScore].SetActive(true);
        }
        else
        {
            TryShowVictoryScreen();
        }

        //if (_goalScore >= Goals.Length)
        //{
        //    _winFeedback.SetActive(true);
        //    Debug.Log("YOU WIN");
        //    return;
        //}
    }


    private void TryShowVictoryScreen()
    {
<<<<<<< HEAD
        // Check if we are in the correct scene
        if (SceneManager.GetActiveScene().name != victorySceneName)
            return;

        if (victoryScreen == null)
        {
            Debug.LogWarning("Victory Screen not assigned.");
            return;
        }

        // Pause the game
        Time.timeScale = 0f;

        // Show UI
        victoryScreen.SetActive(true);

        Debug.Log("YOU WIN!");
=======
        if (SceneManager.GetActiveScene().name == victorySceneName)
        {
            if (victoryScreen == null)
            {
                Debug.LogWarning("Victory Screen not assigned.");
                return;
            }

            Time.timeScale = 0f;
            victoryScreen.SetActive(true);

            Debug.Log("FINAL LEVEL COMPLETE!");
            return;
        }

        // Any other level -> load next level
        if (_levelManager != null)
        {
            Debug.Log("LEVEL COMPLETE, LOADING NEXT LEVEL");
            _levelManager.EnterNextLevelFromGoals();
        }
        else
        {
            Debug.LogWarning("LevelManager reference missing.");
        }
>>>>>>> FinishGameScreen
    }
}
