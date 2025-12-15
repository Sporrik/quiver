using UnityEngine;

public class UICompletion : MonoBehaviour
{
    [SerializeField] private float _duration = 1f; // Duration of the animation

    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (_rectTransform == null)
            return;

        StartCoroutine(Animate());
    }

    private System.Collections.IEnumerator Animate()
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float progress = elapsed / _duration;

            float rotationZ = Mathf.Lerp(0f, 720f, progress);
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);

            float scale = Mathf.Lerp(0f, 1f, progress);
            _rectTransform.localScale = new Vector3(scale, scale, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, 720f);
        _rectTransform.localScale = Vector3.one;
    }
}
