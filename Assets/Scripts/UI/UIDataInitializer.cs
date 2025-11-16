using UI;
using UnityEngine;

/// Runs very early so all bars start from UIData defaults before anything else reads them.
[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public sealed class UIDataInitializer : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UIScriptableObject _uiData;

    [Header("Reset Policy")]
    [Tooltip("Reset every time this scene loads.")]
    [SerializeField] private bool _resetOnSceneLoad = true;

    [Tooltip("If true, reset once per app run only.")]
    [SerializeField] private bool _oncePerProcess = false;

    private static bool s_alreadyResetThisProcess;

    private void Awake()
    {
        if (_uiData == null) { Debug.LogError($"{nameof(UIDataInitializer)}: UIData asset not assigned.", this); return; }

        if (!_resetOnSceneLoad) return;
        if (_oncePerProcess && s_alreadyResetThisProcess) return;

        _uiData.ResetAllToDefaults();
        s_alreadyResetThisProcess = true;
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Now (Apply Defaults)")]
    private void ResetNow()
    {
        if (_uiData == null) return;
        _uiData.ResetAllToDefaults();
        Debug.Log("UIData defaults applied.", this);
    }
#endif
}
