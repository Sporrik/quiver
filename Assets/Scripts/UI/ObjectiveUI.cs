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

    [Header("Glow Settings")]
    [SerializeField] private float glowInPower = 0.6f;
    [SerializeField] private float glowOutPower = 0f;

    private Material textMaterial;
    private int GlowPowerID;

    private Vector2 targetPos;
    private Vector2 centerPos;
    private int currentObjectiveIndex = 0;
    private Coroutine currentRoutine;

    //[SerializeField] private MinigameManager _minigameManager;

    private void Awake()
    {
        targetPos = objectiveRect.anchoredPosition;

        // Start from center (same Y, screen center X)
        centerPos = new Vector2(Screen.width / 2, targetPos.y);

        canvasGroup.alpha = 0f;
        objectiveRect.anchoredPosition = centerPos;

        GlowPowerID = TMPro.ShaderUtilities.ID_GlowPower;

        textMaterial = Instantiate(objectiveText.fontMaterial);
        objectiveText.fontMaterial = textMaterial;

        textMaterial.SetFloat(GlowPowerID, glowOutPower);
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

        float startGlow = textMaterial.GetFloat(GlowPowerID);
        float endGlow = flyIn ? glowInPower : glowOutPower;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / transitionDuration);
            float easedT = easeCurve.Evaluate(t);

            objectiveRect.anchoredPosition =
                Vector2.Lerp(startPos, endPos, easedT);

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, easedT);

            textMaterial.SetFloat(GlowPowerID, Mathf.Lerp(startGlow, endGlow, easedT));

            yield return null;
        }

        objectiveRect.anchoredPosition = endPos;
        canvasGroup.alpha = endAlpha;
        textMaterial.SetFloat(GlowPowerID, endGlow);
    }

    //private void OnEnable()
    //{
    //    if (_minigameManager == null)
    //    {
    //        Debug.LogError("ObjectiveUI: MinigameManager reference not set.", this);
    //        return;
    //    }

    //    _minigameManager.Opened += OnMinigameOpened;
    //    _minigameManager.Closed += OnMinigameClosed;
    //}

    //private void OnDisable()
    //{
    //    if (_minigameManager == null) return;

    //    _minigameManager.Opened -= OnMinigameOpened;
    //    _minigameManager.Closed -= OnMinigameClosed;
    //}

    //private void OnMinigameOpened(string sceneName)
    //{
    //    canvasGroup.alpha = 0f;
    //    canvasGroup.interactable = false;
    //    canvasGroup.blocksRaycasts = false;
    //}

    //private void OnMinigameClosed(string sceneName)
    //{
    //    canvasGroup.alpha = 1f;
    //    canvasGroup.interactable = true;
    //    canvasGroup.blocksRaycasts = true;
    //}
}
