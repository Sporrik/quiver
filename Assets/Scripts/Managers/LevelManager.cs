using System.Collections.Generic;
using System.Linq;
//using TMPro.EditorUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gameplay.AI;
using Audio;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject _gameOverPanel;
    [SerializeField] private string[] _levelNames;

    private static int _currentLevel = 0;
    private static bool _loadFirstLevel = false;

    private bool _gameOver;
    [SerializeField] private float _timeUntilRestart = 2.5f;

    private GameObject _player;

    [SerializeField] private float _goalEnterDistance;
    private GameObject _goal;

    private List<GameObject> guards = new List<GameObject>();
    private List<GuardBehavior> guardBehaviors = new List<GuardBehavior>();

    private void Awake()
    {
        if (!_loadFirstLevel)
        {
            _loadFirstLevel = true;
            LoadLevel();
        }
        _gameOver = false;
        _gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        // temporary goal fix
        _player = GameObject.FindGameObjectWithTag("Player");
        _goal = GameObject.FindGameObjectWithTag("Goal");

        guards = new List<GameObject>(GameObject.FindGameObjectsWithTag("Guard"));

        guardBehaviors = new List<GuardBehavior>();

        for (var idx = 0; idx < guards.Count; idx++)
        {
            guardBehaviors.Add(guards[idx].GetComponent<GuardBehavior>());
        }

    }

    private void Update()
    {
        ManageLoseConditions();
        ManageWinCondition();
        ManageLevelReset();
    }

    private void ManageLoseConditions()
    {
        if (_gameOver) return;

        if (_player == null) return;

        //List<GameObject> guards = new List<GameObject>();
        //GameObject.FindGameObjectsWithTag("Guard", guards);
        if (guards.Count == 0) return;

        foreach (GuardBehavior guardBehavior in guardBehaviors)
        {
            if (guardBehavior == null) return;
            if (guardBehavior.DistanceToPlayer <= guardBehavior.CatchRange && guardBehavior.SeesPlayer)
            {
                TriggerGameOver();
                MusicController.instance.SetDeath();
            }
        }
    }

    private void ManageWinCondition()
    {
        if (_goal == null) return;

        if (_player == null) return;

        float playerToGoalDistance = (_goal.transform.position - _player.transform.position).magnitude;

        if (playerToGoalDistance < _goalEnterDistance)
        {
            EnterNextLevel();
        }
    }

    private void ManageLevelReset()
    {
        if (_gameOver && _timeUntilRestart <= 0f)
        {
            _gameOver = false;
            LoadLevel();
        }
        else if (_gameOver)
        {
            _timeUntilRestart -= Time.deltaTime;
        }
    }

    private void TriggerGameOver()
    {
        _gameOver = true;
        //_timeUntilRestart = 5f;
        _gameOverPanel.SetActive(true);
    }

    private void EnterNextLevel()
    {
        _currentLevel++;

        if (_currentLevel >= _levelNames.Count()) _currentLevel = 0;

        LoadLevel();
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(_levelNames[_currentLevel]);
    }

    public bool IsGameOver()
    {
        return _gameOver;
    }
}
