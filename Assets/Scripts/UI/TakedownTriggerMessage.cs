using UnityEngine;

public class TakedownTriggerMessage : MonoBehaviour
{
    [Header("UI Object (must have a CanvasGroup)")]
    [SerializeField] private GameObject messageUI;

    [Header("Timings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private CanvasGroup _canvasGroup;
    private bool _hasTriggered = false;
    private bool _isAnimating = false;

    private float _animStartTime;
    private enum AnimState { None, FadeIn, VisibleWait, FadeOut }
    private AnimState _state = AnimState.None;

    private void Awake()
    {
        _canvasGroup = messageUI.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = messageUI.AddComponent<CanvasGroup>();

        messageUI.SetActive(false);
        _canvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        _hasTriggered = true;
        StartFadeIn();
    }

    private void Update()
    {
        if (!_isAnimating) return;

        float t = Time.time - _animStartTime;

        switch (_state)
        {
            case AnimState.FadeIn:
                float fadeInT = Mathf.Clamp01(t / fadeInDuration);
                _canvasGroup.alpha = fadeInT;

                if (fadeInT >= 1f)
                {
                    StartVisibleWait();
                }
                break;

            case AnimState.VisibleWait:
                if (t >= visibleDuration)
                    StartFadeOut();
                break;

            case AnimState.FadeOut:
                float fadeOutT = Mathf.Clamp01(t / fadeOutDuration);
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeOutT);

                if (fadeOutT >= 1f)
                    Finish();
                break;
        }
    }

    private void StartFadeIn()
    {
        messageUI.SetActive(true);
        _canvasGroup.alpha = 0f;

        _isAnimating = true;
        _animStartTime = Time.time;
        _state = AnimState.FadeIn;
    }

    private void StartVisibleWait()
    {
        _animStartTime = Time.time;
        _state = AnimState.VisibleWait;
    }

    private void StartFadeOut()
    {
        _animStartTime = Time.time;
        _state = AnimState.FadeOut;
    }

    private void Finish()
    {
        _isAnimating = false;
        _state = AnimState.None;

        messageUI.SetActive(false);
    }
}
