using UnityEngine;
using Audio;

public sealed class MusicHooks_Menu : MonoBehaviour
{
    private void OnEnable()
    {
        if (MusicController.instance != null)
            MusicController.instance.SetMenu();
    }
}

public sealed class MusicHooks_Level : MonoBehaviour
{
    private void OnEnable()
    {
        if (MusicController.instance != null)
            MusicController.instance.SetMenu();
    }

    public void OnChaseStarted() => MusicController.instance?.SetChase(true);
    public void OnChaseStopped() => MusicController.instance?.SetChase(false);
    public void OnMinigameOpen() => MusicController.instance?.SetMinigame(true);
    public void OnMinigameClose() => MusicController.instance?.SetMinigame(false);
}