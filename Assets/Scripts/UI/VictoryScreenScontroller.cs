using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryScreenAnimator : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float panelSlideDuration = 1.5f;
    [SerializeField]
    private AnimationCurve easeCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [System.Serializable]
    public class VictoryText
    {
        public RectTransform rect;
        public CanvasGroup canvasGroup;
        public float duration = 1f;
    }

    [Header("Text Elements")]
    [SerializeField] private VictoryText[] texts;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private KeyCode[] confirmKeys = { KeyCode.E, KeyCode.Space };

    private Vector2 panelTargetPos;
    private Vector2 panelStartPos;

    private void Awake()
    {
        panelTargetPos = panelRect.anchoredPosition;
        panelStartPos = panelTargetPos + new Vector2(Screen.width, 0);

        panelRect.anchoredPosition = panelStartPos;

        foreach (var text in texts)
        {
            if (text.canvasGroup != null)
                text.canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        StartCoroutine(AnimatePanel());
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        foreach (var key in confirmKeys)
        {
            if (Input.GetKeyDown(key))
            {
                LoadNextScene();
                break;
            }
        }
    }

    private IEnumerator AnimatePanel()
    {
        yield return AnimateRect(
            panelRect,
            panelStartPos,
            panelTargetPos,
            panelSlideDuration
        );

        // After panel animation -> animate texts
        foreach (var text in texts)
        {
            StartCoroutine(AnimateText(text));
        }
    }

    private IEnumerator AnimateText(VictoryText textData)
    {
        RectTransform rect = textData.rect;
        CanvasGroup group = textData.canvasGroup;

        Vector2 targetPos = rect.anchoredPosition;
        Vector2 startPos = targetPos + new Vector2(Screen.width / 2f, 0);

        rect.anchoredPosition = startPos;
        group.alpha = 0f;

        yield return AnimateRect(rect, startPos, targetPos, textData.duration, group);
    }

    private IEnumerator AnimateRect(
        RectTransform rect,
        Vector2 start,
        Vector2 end,
        float duration,
        CanvasGroup group = null
    )
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float easedT = easeCurve.Evaluate(t);

            rect.anchoredPosition = Vector2.Lerp(start, end, easedT);

            if (group != null)
                group.alpha = Mathf.Lerp(0f, 1f, easedT);

            yield return null;
        }

        rect.anchoredPosition = end;
        if (group != null) group.alpha = 1f;
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}