using UnityEngine;

public enum GameState { START, PLAYING, END };

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool _gamePaused = false; //game pause check variable
    private GameState _gameState;

    public GameState gameState { get { return _gameState; } }
    public bool gamePaused { get { return _gamePaused; } }

    public void GamePaused()
    {
        _gamePaused = true;
    }

    public void GameResume()
    {
        _gamePaused = false;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _gameState = GameState.START;
        _gamePaused = false;
    }
}