using UnityEngine;
using Audio;

public sealed class MusicHooks_Menu : MonoBehaviour
{
    private void OnEnable()
    {
        MusicController.instance?.SetMenu();
    }
}

public sealed class MusicHooks_Level : MonoBehaviour
{
    private void OnEnable()
    { 
        MusicController.instance?.SetGameplay();
        AudioBus.Ambience(SoundID.Ambience_Main);
    }

    public void OnChaseStarted() => MusicController.instance?.SetChase(true);
    public void OnChaseStopped() => MusicController.instance?.SetChase(false);
    public void OnMinigameOpen() => MusicController.instance?.SetMinigame(true);
    public void OnMinigameClose() => MusicController.instance?.SetMinigame(false);
}