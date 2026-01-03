using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public sealed class HideUIWhileMinigameIsRunning : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MinigameManager _minigameManager;
    [SerializeField] private MinigameScreen _minigameScreen;

    [Header("Behavior")]
    [SerializeField] private bool fade = true;
    [SerializeField] private float fadeDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_minigameManager == null)
        {
            _minigameManager = FindFirstObjectByType<MinigameManager>();
        }

        if (_minigameManager == null)
        {
            Debug.LogError(
                $"{nameof(HideUIWhileMinigameIsRunning)}: MinigameManager not found.",
                this
            );
        }

        if (_minigameScreen == null)
        {
            _minigameScreen = FindFirstObjectByType<MinigameScreen>();
        }
    }

    private void OnEnable()
    {
        if (_minigameScreen == null) return;

        _minigameScreen.ScreenShown += HandleScreenShown;
        _minigameScreen.ScreenHidden += HandleScreenHidden;
    }

    private void OnDisable()
    {
        if (_minigameScreen == null) return;

        _minigameScreen.ScreenShown -= HandleScreenShown;
        _minigameScreen.ScreenHidden -= HandleScreenHidden;
    }

    private void HandleScreenShown()
    {
        SetVisible(false);
    }

    private void HandleScreenHidden()
    {
        SetVisible(true);
    }

    private void HandleMinigameOpened(string sceneName)
    {
        SetVisible(false);
    }

    private void HandleMinigameClosed(string sceneName)
    {
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        if (fade)
            _fadeRoutine = StartCoroutine(FadeTo(visible ? 1f : 0f));
        else
            ApplyState(visible ? 1f : 0f);
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        ApplyState(targetAlpha);
    }

    private void ApplyState(float alpha)
    {
        _canvasGroup.alpha = alpha;
        _canvasGroup.interactable = alpha > 0f;
        _canvasGroup.blocksRaycasts = alpha > 0f;
    }
}
