using UnityEngine;
using Audio;
using UI;

public class PersistentContext : MonoBehaviour
{
    [Header("Core Systems")]
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MinigameManager _minigameManager;
    [SerializeField] private MusicController _musicController;

    [Header("Global UI")]
    [SerializeField] private BarManager _barManager;
    [SerializeField] private UIMeterDataProvider _uiMeterDataProvider;
    [SerializeField] private UIDataInitializer _uiDataInitializer;
    [SerializeField] private MinigameScreen _minigameScreen;

    [Header("Integrations")]
    [SerializeField] private TwitchGameManager _twitchGameManager;

    // Public API
    public SceneLoader SceneLoader => _sceneLoader;
    public GameManager GameManager => _gameManager;
    public MinigameManager MinigameManager => _minigameManager;
    public MusicController MusicController => _musicController;
    public BarManager BarManager => _barManager;
    public UIMeterDataProvider UIMeterDataProvider => _uiMeterDataProvider;
    public UIDataInitializer UIDataInitializer => _uiDataInitializer;
    public MinigameScreen MinigameScreen => _minigameScreen;
    public TwitchGameManager TwitchGameManager => _twitchGameManager;
}