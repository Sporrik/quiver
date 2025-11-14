using UnityEngine;

public class MinigameWinToggle : MonoBehaviour
{
    private bool _wonMinigame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void WinMinigame()
    {
        _wonMinigame = true;
    }

    public bool WonMinigame()
    {
        return _wonMinigame;
    }
}
