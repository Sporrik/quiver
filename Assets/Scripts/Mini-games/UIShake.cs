using UnityEngine;

public class UIShake : MonoBehaviour
{
    [SerializeField] private float _scaleDuration = 0.25f; // seconds per call (calls add)
    [SerializeField] private float _scaleFactor = 1.2f; // how much to grow (e.g., 1.2 = 20% larger)
    [SerializeField] private bool _useUnscaledTime = true;

    private RectTransform _rectTransform;
    private Vector3 _originalScale;

    // Runtime scale state
    private Coroutine _scaleCoroutine;
    private float _remainingScaleTime;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform != null)
            _originalScale = _rectTransform.localScale;
    }

    public void ShakeUI()
    {
        if (_rectTransform == null)
            return;

        // Add duration so rapid calls extend the scaling effect
        _remainingScaleTime += _scaleDuration;

        // Start coroutine if not already running
        if (_scaleCoroutine == null)
            _scaleCoroutine = StartCoroutine(ScaleRoutine());
    }

    private System.Collections.IEnumerator ScaleRoutine()
    {
        float elapsed = 0f;
        bool scalingUp = true;

        while (_remainingScaleTime > 0f)
        {
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _remainingScaleTime -= dt;
            elapsed += dt;

            // Determine the progress of the scaling effect
            float progress = Mathf.Clamp01(elapsed / (_scaleDuration / 2f)); // Half duration for up/down

            if (scalingUp)
            {
                // Scale up
                _rectTransform.localScale = Vector3.Lerp(_originalScale, _originalScale * _scaleFactor, progress);

                if (progress >= 1f)
                {
                    scalingUp = false;
                    elapsed = 0f; // Reset for scaling down
                }
            }
            else
            {
                // Scale back down
                _rectTransform.localScale = Vector3.Lerp(_originalScale * _scaleFactor, _originalScale, progress);

                if (progress >= 1f)
                {
                    scalingUp = true;
                    elapsed = 0f; // Reset for next cycle
                }
            }

            yield return null;
        }

        // Ensure the scale is restored to the original size
        _rectTransform.localScale = _originalScale;

        // Reset state
        _remainingScaleTime = 0f;
        _scaleCoroutine = null;
    }
}
