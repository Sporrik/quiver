using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform objectiveRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text objectiveText;

    [Header("Objectives")]
    [TextArea(2, 4)]
    [SerializeField] private string[] objectives;

    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField]
    private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 targetPos;
    private Vector2 centerPos;
    private int currentObjectiveIndex = 0;
    private Coroutine currentRoutine;

    private void Awake()
    {
        targetPos = objectiveRect.anchoredPosition;

        // Start from center (same Y, screen center X)
        centerPos = new Vector2(Screen.width / 2, targetPos.y);

        canvasGroup.alpha = 0f;
        objectiveRect.anchoredPosition = centerPos;
    }

    private void Start()
    {
        ShowObjective(0);
    }

    public void OnGoalCompleted()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SwapObjective());
    }

    private IEnumerator SwapObjective()
    {
        // Fly out
        yield return Animate(centerPos, 0f, false);

        currentObjectiveIndex++;

        if (currentObjectiveIndex >= objectives.Length)
            yield break;

        objectiveText.text = objectives[currentObjectiveIndex];

        // Fly in
        yield return Animate(targetPos, 1f, true);
    }

    private void ShowObjective(int index)
    {
        objectiveText.text = objectives[index];

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Animate(targetPos, 1f, true));
    }

    private IEnumerator Animate(Vector2 endPos, float endAlpha, bool flyIn)
    {
        float time = 0f;

        Vector2 startPos = objectiveRect.anchoredPosition;
        float startAlpha = canvasGroup.alpha;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / transitionDuration);
            float easedT = easeCurve.Evaluate(t);

            objectiveRect.anchoredPosition =
                Vector2.Lerp(startPos, endPos, easedT);

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, easedT);

            yield return null;
        }

        objectiveRect.anchoredPosition = endPos;
        canvasGroup.alpha = endAlpha;
    }
}
