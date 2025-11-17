using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MinigameInputGate : MonoBehaviour
{
    [SerializeField] private MinigameManager _minigameManager;
    [SerializeField] private PlayerInputRelay _inputRelay;
    private bool _blocked;

    private void OnEnable()
    {
        if (_minigameManager == null || _inputRelay == null)
        {
            Debug.LogError($"{nameof(MinigameInputGate)}: Assign references in the Inspector");
            return;
        }

        _minigameManager.Opened  += HandleOpened;
        _minigameManager.Closed  += HandleClosed;
        _minigameManager.Paused  += HandleOpened;
        _minigameManager.Resumed += HandleClosed;

        if (_minigameManager.MinigameIsRunning()) HandleOpened("_");
    }

    private void OnDisable()
    {
        if (_minigameManager != null)
        {
            _minigameManager.Opened -= HandleOpened;
            _minigameManager.Closed -= HandleClosed;
        }

        if (_blocked && _inputRelay != null)
        {
            _inputRelay.EndBlock(this);
            _blocked = false;
        }
    }

    private void HandleOpened(string _)
    {
        if (_blocked) return;
        _inputRelay.BeginBlock(this);
        _blocked = true;
    }

    private void HandleClosed(string _)
    {
        if (!_blocked) return;
        _inputRelay.EndBlock(this);
        _blocked = false;
    }

    private void HandleOpened()
    {
        if (_blocked) return;
        _inputRelay.BeginBlock(this);
        _blocked = true;
    }

    private void HandleClosed()
    {
        if (!_blocked) return;
        _inputRelay.EndBlock(this);
        _blocked = false;
    }
}