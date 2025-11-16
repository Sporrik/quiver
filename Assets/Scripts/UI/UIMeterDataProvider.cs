using UnityEngine;

namespace UI
{
    /// Provides a shared UIData asset to all UIMeter instances (no per-bar refs).
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class UIMeterDataProvider : MonoBehaviour
    {
        [SerializeField] private UIScriptableObject _uiData;
        public static UIScriptableObject Shared { get; private set; }

        private void Awake()
        {
            if (_uiData == null)
            {
                Debug.LogError($"{nameof(UIMeterDataProvider)}: UIData asset missing.", this);
                return;
            }
            Shared = _uiData; // make globally available before any meters enable
        }
    }
}