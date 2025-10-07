using UnityEngine;

public enum GameState { START, PLAYING, END};

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState _gameState;
    public GameState gameState { get { return _gameState; } }

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
    }
}